//using SharpDX;
//using SharpDX.Direct3D11;
//using SharpDX.D3DCompiler;
//using SharpDX.Mathematics.Interop;
//using System.Runtime.InteropServices;
//using SharpDX.DXGI;
//using SharpDX.Direct2D1;
//using Buffer = SharpDX.Direct3D11.Buffer;
//using Device = SharpDX.Direct3D11.Device;
//using Filter = SharpDX.Direct3D11.Filter;
//using InputElement = SharpDX.Direct3D11.InputElement;

//namespace VideoEditor.UI;

//public class DisplayControlDX3D : Control
//{
//    private Device device;
//    private SwapChain swapChain;
//    private RenderTargetView renderTargetView;
//    private ShaderBytecode pixelShaderBytecode;
//    private PixelShader pixelShader;
//    private SamplerState samplerState;
//    private Buffer saturationBuffer;
//    private InputLayout inputLayout;
//    private Texture2D texture;
//    private ShaderResourceView textureView;
//    private Effect effect;

//    private float saturation = 1.0f; // Default saturation value

//    public void SetFrame(byte[] frameBuffer, int width, int height, float saturation)
//    {
//        this.saturation = saturation;
//        UpdateTexture(frameBuffer, width, height);
//        Render();
//    }

//    private void UpdateTexture(byte[] frameBuffer, int width, int height)
//    {
//        var textureDesc = new Texture2DDescription
//        {
//            Width = width,
//            Height = height,
//            MipLevels = 1,
//            ArraySize = 1,
//            Format = Format.B8G8R8A8_UNorm,
//            Usage = ResourceUsage.Default,
//            BindFlags = BindFlags.ShaderResource,
//            CpuAccessFlags = CpuAccessFlags.None,
//            OptionFlags = ResourceOptionFlags.None
//        };

//        var dataStream = DataStream.Create(frameBuffer, true, false);
//        var textureData = new DataRectangle(dataStream.DataPointer, width * 4);
//        //var textureData = new DataRectangle(Marshal.UnsafeAddrOfPinnedArrayElement(frameBuffer, 0), width * 4);

//        texture?.Dispose();
//        textureView?.Dispose();
//        texture = new Texture2D(device, textureDesc, new[] { textureData });
//        textureView = new ShaderResourceView(device, texture);
//    }

//    private void Render()
//    {
//        // Clear the back buffer
//        device.ImmediateContext.ClearRenderTargetView(renderTargetView, new RawColor4(0, 0, 0, 1));

//        // Set up the pipeline for rendering
//        device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
//        device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(null, 0, 0));
//        device.ImmediateContext.PixelShader.Set(pixelShader);
//        device.ImmediateContext.PixelShader.SetShaderResource(0, textureView);
//        device.ImmediateContext.PixelShader.SetConstantBuffer(0, saturationBuffer);

//        // Apply the saturation effect via constant buffer
//        device.ImmediateContext.UpdateSubresource(ref saturation, saturationBuffer);

//        // Render the scene (this will draw the texture with the saturation effect applied)
//        swapChain.Present(0, PresentFlags.None);
//    }

//    protected override void OnHandleCreated(EventArgs e)
//    {
//        base.OnHandleCreated(e);

//        // Initialize Direct3D Device, SwapChain, and RenderTarget here
//        InitializeDirect3D();
//    }

//    private void InitializeDirect3D()
//    {
//        var swapChainDescription = new SwapChainDescription()
//        {
//            BufferCount = 1,
//            ModeDescription = new ModeDescription(Width, Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
//            IsWindowed = true,
//            OutputHandle = Handle,
//            SampleDescription = new SampleDescription(1, 0),
//            Usage = Usage.RenderTargetOutput
//        };

//        Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.None, swapChainDescription, out device, out var swapChain);

//        renderTargetView = new RenderTargetView(device, swapChain.GetBackBuffer<Texture2D>(0));

//        // Load shaders and create input layout
//        LoadShaders();
//    }
//    private void LoadShaders()
//    {
//        // Compile en laad de vertex shader
//        var vertexShaderBytecode = ShaderBytecode.CompileFromFile("SaturationShader.hlsl", "VSMain", "vs_5_0");
//        var vertexShader = new VertexShader(device, vertexShaderBytecode);

//        // Compile en laad de pixel shader
//        pixelShaderBytecode = ShaderBytecode.CompileFromFile("SaturationShader.hlsl", "PSMain", "ps_5_0");
//        pixelShader = new PixelShader(device, pixelShaderBytecode);

//        // Definieer de vertex input layout
//        inputLayout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderBytecode), new[]
//        {
//            new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
//            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
//        });

//        // Maak een sampler state
//        samplerState = new SamplerState(device, new SamplerStateDescription()
//        {
//            Filter = Filter.MinMagMipPoint,
//            AddressU = TextureAddressMode.Wrap,
//            AddressV = TextureAddressMode.Wrap,
//            AddressW = TextureAddressMode.Wrap
//        });

//        // Maak de constant buffer aan
//        saturationBuffer = new Buffer(device, Utilities.SizeOf<float>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 4);
//    }

//    protected override void Dispose(bool disposing)
//    {
//        if (disposing)
//        {
//            renderTargetView?.Dispose();
//            textureView?.Dispose();
//            texture?.Dispose();
//            samplerState?.Dispose();
//            pixelShader?.Dispose();
//            pixelShaderBytecode?.Dispose();
//            saturationBuffer?.Dispose();
//            inputLayout?.Dispose();
//            device?.Dispose();
//            swapChain?.Dispose();
//        }
//        base.Dispose(disposing);
//    }

//}
