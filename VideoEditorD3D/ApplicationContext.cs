using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Types;
using VideoEditorD3D.Direct3D.Canvas;
using VideoEditorD3D.Timers;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Entities;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.ComponentModel;
using VideoEditorD3D.Drawers;

namespace VideoEditorD3D;

public partial class ApplicationContext : Form
{
    public ApplicationContext()
    {
        Logger = new ConsoleLogger();
        Config = ApplicationConfig.Load();
        if (Config.LastDatabaseFullName == null)
        {
            Database = new ApplicationDbContext($"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip");
            Config.LastDatabaseFullName = Database.FullName;
            Config.Save();
        }
        else
        {
            Database = new ApplicationDbContext(Config.LastDatabaseFullName);
        }
        Controller = new Application(this);
        Stopwatch = new Stopwatch();
        Timers = new AllTimers(Stopwatch);
        UILock = new Lock();

        this.SuspendLayout();
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Video editor";
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        this.MouseWheel += Controller.OnMouseWheel;
        this.MouseMove += Controller.OnMouseMove;
        this.MouseDown += Controller.MouseDown;
        this.MouseUp += Controller.MouseUp;
        this.MouseClick += Controller.MouseClick;
        this.KeyDown += Controller.OnKeyDown;
        this.KeyUp += Controller.OnKeyUp;
        this.KeyPress += Controller.OnKeyPress;
        this.ResumeLayout(false);
    }

    public ConsoleLogger Logger { get; }
    public ApplicationConfig Config { get; }
    public ApplicationDbContext Database { get; }
    public Application Controller { get; }
    public Stopwatch Stopwatch { get; }
    public AllTimers Timers { get; }

    private readonly Lock UILock;
    private Device? _Device;
    private int _PhysicalWidth;
    private int _PhysicalHeight;
    private WindowsScaling? WindowsScaling;
    private DeviceContext? Context;
    private SwapChain? SwapChain;
    private RenderTargetView? RenderTargetView;
    private VertexShader? NormalVertexShader;
    private PixelShader? NormalPixelShader;
    private InputLayout? NormalInputLayout;
    private VertexShader? BitmapVertexShader;
    private PixelShader? BitmapPixelShader;
    private InputLayout? BitmapInputLayout;
    private Buffer? VertexBuffer;
    private SamplerState? SamplerState;
    private CharacterCollection? _Characters;
    private ApplicationDrawer? Drawer;
    private bool ReInitialize;
    private bool Initialized;

    public Device Device => _Device!;
    public CharacterCollection Characters => _Characters!;
    public int PhysicalWidth => _PhysicalWidth;
    public int PhysicalHeight => _PhysicalHeight;

    private bool IsMinimized
    {
        get
        {
            var res = true;
            try
            {
                res = WindowState == FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            return res;
        }
    }
    private bool IsNotReadyToDraw => !Initialized || Width == 0 || Height == 0;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        WindowsScaling = new WindowsScaling(Handle);
        RecreateSwapChain();
    }
    protected override void OnLoad(EventArgs e)
    {
        Controller.StartThread();
        CenterToScreen();
    }
    protected override void OnResize(EventArgs e)
    {
        if (IsNotReadyToDraw || _Device == null || WindowsScaling == null || SwapChain == null)
            return;

        if (IsMinimized)
        {
            ReInitialize = true;
            Initialized = false;
            return;
        }

        if (ReInitialize)
        {
            RecreateSwapChain();
            return;
        }

        lock (UILock)
        {
            _Device.ImmediateContext.ClearState();
            RenderTargetView?.Dispose();

            int newWidth = Convert.ToInt32(Width * WindowsScaling.Scaling);
            int newHeight = Convert.ToInt32(Height * WindowsScaling.Scaling);

            if (newWidth == 0 || newHeight == 0)
                return;

            _PhysicalWidth = newWidth;
            _PhysicalHeight = newHeight;

            try
            {
                SwapChain.ResizeBuffers(1, _PhysicalWidth, _PhysicalHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            }
            catch (SharpDXException ex)
            {
                Logger.WriteException(ex);
                return;
            }

            AfterResizeOrRecreate();
        }

        base.OnResize(e);
    }
    private void RecreateSwapChain()
    {
        lock (UILock)
        {
            try
            {
                NormalInputLayout?.Dispose();
                NormalVertexShader?.Dispose();
                NormalPixelShader?.Dispose();
                RenderTargetView?.Dispose();
                SwapChain?.Dispose();
                Context?.Dispose();
                _Device?.Dispose();
                SamplerState?.Dispose();
                _Characters?.Dispose();

                _PhysicalWidth = Convert.ToInt32(Width * WindowsScaling!.Scaling);
                _PhysicalHeight = Convert.ToInt32(Height * WindowsScaling!.Scaling);

                var sampleSize = 1; // No MSAA
                using (var tempDevice = new Device(DriverType.Hardware, DeviceCreationFlags.None))
                {
                    int quality4 = tempDevice.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 4);
                    if (quality4 > 0)
                        sampleSize = 4; // MSAA4 supported
                    int quality8 = tempDevice.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 8);
                    if (quality8 > 0)
                        sampleSize = 8; // MSAA8 supported
                }

                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(_PhysicalWidth, _PhysicalHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = Handle,
                    SampleDescription = new SampleDescription(sampleSize, 0),
                    IsWindowed = true,
                    SwapEffect = SwapEffect.Discard
                };

                Device.CreateWithSwapChain(
                    DriverType.Hardware,
                    DeviceCreationFlags.BgraSupport,
                    swapChainDesc,
                    out _Device, out SwapChain);

                Context = _Device.ImmediateContext;

                AfterResizeOrRecreate();
                CompileShaders();

                SamplerState = new SamplerState(_Device, new SamplerStateDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    ComparisonFunction = Comparison.Never,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                });
                Context.PixelShader.SetSampler(0, SamplerState);

                _Characters = new CharacterCollection(_Device);
                Drawer = new ApplicationDrawer(this);

                ReInitialize = false;
                Initialized = true;
            }
            catch (Exception ex)
            {
                // Waarschijnlijk aan het afsluiten.
                Logger.WriteException(ex);
            }
        }
    }
    private void AfterResizeOrRecreate()
    {
        Utilities.Dispose(ref RenderTargetView);

        using (var backBuffer = SwapChain!.GetBackBuffer<Texture2D>(0))
        {
            RenderTargetView = new RenderTargetView(_Device, backBuffer);
        }

        Context!.OutputMerger.SetRenderTargets(RenderTargetView);
        Context!.Rasterizer.SetViewport(0, 0, _PhysicalWidth, _PhysicalHeight);
    }
    private void CompileShaders()
    {
        // Normale shader (bijvoorbeeld voor lijnen, debug shapes, etc.)
        var normalVertexShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.Compile(@"
                struct VS_INPUT {
                    float2 pos : POSITION;
                    float4 color : COLOR;
                };
                struct PS_INPUT {
                    float4 pos : SV_POSITION;
                    float4 color : COLOR;
                };
                PS_INPUT VSMain(VS_INPUT input) {
                    PS_INPUT output;
                    output.pos = float4(input.pos, 0, 1);
                    output.color = input.color;
                    return output;
                }",
            "VSMain", "vs_4_0");

        var normalPixelShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.Compile(@"
                struct PS_INPUT {
                    float4 pos : SV_POSITION;
                    float4 color : COLOR;
                };
                float4 PSMain(PS_INPUT input) : SV_Target {
                    return input.color;
                }",
            "PSMain", "ps_4_0");

        NormalVertexShader = new VertexShader(_Device, normalVertexShaderByteCode);
        NormalPixelShader = new PixelShader(_Device, normalPixelShaderByteCode);

        // Input layout voor normale shader
        NormalInputLayout = new InputLayout(_Device,
            SharpDX.D3DCompiler.ShaderSignature.GetInputSignature(normalVertexShaderByteCode),
            [
                new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 8, 0),
            ]);

        // Bitmap shader met UV en Texture sampling
        var bitmapVertexShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.Compile(@"
                struct VS_INPUT {
                    float2 pos : POSITION;
                    float2 uv : TEXCOORD;
                };
                struct PS_INPUT {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };
                PS_INPUT VSMain(VS_INPUT input) {
                    PS_INPUT output;
                    output.pos = float4(input.pos, 0, 1);
                    output.uv = input.uv;
                    return output;
                }",
            "VSMain", "vs_4_0");

        var bitmapPixelShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.Compile(@"
                Texture2D tex : register(t0);
                SamplerState samp : register(s0);

                struct PS_INPUT {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };
                float4 PSMain(PS_INPUT input) : SV_Target {
                    return tex.Sample(samp, input.uv);
                }",
            "PSMain", "ps_4_0");

        BitmapVertexShader = new VertexShader(_Device, bitmapVertexShaderByteCode);
        BitmapPixelShader = new PixelShader(_Device, bitmapPixelShaderByteCode);

        // Input layout voor bitmap shader
        BitmapInputLayout = new InputLayout(_Device,
            SharpDX.D3DCompiler.ShaderSignature.GetInputSignature(bitmapVertexShaderByteCode),
            [
                new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
            ]);
    }

    public void Draw()
    {
        if (IsNotReadyToDraw)
            return;

        try
        {
            LoadDataAndRenderToGpu();
        }
        catch (Exception ex)
        {
            // Waarschijnlijk aan het afsluiten.
            Logger.WriteException(ex);

            // Deze check doen we omdat we niet weten hoeveel tijd er voorbij is gegaan
            if (IsNotReadyToDraw)
                return;

            RecreateSwapChain();
        }
    }
    private void LoadDataAndRenderToGpu()
    {
        if (Drawer == null || IsNotReadyToDraw)
            return;

        Timers.GraphDrawCanvasTimer.Start();

        using (var compiledCanvas = Drawer.DrawCanvas())
        {
            Timers.GraphDrawCanvasTimer.Stop();

            Timers.GraphDrawToGpuTimer.Start();

            // Nu hebben we alles voor onszelf
            RenderToGpu(compiledCanvas);

            Timers.GraphDrawToGpuTimer.Stop();

        }

        Timers.FpsTimer.CountFps();
    }
    private void RenderToGpu(Canvas compiledCanvas)
    {
        lock (UILock)
        {
            if (IsNotReadyToDraw || Context == null || _Device == null || SwapChain == null)
                return;

            Context.ClearRenderTargetView(RenderTargetView, compiledCanvas.BackgroundColor);

            foreach (var layer in compiledCanvas.Layers)
            {
                // Triangles
                if (layer.FillVertices.Count > 0)
                {
                    VertexBuffer?.Dispose();
                    VertexBuffer = Buffer.Create(_Device, BindFlags.VertexBuffer, layer.FillVertices.ToArray());

                    Context.InputAssembler.InputLayout = NormalInputLayout;
                    Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<Vertex>(), 0));
                    Context.VertexShader.Set(NormalVertexShader);
                    Context.PixelShader.Set(NormalPixelShader);

                    Context.Draw(layer.FillVertices.Count, 0);
                }

                // Lines
                if (layer.LineVertices.Count > 0)
                {
                    VertexBuffer?.Dispose();
                    VertexBuffer = Buffer.Create(_Device, BindFlags.VertexBuffer, layer.LineVertices.ToArray());

                    Context.InputAssembler.InputLayout = NormalInputLayout;
                    Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                    Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<Vertex>(), 0));
                    Context.VertexShader.Set(NormalVertexShader);
                    Context.PixelShader.Set(NormalPixelShader);

                    Context.Draw(layer.LineVertices.Count, 0);
                }

                // Images
                foreach (var bitmap in layer.Images)
                {
                    VertexBuffer?.Dispose();
                    VertexBuffer = Buffer.Create(_Device, BindFlags.VertexBuffer, bitmap.Vertices);

                    Context.InputAssembler.InputLayout = BitmapInputLayout;
                    Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<TextureVertex>(), 0));
                    Context.VertexShader.Set(BitmapVertexShader);
                    Context.PixelShader.Set(BitmapPixelShader);
                    Context.PixelShader.SetShaderResource(0, bitmap.Texture.TextureView);

                    Context.Draw(bitmap.Vertices.Length, 0);
                }
            }

            SwapChain.Present(1, PresentFlags.None);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _Characters?.Dispose();
        VertexBuffer?.Dispose();
        NormalInputLayout?.Dispose();
        NormalVertexShader?.Dispose();
        NormalPixelShader?.Dispose();
        RenderTargetView?.Dispose();
        SwapChain?.Dispose();
        Context?.Dispose();
        _Device?.Dispose();
        Controller.Dispose();
        Database.Dispose();
    }
}
