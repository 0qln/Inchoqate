using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class GraphicsModel
    {
        private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<GraphicsModel>();

        /// <summary>
        /// Checks and logs GL errors.
        /// </summary>
        /// <returns>True if there was any error.</returns>
        public static bool CheckErrors()
        {
            var result = false;
            while (true)
            {
                var error = GL.GetError();
                if (error == ErrorCode.NoError) break;
                result = true;
                Logger.LogError("Graphics Error: {err}", error);
            }

            return result;
        }

        public static bool CheckErrors(string message, ILogger logger)
        {
            var result = CheckErrors();
            if (result) logger.LogError(message);
            return result;
        }
    }
}
