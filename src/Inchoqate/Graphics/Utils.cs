using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.Graphics
{
    public static class Utils
    {
        /// <summary>
        /// Checks and logs GL errors.
        /// </summary>
        /// <returns>True if there was any error.</returns>
        internal static bool CheckErrors(this ILogger logger)
        {
            var result = false;
            while (true)
            {
                var error = GL.GetError();
                if (error == ErrorCode.NoError) break;
                result = true;
                logger.LogError("Graphics Error: {err}", error);
            }

            return result;
        }

        internal static bool CheckErrors(this ILogger logger, string message)
        {
            var result = CheckErrors(logger);
            if (result) logger.LogError(message);
            return result;
        }
    }
}