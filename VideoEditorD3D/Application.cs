using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Types;
using VideoEditorD3D.Timers;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Forms;
using VideoEditorD3D.Direct3D.Forms;
using Device = SharpDX.Direct3D11.Device;
using VideoEditorD3D.Direct3D.Extentions;
using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Types;

namespace VideoEditorD3D;

public partial class Application : Form, IApplication
{
    // Initilized in constructor
    private readonly Lock UILock;
    private readonly ConsoleLogger Logger;
    private readonly ApplicationConfig Config;
    private readonly ApplicationDbContext Db;
    private readonly Router Router;
    private readonly Stopwatch Stopwatch;
    private readonly AllTimers Timers;
    private bool ReInitialize;
    private bool Initialized;
    private bool KillSwitch;

    // Initialized at OnHandleCreated
    private WindowsScaling? _WindowsScaling;
    private DeviceContext? _DeviceContext;
    private SwapChain? _SwapChain;
    private RenderTargetView? _RenderTargetView;
    private VertexShader? _NormalVertexShader;
    private PixelShader? _NormalPixelShader;
    private InputLayout? _NormalInputLayout;
    private VertexShader? _BitmapVertexShader;
    private PixelShader? _BitmapPixelShader;
    private InputLayout? _BitmapInputLayout;
    private SamplerState? _SamplerState;
    private Device? _Device;
    private Characters? _Characters;
    private FormD3D? _CurrentForm;
    private int? _PhysicalWidth;
    private int? _PhysicalHeight;

    public Application()
    {
        Logger = new ConsoleLogger();
        Config = ApplicationConfig.Load();
        if (Config.LastDatabaseFullName == null)
        {
            Db = new ApplicationDbContext($"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip");
            Config.LastDatabaseFullName = Db.FullName;
            Config.Save();
        }
        else
        {
            Db = new ApplicationDbContext(Config.LastDatabaseFullName);
        }
        Router = new Router(this);
        Stopwatch = new Stopwatch();
        Timers = new AllTimers(Stopwatch);
        UILock = new Lock();

        InitializeComponent();
    }
    
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
    private bool IsNotReadyToDraw => !Initialized || Width == 0 || Height == 0 || KillSwitch;

    #region IApplication interface

    ConsoleLogger IApplication.Logger => Logger!;
    FormD3D IApplication.CurrentForm
    {
        get => _CurrentForm!;
        set => _CurrentForm = value;
    }
    ApplicationConfig IApplication.Config => Config;
    ApplicationDbContext IApplication.Db => Db;
    Stopwatch IApplication.Stopwatch => Stopwatch;
    AllTimers IApplication.Timers => Timers;
    bool IApplication.KillSwitch { get => KillSwitch; set => KillSwitch = value; }

    Device IApplicationD3D.Device => _Device!;
    Characters IApplicationD3D.Characters => _Characters!;
    int IApplicationD3D.Width => _PhysicalWidth!.Value;
    int IApplicationD3D.Height => _PhysicalHeight!.Value;


    #endregion

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(800, 450);
        this.Text = "Video editor";
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        this.ResumeLayout(false);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RecreateSwapChain();
    }
    protected override void OnLoad(EventArgs e)
    {
        Router.StartThread();
        CenterToScreen();
        Stopwatch.Start();
    }
    protected override void OnResize(EventArgs e)
    {
        if (IsNotReadyToDraw || _Device == null || _WindowsScaling == null || _SwapChain == null || _CurrentForm == null)
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
            _RenderTargetView?.Dispose();

            int newWidth = Convert.ToInt32(Width * _WindowsScaling.Scaling);
            int newHeight = Convert.ToInt32(Height * _WindowsScaling.Scaling);

            if (newWidth == 0 || newHeight == 0)
                return;

            _PhysicalWidth = newWidth;
            _PhysicalHeight = newHeight;

            _CurrentForm.Width = newWidth;
            _CurrentForm.Height = newHeight;

            try
            {
                _SwapChain.ResizeBuffers(1, newWidth, newHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            }
            catch (SharpDXException ex)
            {
                Logger.WriteException(ex);
                return;
            }

            AfterResizeOrRecreate(newWidth, newHeight);
        }

        base.OnResize(e);
    }
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (_CurrentForm == null) return;
        _CurrentForm.OnKeyPress(e);
    }
    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (_CurrentForm == null) return;
        _CurrentForm.OnKeyUp(e);
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_CurrentForm == null) return;
        _CurrentForm.OnKeyDown(e);
    }
    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseClick(e, new RawVector2(newX, newY));
    }
    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseDoubleClick(e, new RawVector2(newX, newY));
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseUp(e, new RawVector2(newX, newY));
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseDown(e, new RawVector2(newX, newY));
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseMove(e, new RawVector2(newX, newY));
    }
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (_WindowsScaling == null || _CurrentForm == null) return;
        var newX = e.X * _WindowsScaling.Scaling;
        var newY = e.Y * _WindowsScaling.Scaling;
        _CurrentForm.OnMouseWheel(e, new RawVector2(newX, newY));
    }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        lock (UILock)
        {
            KillSwitch = true;
            Router.Dispose();
            base.OnFormClosing(e);
        }
    }

    private void RecreateSwapChain()
    {
        lock (UILock)
        {
            try
            {
                _NormalInputLayout?.Dispose();
                _NormalVertexShader?.Dispose();
                _NormalPixelShader?.Dispose();
                _RenderTargetView?.Dispose();
                _SwapChain?.Dispose();
                _DeviceContext?.Dispose();
                _Device?.Dispose();
                _SamplerState?.Dispose();
                _Characters?.Dispose();

                _WindowsScaling = new WindowsScaling(Handle);

                int realWidth = Convert.ToInt32(Width * _WindowsScaling!.Scaling);
                int realHeight = Convert.ToInt32(Height * _WindowsScaling!.Scaling);

                if (realWidth == 0 || realHeight == 0)
                    return;

                _PhysicalWidth = realWidth;
                _PhysicalHeight = realHeight;

                var sampleSize = 1; // No MSAA
                //using (var tempDevice = new Device(DriverType.Hardware, DeviceCreationFlags.None))
                //{
                //    int quality4 = tempDevice.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 4);
                //    if (quality4 > 0)
                //        sampleSize = 4; // MSAA4 supported
                //    int quality8 = tempDevice.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 8);
                //    if (quality8 > 0)
                //        sampleSize = 8; // MSAA8 supported
                //}

                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(realWidth, realHeight, new Rational(120, 1), Format.R8G8B8A8_UNorm),
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
                    out _Device, out _SwapChain);

                _DeviceContext = _Device.ImmediateContext;

                AfterResizeOrRecreate(realWidth, realHeight);
                CompileShaders();

                _SamplerState = new SamplerState(_Device, new SamplerStateDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    ComparisonFunction = Comparison.Never,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                });
                _DeviceContext.PixelShader.SetSampler(0, _SamplerState);

                _Characters = new Characters(this);
                _CurrentForm = new MainForm(this)
                {
                    Width = realWidth,
                    Height = realHeight
                };

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
    private void AfterResizeOrRecreate(int width, int height)
    {
        Utilities.Dispose(ref _RenderTargetView);

        using (var backBuffer = _SwapChain!.GetBackBuffer<Texture2D>(0))
        {
            _RenderTargetView = new RenderTargetView(_Device, backBuffer);
        }

        _DeviceContext!.OutputMerger.SetRenderTargets(_RenderTargetView);
        _DeviceContext!.Rasterizer.SetViewport(0, 0, width, height);
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

        _NormalVertexShader = new VertexShader(_Device, normalVertexShaderByteCode);
        _NormalPixelShader = new PixelShader(_Device, normalPixelShaderByteCode);

        // Input layout voor normale shader
        _NormalInputLayout = new InputLayout(_Device,
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

        _BitmapVertexShader = new VertexShader(_Device, bitmapVertexShaderByteCode);
        _BitmapPixelShader = new PixelShader(_Device, bitmapPixelShaderByteCode);

        // Input layout voor bitmap shader
        _BitmapInputLayout = new InputLayout(_Device,
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
            Timers.FpsTimer.SleepTillNextFrame(new Fps(1, 25));

            var currentForm = _CurrentForm;
            if (IsNotReadyToDraw || currentForm == null)
                return;

            Timers.OnUpdateTimer.Start();
            currentForm.OnUpdate();
            Timers.OnUpdateTimer.Stop();

            Timers.DrawTimer.Start();
            lock (UILock) RenderToGpu(currentForm);
            Timers.DrawTimer.Stop();

            Timers.FpsTimer.CountFps();
        }
        catch (Exception ex)
        {
            // Waarschijnlijk aan het afsluiten.
            Logger.WriteException(ex);

            // Deze check doen we omdat we niet weten hoeveel tijd er voorbij is gegaan
            if (!IsNotReadyToDraw)
            {
                RecreateSwapChain();
            }
        }
    }
    private void RenderToGpu(FormD3D currentForm)
    {
        if (IsNotReadyToDraw || _DeviceContext == null || _Device == null || _SwapChain == null)
            return;

        _DeviceContext.ClearRenderTargetView(_RenderTargetView, currentForm.BackgroundColor.ToSharpDXColor());

        foreach (var layer in currentForm.GetCanvasLayers())
        {
            // Triangles
            if (layer.FillVerticesBuffer != null)
            {
                _DeviceContext.InputAssembler.InputLayout = _NormalInputLayout;
                _DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                _DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(layer.FillVerticesBuffer, Utilities.SizeOf<Vertex>(), 0));
                _DeviceContext.VertexShader.Set(_NormalVertexShader);
                _DeviceContext.PixelShader.Set(_NormalPixelShader);

                _DeviceContext.Draw(layer.FillVertices.Count, 0);
            }

            // Lines
            if (layer.LineVerticesBuffer != null)
            {
                _DeviceContext.InputAssembler.InputLayout = _NormalInputLayout;
                _DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                _DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(layer.LineVerticesBuffer, Utilities.SizeOf<Vertex>(), 0));
                _DeviceContext.VertexShader.Set(_NormalVertexShader);
                _DeviceContext.PixelShader.Set(_NormalPixelShader);

                _DeviceContext.Draw(layer.LineVertices.Count, 0);
            }

            // Images
            var images = layer.ImageTextures.Cast<ITextureImage>()
                .Concat(layer.CharacterTextures.Cast<ITextureImage>());
            foreach (var image in images)
            {
                _DeviceContext.InputAssembler.InputLayout = _BitmapInputLayout;
                _DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                _DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(image.VerticesBuffer, Utilities.SizeOf<TextureVertex>(), 0));
                _DeviceContext.VertexShader.Set(_BitmapVertexShader);
                _DeviceContext.PixelShader.Set(_BitmapPixelShader);
                _DeviceContext.PixelShader.SetShaderResource(0, image.Texture.TextureView);

                _DeviceContext.Draw(image.Vertices.Length, 0);
            }
        }

        _SwapChain.Present(0, PresentFlags.None);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _Characters?.Dispose();
        _NormalInputLayout?.Dispose();
        _NormalVertexShader?.Dispose();
        _NormalPixelShader?.Dispose();
        _RenderTargetView?.Dispose();
        _SwapChain?.Dispose();
        _DeviceContext?.Dispose();
        _Device?.Dispose();
        Router.Dispose();
        Db.Dispose();
    }
}