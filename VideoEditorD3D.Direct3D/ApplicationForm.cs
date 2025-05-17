using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Timers;
using Device = SharpDX.Direct3D11.Device;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Direct3D;

public class ApplicationForm : System.Windows.Forms.Form, IApplicationForm
{
    #region Initilized at Constructor
    private readonly IApplicationContext ApplicationContext;
    private readonly ApplicationFormEvents ApplicationFormEvents;
    private readonly PopupCollection Popups;
    private readonly Lock UILock;
    private readonly Stopwatch Stopwatch;
    private readonly AllTimers Timers;
    private readonly IDrawerThread DrawerThread;
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
    private Forms.Form? _CurrentForm;
    private int? _PhysicalWidth;
    private int? _PhysicalHeight;
    #endregion

    #region (Private) IApplicationForm interface
    Device IApplicationForm.Device => _Device!;
    CharacterCollection IApplicationForm.Characters => _Characters!;
    int IApplicationForm.Width => _PhysicalWidth!.Value;
    int IApplicationForm.Height => _PhysicalHeight!.Value;
    Forms.Form IApplicationForm.CurrentForm { get => _CurrentForm!; set => _CurrentForm = value; }
    Stopwatch IApplicationForm.Stopwatch => Stopwatch;
    AllTimers IApplicationForm.Timers => Timers;
    PopupCollection IApplicationForm.Popups => Popups;
    #endregion

    public ApplicationForm(IApplicationContext applicationContext)
    {
        ApplicationContext = applicationContext;
        Popups = new PopupCollection(this);
        UILock = new Lock();
        Stopwatch = new Stopwatch();
        Timers = new AllTimers(Stopwatch);
        DrawerThread = applicationContext.OnCreateDrawerThread(this) ?? new Default60FpsDrawerThread(this, applicationContext);
        ApplicationFormEvents = new ApplicationFormEvents(this);

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
    private bool IsNotReadyToDraw => !Initialized || IsClosed || ApplicationContext.KillSwitch || Width == 0 || Height == 0;


    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RecreateSwapChain();
    }
    protected override void OnLoad(EventArgs e)
    {
        CenterToScreen();
        Stopwatch.Start();
        ApplicationContext.Start(this);
        DrawerThread.StartThread();
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
            ApplicationContext.KillSwitch = true;
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
        ApplicationContext.Dispose();
    }

    private void InitializeComponents()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(1280, 720);
        this.Text = "Video editor";
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        this.AllowDrop = true;
        this.MouseMove += ApplicationFormEvents.OnMouseMove;
        this.MouseDown += ApplicationFormEvents.OnMouseDown;
        this.MouseUp += ApplicationFormEvents.OnMouseUp;
        this.MouseClick += ApplicationFormEvents.OnMouseClick;
        this.MouseDoubleClick += ApplicationFormEvents.OnMouseDoubleClick;
        this.MouseWheel += ApplicationFormEvents.OnMouseWheel;
        this.MouseEnter += ApplicationFormEvents.OnMouseEnter;
        this.MouseLeave += ApplicationFormEvents.OnMouseLeave;
        this.KeyDown += ApplicationFormEvents.OnKeyDown;
        this.KeyUp += ApplicationFormEvents.OnKeyUp;
        this.KeyPress += ApplicationFormEvents.OnKeyPress;
        this.DragDrop += ApplicationFormEvents.OnDragDrop;
        this.DragEnter += ApplicationFormEvents.OnDragEnter;
        this.DragOver += ApplicationFormEvents.OnDragOver;
        this.DragLeave += ApplicationFormEvents.OnDragLeave;
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

                // Ask the application to create the start form
                _CurrentForm = ApplicationContext.OnCreateStartForm(this);
                _CurrentForm.Width = realWidth;
                _CurrentForm.Height = realHeight;

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
        if (IsNotReadyToDraw || _Device == null || _SwapChain == null || _CurrentForm == null)
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
            _CurrentForm.Width = newWidth;
            _CurrentForm.Height = newHeight;

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

    public void TryDraw()
    {
        if (IsNotReadyToDraw)
            return;

        try
        {
            var currentForm = _CurrentForm;
            if (currentForm == null)
                return;

            Draw(currentForm);
        }
        catch (Exception ex)
        {
            // Dit zou niet meer voor kunnen komen nu
            ApplicationContext.Logger?.WriteException(ex);

            // Checken omdat de KillSwitch inmiddels ook aangezet kan zijn
            if (!IsNotReadyToDraw)
            {
                RecreateSwapChain();
            }
        }
    }
    private void Draw(Forms.Form currentForm)
    {
        if (IsNotReadyToDraw)
            return;

        if (!currentForm.Loaded)
        {
            currentForm.OnLoad();
        }

        using (Timers.OnUpdateTimer.DisposableObject)
        {
            currentForm.OnUpdate();
        }

        using (Timers.RenderToGpuTimer.DisposableObject)
        {
            LockAndRenderToGpu(currentForm);
        }

        Timers.FpsTimer.CountFps();
    }
    private void LockAndRenderToGpu(Forms.Form currentForm)
    {
        if (IsNotReadyToDraw)
            return;

        lock (UILock)
        {
            RenderToGpu(currentForm);
        }
    }
    private void RenderToGpu(Forms.Form currentForm)
    {
        if (IsNotReadyToDraw || _DeviceContext == null || _Device == null || _SwapChain == null)
            return;

        _DeviceContext.ClearRenderTargetView(_RenderTargetView, currentForm.BackColor);

        foreach (GraphicsLayer layer in GetAllLayers())
        {
            if (layer.TriangleVerticesBuffer != null)
            {
                DrawTriangle(_DeviceContext, layer);
            }

            if (layer.LineVerticesBuffer != null)
            {
                DrawLine(_DeviceContext, layer);
            }

            foreach (var image in layer.TextureImages)
            {
                DrawImage(_DeviceContext, image);
            }
        }

        _SwapChain.Present(0, PresentFlags.None);
    }

    private IEnumerable<GraphicsLayer> GetAllLayers()
    {
        if (_CurrentForm != null)
        {
            foreach (var layer in _CurrentForm.GetAllCanvasLayers())
            {
                yield return layer;
            }
        }
        foreach (var popup in Popups)
        {
            foreach (var layer in popup.GetAllCanvasLayers())
            {
                yield return layer;
            }
        }
    }

    private void DrawImage(DeviceContext deviceContext, ITextureImage image)
    {
        deviceContext.InputAssembler.InputLayout = _BitmapInputLayout;
        deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(image.VerticesBuffer, Utilities.SizeOf<TextureVertex>(), 0));
        deviceContext.VertexShader.Set(_BitmapVertexShader);
        deviceContext.PixelShader.Set(_BitmapPixelShader);
        deviceContext.PixelShader.SetShaderResource(0, image.Texture.TextureView);
        deviceContext.Draw(image.Vertices.Length, 0);
    }
    private void DrawLine(DeviceContext deviceContext, GraphicsLayer layer)
    {
        deviceContext.InputAssembler.InputLayout = _NormalInputLayout;
        deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(layer.LineVerticesBuffer, Utilities.SizeOf<Vertex>(), 0));
        deviceContext.VertexShader.Set(_NormalVertexShader);
        deviceContext.PixelShader.Set(_NormalPixelShader);
        deviceContext.Draw(layer.LineVertices.Count, 0);
    }
    private void DrawTriangle(DeviceContext deviceContext, GraphicsLayer layer)
    {
        deviceContext.InputAssembler.InputLayout = _NormalInputLayout;
        deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(layer.TriangleVerticesBuffer, Utilities.SizeOf<Vertex>(), 0));
        deviceContext.VertexShader.Set(_NormalVertexShader);
        deviceContext.PixelShader.Set(_NormalPixelShader);
        deviceContext.Draw(layer.TriangleVertices.Count, 0);
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
// 553 lines isn't that bad, right? ¯\_(ツ)_/¯
//
// (•_•)
//
// ( •_•)>⌐■-■
//
// (⌐■_■)