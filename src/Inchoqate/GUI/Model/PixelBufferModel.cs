using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class PixelBufferModel : IDisposable, IEditSourceModel, IEditDestinationModel
    {
        private bool disposedValue;

        public byte[] Data { get; private set; }
        public readonly int Width, Height;


        public PixelBufferModel(int size, int width, int height)
        {
            Data = new byte[size];
            Width = width; 
            Height = height;
        }

        public static PixelBufferModel? FromGpu(FrameBufferModel buffer)
        {
            PixelBufferModel result = new(buffer.Data.Stride * buffer.Data.Height, buffer.Data.Width, buffer.Data.Height);
            GL.ReadPixels(0, 0, buffer.Data.Width, buffer.Data.Height, TextureModel.GLPixelFormat, TextureModel.GLPixelType, result.Data);
            return result;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Data = null!;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
