using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;
using VideoEditorD3D.Direct3D.Vertices;
using VideoEditorD3D.Timers;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Extentions;
using VideoEditorD3D.Direct3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D;

public partial class ApplicationForm : System.Windows.Forms.Form, IApplicationForm
{
    #region Initilized at Constructor
    private readonly IApplicationContext Application;
    private readonly Lock UILock;
    private readonly Stopwatch Stopwatch;
    private readonly AllTimers Timers;
    private readonly ApplicationFormEventHandlers EventHandler;
    private readonly IDrawerThread DrawerThread;
    private bool ReInitialize;
    private bool Initialized;
    #endregion

    #region Initialized at OnHandleCreated
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
    private BlendState? _AlphaBlendState;
    private CharacterCollection? _Characters;
    private Forms.Form? _CurrentForm;
    private int? _PhysicalWidth;
    private int? _PhysicalHeight;
    #endregion

    public ApplicationForm(IApplicationContext application)
    {
        Application = application;
        UILock = new Lock();
        Stopwatch = new Stopwatch();
        Timers = new AllTimers(Stopwatch);
        DrawerThread = application.OnCreateDrawerThread(this) ?? new Default60FpsDrawerThread(this, application);
        EventHandler = new ApplicationFormEventHandlers(this, application);

        InitializeComponents();
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
                Application.Logger.WriteException(ex);
            }
            return res;
        }
    }
    private bool IsNotReadyToDraw => !Initialized || Width == 0 || Height == 0 || Application.KillSwitch;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RecreateSwapChain();
    }
    protected override void OnLoad(EventArgs e)
    {
        CenterToScreen();
        Stopwatch.Start();
        DrawerThread.StartThread();
        _CurrentForm!.OnLoad();
    }
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        ResizeSwapChain();
    }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        lock (UILock)
        {
            Application.KillSwitch = true;
            DrawerThread.Dispose();
            base.OnFormClosing(e);
        }
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

        DrawerThread.Dispose(); // Zou al disposed moeten zijn, maar goed
        Application.Dispose();
    }

    private void InitializeComponents()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(1280, 720);
        this.Text = "Video editor";
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        this.AllowDrop = true;
        this.MouseMove += EventHandler.OnMouseMove;
        this.MouseDown += EventHandler.OnMouseDown;
        this.MouseUp += EventHandler.OnMouseUp;
        this.MouseClick += EventHandler.OnMouseClick;
        this.MouseDoubleClick += EventHandler.OnMouseDoubleClick;
        this.MouseWheel += EventHandler.OnMouseWheel;
        this.KeyDown += EventHandler.OnKeyDown;
        this.KeyUp += EventHandler.OnKeyUp;
        this.KeyPress += EventHandler.OnKeyPress;
        this.DragDrop += EventHandler.OnDragDrop;
        this.DragEnter += EventHandler.OnDragEnter;
        this.DragOver += EventHandler.OnDragOver;
        this.DragLeave += EventHandler.OnDragLeave;
        this.ResumeLayout(false);
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
                _AlphaBlendState?.Dispose(); 

                int realWidth = ClientRectangle.Width;
                int realHeight = ClientRectangle.Height;

                if (realWidth == 0 || realHeight == 0)
                    return;

                _PhysicalWidth = realWidth;
                _PhysicalHeight = realHeight;

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
                    ModeDescription = new ModeDescription(realWidth, realHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
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

                AfterResizeOrRecreateSwapChain(realWidth, realHeight);
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


                var blendDesc = new BlendStateDescription
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false
                };

                blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
                {
                    IsBlendEnabled = true,
                    SourceBlend = BlendOption.SourceAlpha,
                    DestinationBlend = BlendOption.InverseSourceAlpha,
                    BlendOperation = BlendOperation.Add,
                    SourceAlphaBlend = BlendOption.One,
                    DestinationAlphaBlend = BlendOption.Zero,
                    AlphaBlendOperation = BlendOperation.Add,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };

                _AlphaBlendState = new BlendState(_Device, blendDesc);
                _DeviceContext.OutputMerger.SetBlendState(_AlphaBlendState);

                _Characters = new CharacterCollection(this);
                _CurrentForm = Application.OnCreateStartForm(this);
                _CurrentForm.Width = realWidth;
                _CurrentForm.Height = realHeight;

                ReInitialize = false;
                Initialized = true;
            }
            catch (Exception ex)
            {
                // Waarschijnlijk aan het afsluiten.
                Application.Logger.WriteException(ex);
            }
        }
    }
    private void ResizeSwapChain()
    {
        if (IsNotReadyToDraw || _Device == null || _SwapChain == null || _CurrentForm == null)
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

            int newWidth = ClientRectangle.Width;
            int newHeight = ClientRectangle.Height;

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
                Application.Logger.WriteException(ex);
                return;
            }

            AfterResizeOrRecreateSwapChain(newWidth, newHeight);
        }
    }
    private void AfterResizeOrRecreateSwapChain(int width, int height)
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
            var currentForm = _CurrentForm;
            if (IsNotReadyToDraw || currentForm == null) return;

            Timers.OnUpdateTimer.Start();
            currentForm.OnUpdate();
            Timers.OnUpdateTimer.Stop();

            Timers.DrawTimer.Start();
            // Op het aller laatste moment nog een keer checken op
            // Application.KillSwitch om deadlocks te voorkomen
            if (IsNotReadyToDraw) return; 
            // Gelockt starten van de render
            lock (UILock) RenderToGpu(currentForm);
            Timers.DrawTimer.Stop();

            Timers.FpsTimer.CountFps();
        }
        catch (Exception ex)
        {
            // Dit zou niet meer voor kunnen komen nu
            Application.Logger.WriteException(ex);

            // Checken omdat de KillSwitch inmiddels ook aangezet kan zijn
            if (!IsNotReadyToDraw)
            {
                RecreateSwapChain();
            }
        }
    }
    private void RenderToGpu(Forms.Form currentForm)
    {
        if (IsNotReadyToDraw || _DeviceContext == null || _Device == null || _SwapChain == null)
            return;

        _DeviceContext.ClearRenderTargetView(_RenderTargetView, currentForm.BackgroundColor);

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

    #region IApplicationForm interface
    Device IApplicationForm.Device => _Device!;
    CharacterCollection IApplicationForm.Characters => _Characters!;
    int IApplicationForm.Width => _PhysicalWidth!.Value;
    int IApplicationForm.Height => _PhysicalHeight!.Value;
    Forms.Form IApplicationForm.CurrentForm { get => _CurrentForm!; set => _CurrentForm = value; }
    Stopwatch IApplicationForm.Stopwatch => Stopwatch;
    AllTimers IApplicationForm.Timers => Timers;
    #endregion
}