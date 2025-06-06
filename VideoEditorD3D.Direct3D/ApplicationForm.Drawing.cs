using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D
{
    public partial class ApplicationForm
    {

        public void TryDraw()
        {
            if (IsNotReadyToDraw || _Forms == null || _Forms.Count < 1)
                return;

            Draw();
        }
        private void Draw()
        {
            if (IsNotReadyToDraw || _Forms == null || _Forms.Count < 1)
                return;

            using (Timers.OnUpdateTimer.DisposableObject)
            {
                foreach (var form in _Forms)
                {
                    form.OnUpdate();
                }
            }

            using (Timers.RenderToGpuTimer.DisposableObject)
            {
                LockAndRenderToGpu();
            }

            Timers.FpsTimer.CountFps();
        }
        private void LockAndRenderToGpu()
        {
            if (IsNotReadyToDraw || _Forms == null || _Forms.Count < 1)
                return;

            try
            {
                lock (UILock)
                {
                    RenderToGpu();
                }
            }
            catch (Exception ex)
            {
                // Dit zou niet meer voor kunnen komen nu
                ApplicationContext.Logger?.WriteException(ex);

                // Checken omdat de KillSwitch inmiddels ook aangezet kan zijn
                if (IsNotReadyToDraw)
                    return;

                RecreateSwapChain();
            }
        }
        private void RenderToGpu()
        {
            if (IsNotReadyToDraw || _DeviceContext == null || _Device == null || _SwapChain == null || _Forms == null || _Forms.Count < 1)
                return;

            foreach (var form in _Forms)
            {
                foreach (var layer in form.GetAllCanvasLayers())
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
            }

            _SwapChain.Present(0, PresentFlags.None);
        }

        private void DrawImage(DeviceContext deviceContext, ITextureWithVerticies image)
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

    }
}
