using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    internal class NoopHardwareEditModel : IHardwareEdit
    {
        private readonly static ShaderModel _shaderModel;

        static NoopHardwareEditModel()
        {
            //_shaderModel = new ShaderModel(
            //    new Uri("pack://application:,,,/Shaders/Base.vert", UriKind.Relative),
            //    new Uri("pack://application:,,,/Shaders/Base.frag", UriKind.Relative),
            //    out bool success);

            _shaderModel = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }
        }

        void IHardwareEdit.Apply(TextureModel source, FrameBufferModel destination, VertexArrayModel vertexArray)
        {
            destination.Use(FramebufferTarget.Framebuffer);
            source.Use(TextureUnit.Texture0);
            _shaderModel.Use();
            vertexArray.Use();
            GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        void IDisposable.Dispose()
        {
            // static shader will live during the programs runtime.
        }
    }
}
