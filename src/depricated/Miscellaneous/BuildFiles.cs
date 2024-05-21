using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;

namespace Inchoqate.Miscellaneous
{
    // TODO: Is there a standard way to do this?

    /// <summary>
    /// A helper class to get files from the solution structure over to the deployment location, such 
    /// that the resources can be accessed, whichever way to exe was booted up (root directory, cw directory, etc...).
    /// </summary>
    public class BuildFiles
    {
        private static readonly ILogger<BuildFiles> _logger = FileLoggerFactory.CreateLogger<BuildFiles>();

        private static readonly string AppDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
        
        private static readonly string[] SourceDirs = [
            Path.GetFullPath(Path.Combine(AppDir, "../../../Shaders/")),
            Path.GetFullPath(Path.Combine(AppDir, "../../../Themes/")),
            Path.GetFullPath(Path.Combine(AppDir, "../../../Main/")),
            //Path.GetFullPath(Path.Combine(AppDir, "../../../Main/Editor/FCN_Grayscale.xaml")),
            Path.GetFullPath(Path.Combine(AppDir, "../../../../Sample-Images/"))
        ];

        private static readonly string TargetDir = Path.Combine(AppDir, "Files/");


        public static bool Initiate(bool clearOldData = true)
        {
            // 1. Remove any old data, if requested.
            if (clearOldData && Directory.Exists(TargetDir))
                Directory.Delete(TargetDir, true);

            // 2. Create directory at application path.
            if (!Directory.Exists(TargetDir))
                Directory.CreateDirectory(TargetDir);

            // 3. Copy all files from the presumed directory to the target directory.
            foreach (var dir in SourceDirs)
            {
                try
                {
                    // Make sure the folder has the same name as the source.
                    var sourceName = Directory.GetParent(dir)?.Name;
                    var target = Path.Combine(TargetDir, sourceName!);
                    CopyAll(dir, target);
                    _logger.LogInformation($"Copied source directory {dir} to {target}.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Could not Copy testing data from \n\t{dir} to \n\t{TargetDir}");
                    return false;
                }
            }
            return true;
        }


        private static void CopyAll(string sourceDir, string targetDir)
        {
            // Check if the target directory exists, if not, create it.
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Copy all files in the source directory.
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destPath = Path.Combine(targetDir, fileName);
                File.Copy(filePath, destPath, true);
            }

            // Recursively copy all subdirectories.
            foreach (string subDirPath in Directory.GetDirectories(sourceDir))
            {
                string subDirName = Path.GetFileName(subDirPath);
                string destSubDirPath = Path.Combine(targetDir, subDirName);
                CopyAll(subDirPath, destSubDirPath);
            }
        }


        public static string Get(string data)
        {
            return Path.Combine(TargetDir, data);
        }
    }
}
