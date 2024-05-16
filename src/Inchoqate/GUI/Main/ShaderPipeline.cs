using Inchoqate.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqater.GUI.Main
{
    public class ShaderPipeline
    {
        private List<Shader> _shaders = [];



        private void SetupVertexAttribs(Shader shader)
        {
            int aPositionLoc = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.EnableVertexAttribArray(aPositionLoc);
            GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int aTexCoordLoc = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.EnableVertexAttribArray(aTexCoordLoc);
            GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }
    }
}
