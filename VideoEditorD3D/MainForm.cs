using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.Diagnostics;
using VideoEditorD3D.Direct3D;
using VideoeditorD3D.Direct3D.Types;
using VideoEditorD3D.Direct3D.Canvas;
using VideoEditorD3D.Timers;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Interfaces;
using VideoEditorD3D.Engine;
using VideoEditorD3D.Direct3D.Types;

namespace VideoeditorD3D
{
    public partial class MainForm : Form, IApplication
    {
        public MainForm()
        {
            Logger = new ConsoleLogger(this);
            Config = ApplicationConfig.Load();
            if (Config.LastDatabaseFullName == null)
            {
                Database = new ProjectDbContext($"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip");
                Config.LastDatabaseFullName = Database.FullName;
                Config.Save();
            }
            else
            {
                Database = new ProjectDbContext(Config.LastDatabaseFullName);
            }
            Timeline = new Timeline(this);
            Timers = new AllTimers(this);
            Stopwatch = new Stopwatch();
            UILock = new object();

            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Video editor";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            this.MouseWheel += Timeline.OnMouseWheel;
            this.MouseMove += Timeline.OnMouseMove;
            this.MouseDown += Timeline.MouseDown;
            this.MouseUp += Timeline.MouseUp;
            this.MouseClick += Timeline.MouseClick;
            this.KeyDown += Timeline.OnKeyDown;
            this.KeyUp += Timeline.OnKeyUp;
            this.KeyPress += Timeline.OnKeyPress;
            this.ResumeLayout(false);
        }

        private ConsoleLogger Logger;
        private ApplicationConfig Config;
        private ProjectDbContext Database;
        private Timeline Timeline;
        private AllTimers Timers;
        private Stopwatch Stopwatch;
        private Device? Device;
        private CharacterCollection? TextCharacters;
        private object UILock;
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
        private Drawer? Drawer;
        private bool ReInitialize;
        private bool Initialized;
        private int PhysicalWidth;
        private int PhysicalHeight;

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
        ConsoleLogger IApplication.Logger => Logger;
        ApplicationConfig IApplication.Config => Config;
        ProjectDbContext IApplication.Database => Database;
        Timeline IApplication.Timeline => Timeline;
        AllTimers IApplication.Timers => Timers;
        Stopwatch IApplication.Stopwatch => Stopwatch;
        int IApplication.Width => PhysicalWidth;
        int IApplication.Height => PhysicalHeight;
        Device IApplication.Device => Device!;
        CharacterCollection IApplication.Characters => TextCharacters!;
        Drawer IApplication.Drawer => Drawer!;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            WindowsScaling = new WindowsScaling(Handle);
            RecreateSwapChain();
        }
        protected override void OnLoad(EventArgs e)
        {
            Timeline.StartThread();

            Database.ProjectSettings.Value.TestString = "Hoi dit is een test";
        }
        protected override void OnResize(EventArgs e)
        {
            if (IsNotReadyToDraw || Device == null || WindowsScaling == null || SwapChain == null)
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
                Device.ImmediateContext.ClearState();
                RenderTargetView?.Dispose();

                int newWidth = Convert.ToInt32(Width * WindowsScaling.Scaling);
                int newHeight = Convert.ToInt32(Height * WindowsScaling.Scaling);

                if (newWidth == 0 || newHeight == 0)
                    return;

                PhysicalWidth = newWidth;
                PhysicalHeight = newHeight;

                try
                {
                    SwapChain.ResizeBuffers(1, PhysicalWidth, PhysicalHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
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
                    Device?.Dispose();
                    SamplerState?.Dispose();
                    TextCharacters?.Dispose();

                    PhysicalWidth = Convert.ToInt32(Width * WindowsScaling!.Scaling);
                    PhysicalHeight = Convert.ToInt32(Height * WindowsScaling!.Scaling);

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
                        ModeDescription = new ModeDescription(PhysicalWidth, PhysicalHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
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
                        out Device, out SwapChain);

                    Context = Device.ImmediateContext;

                    AfterResizeOrRecreate();
                    CompileShaders();

                    SamplerState = new SamplerState(Device, new SamplerStateDescription
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

                    TextCharacters = new CharacterCollection(this);
                    Drawer = new Drawer(this);

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
                RenderTargetView = new RenderTargetView(Device, backBuffer);
            }

            Context!.OutputMerger.SetRenderTargets(RenderTargetView);
            Context!.Rasterizer.SetViewport(0, 0, PhysicalWidth, PhysicalHeight);
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

            NormalVertexShader = new VertexShader(Device, normalVertexShaderByteCode);
            NormalPixelShader = new PixelShader(Device, normalPixelShaderByteCode);

            // Input layout voor normale shader
            NormalInputLayout = new InputLayout(Device,
                SharpDX.D3DCompiler.ShaderSignature.GetInputSignature(normalVertexShaderByteCode),
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 8, 0),
                });

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

            BitmapVertexShader = new VertexShader(Device, bitmapVertexShaderByteCode);
            BitmapPixelShader = new PixelShader(Device, bitmapPixelShaderByteCode);

            // Input layout voor bitmap shader
            BitmapInputLayout = new InputLayout(Device,
                SharpDX.D3DCompiler.ShaderSignature.GetInputSignature(bitmapVertexShaderByteCode),
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
                });
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
                if (IsNotReadyToDraw || Context == null || Device == null || SwapChain == null)
                    return;

                Context.ClearRenderTargetView(RenderTargetView, compiledCanvas.BackgroundColor);

                foreach (var layer in compiledCanvas.Layers)
                {
                    // Triangles
                    if (layer.FillVertices.Any())
                    {
                        VertexBuffer?.Dispose();
                        VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, layer.FillVertices.ToArray());

                        Context.InputAssembler.InputLayout = NormalInputLayout;
                        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<Vertex>(), 0));
                        Context.VertexShader.Set(NormalVertexShader);
                        Context.PixelShader.Set(NormalPixelShader);

                        Context.Draw(layer.FillVertices.Count, 0);
                    }

                    // Lines
                    if (layer.LineVertices.Any())
                    {
                        VertexBuffer?.Dispose();
                        VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, layer.LineVertices.ToArray());

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
                        VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, bitmap.Vertices);

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
            TextCharacters?.Dispose();
            VertexBuffer?.Dispose();
            NormalInputLayout?.Dispose();
            NormalVertexShader?.Dispose();
            NormalPixelShader?.Dispose();
            RenderTargetView?.Dispose();
            SwapChain?.Dispose();
            Context?.Dispose();
            Device?.Dispose();
            Database.Dispose();
        }
    }
}
