using System.IO;

namespace PICS
{
    public static class PathUtility
    {
        public static string GetAbsolutePath(string path)
        {
            return GetAbsolutePath(null, path);
        }

        public static string GetAbsolutePath(string basePath, string path)
        {
            if (path == null)
            {
                return null;
            }

            if (basePath == null)
            {
                basePath = Path.GetFullPath("."); // quick way of getting current working directory
            }
            else
            {
                basePath = GetAbsolutePath(null, basePath); // to be REALLY sure ;)
            }

            string finalPath;
            // specific for windows paths starting on \ - they need the drive added to them.
            // I constructed this piece like this for possible Mono support.
            if (!Path.IsPathRooted(path) || "\\".Equals(Path.GetPathRoot(path)))
            {
                if (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    finalPath = Path.Combine(Path.GetPathRoot(basePath), path.TrimStart(Path.DirectorySeparatorChar));
                }
                else
                {
                    finalPath = Path.Combine(basePath, path);
                }
            }
            else
            {
                finalPath = path;
            }
            // resolves any internal "..\" to get the true full path.
            return Path.GetFullPath(finalPath);
        }

        public static void CreateDirectoryIfDoesNotExist(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                _ = Directory.CreateDirectory(dirPath);
            }
        }
    }
}
