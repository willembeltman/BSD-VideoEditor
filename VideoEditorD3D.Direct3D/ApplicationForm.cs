using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows.Forms;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Timers;
using VideoEditorD3D.Direct3D.Vertices;
using Device = SharpDX.Direct3D11.Device;
using FormCollection = VideoEditorD3D.Direct3D.Collections.FormCollection;

namespace VideoEditorD3D.Direct3D;

public partial class ApplicationForm : System.Windows.Forms.Form
{
    #region Initilized at Constructor
    private bool KillSwitch;
    private readonly IApplicationState ApplicationContext;
    private readonly Lock UILock;
    private readonly Stopwatch Stopwatch;
    private readonly AllTimers Timers;
    private bool ReInitialize;
    private bool Initialized;
    private bool IsClosed;
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
    private FormCollection? _Forms;

    private IDrawerThread? _DrawerThread;
    private int? _PhysicalWidth;
    private int? _PhysicalHeight;
    #endregion

    public ApplicationForm(IApplicationState applicationContext)
    {
        ApplicationContext = applicationContext;
        UILock = new Lock();
        Stopwatch = new Stopwatch();
        Timers = new AllTimers(Stopwatch);

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
                ApplicationContext.Logger?.WriteException(ex);
            }
            return res;
        }
    }
    private bool IsNotReadyToDraw => !Initialized || IsClosed || KillSwitch || Width == 0 || Height == 0;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RecreateSwapChain();
    }
    protected override void OnLoad(EventArgs e)
    {
        CenterToScreen();
        Stopwatch.Start();
        ApplicationContext.Logger?.StartThread();
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
            IsClosed = true;
            KillSwitch = true;
            _DrawerThread?.Dispose();
            base.OnFormClosing(e);
        }
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        KillSwitch = true;

        _Forms?.Dispose();
        _Characters?.Dispose();
        _NormalInputLayout?.Dispose();
        _NormalVertexShader?.Dispose();
        _NormalPixelShader?.Dispose();
        _RenderTargetView?.Dispose();
        _SwapChain?.Dispose();
        _DeviceContext?.Dispose();
        _Device?.Dispose();

        _DrawerThread?.Dispose(); // Zou al disposed moeten zijn, maar goed
        ApplicationContext.Dispose();
    }

    private void InitializeComponents()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(1280, 720);
        this.Text = "Video editor";
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        this.AllowDrop = true;
        this.MouseMove += OnMouseMove;
        this.MouseDown += OnMouseDown;
        this.MouseUp += OnMouseUp;
        this.MouseClick += OnMouseClick;
        this.MouseDoubleClick += OnMouseDoubleClick;
        this.MouseWheel += OnMouseWheel;
        this.MouseEnter += OnMouseEnter;
        this.MouseLeave += OnMouseLeave;
        this.KeyDown += OnKeyDown;
        this.KeyUp += OnKeyUp;
        this.KeyPress += OnKeyPress;
        this.DragDrop += OnDragDrop;
        this.DragEnter += OnDragEnter;
        this.DragOver += OnDragOver;
        this.DragLeave += OnDragLeave;
        this.ResumeLayout(false);

        Thread.CurrentThread.Name = "Form Kernel";
    }
    private void RecreateSwapChain()
    {
        lock (UILock)
        {
            try
            {
                // Dispose previous resources
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
                _DrawerThread?.Dispose();
                _Forms?.Dispose();

                // Get the width and height
                int realWidth = ClientRectangle.Width;
                int realHeight = ClientRectangle.Height;

                if (realWidth == 0 || realHeight == 0)
                    return;

                // Store the physical width and height
                _PhysicalWidth = realWidth;
                _PhysicalHeight = realHeight;

                // Detect the sample size for MSAA
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

                // Create swap chain and device and set them up
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

                // Compile the shaders and create the input layouts
                CompileShaders();

                // Create the sampler state and set it to the pixel shader
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

                // Create a new character collection attached to the current device
                _Characters = new CharacterCollection(this);

                _DrawerThread = ApplicationContext.OnCreateDrawerThread(this) ?? new Default60FpsDrawerThread(this);
                _DrawerThread?.StartThread();

                // Ask the application to create the start form
                _Forms = new FormCollection(this);
                var startForm = ApplicationContext.OnCreateMainForm(this);
                startForm.Width = realWidth;
                startForm.Height = realHeight;
                _Forms.Add(startForm);

                // Setup state switches be ready to draw
                ReInitialize = false;
                Initialized = true;
            }
            catch (Exception ex)
            {
                ApplicationContext.Logger?.WriteException(ex);
            }
        }
    }
    private void ResizeSwapChain()
    {
        if (IsClosed || KillSwitch || Width == 0 || Height == 0 || _Device == null || _SwapChain == null || (_Forms?.Count ?? 0) < 1)
            return;

        if (IsMinimized)
        {
            // Resize is called when the window is minimized, so we need to setup
            // reinitialize state here and exit the method
            ReInitialize = true;
            Initialized = false;
            return;
        }

        if (ReInitialize)
        {
            // When the window is restored, again resize is called, so we need to
            // reinitialize the swap chain here, this will setup the swap chain
            // with the new width and height anyways so we can exit the method
            RecreateSwapChain();
            return;
        }

        lock (UILock)
        {
            _Device.ImmediateContext.ClearState();
            _RenderTargetView?.Dispose();

            // Get the new width and height
            int newWidth = ClientRectangle.Width;
            int newHeight = ClientRectangle.Height;
            if (newWidth == 0 || newHeight == 0)
                return;

            // Store the new physical width and height inside of our own state first
            // these values are exposed through the IApplicationForm interface,
            // so all future event handling will use these new values
            _PhysicalWidth = newWidth;
            _PhysicalHeight = newHeight;

            // Then set the width and height of the current form, this will trigger
            // the OnResize event of the current form and force redraw on next frame
            _Forms!.First().Width = newWidth;
            _Forms!.First().Height = newHeight;

            // Then try to resize the swap chain
            try
            {
                _SwapChain.ResizeBuffers(1, newWidth, newHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            }
            catch (SharpDXException ex)
            {
                ApplicationContext.Logger?.WriteException(ex);
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



        // Create the alpha blend state to handle transparency and set it to the output merger
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

    public void CloseForm()
    {
        if (IsClosed)
            return;

        if (InvokeRequired)
        {
            Invoke(new Action(CloseForm));
            return;
        }

        Close();
    }

}
