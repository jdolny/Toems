using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common
{
    public static class PathUtils
    {
        public static string CombineSafe(string rootDirectory, params string[] paths)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ArgumentException(nameof(rootDirectory));

            var rootFullPath = Path.GetFullPath(rootDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            foreach (var p in paths)
            {
                //verify the path to combine, is not a root
                if (Path.IsPathRooted(p))
                    throw new UnauthorizedAccessException("Rooted paths are not allowed.");
                EnsureNoReparsePoints(rootFullPath + p);
            }

            var combinedPath = Path.Combine(paths);
            var fullPath = Path.GetFullPath(Path.Combine(rootFullPath, combinedPath));

            if (!fullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Path traversal detected.");

            return fullPath;
        }

        public static void ValidateFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Filename is required");

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                name.Contains(Path.DirectorySeparatorChar) ||
                name.Contains(Path.AltDirectorySeparatorChar))
            {
                throw new ArgumentException("Invalid file name");
            }

            if (name.EndsWith(" ") || name.EndsWith("."))
                throw new ArgumentException("Invalid filename format");
        }

        public static void ValidatePathIsUnderRoot(string rootPath, string filePath)
        {

            var rootFull = Path.GetFullPath(rootPath)
                               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                           + Path.DirectorySeparatorChar;

            var fileFull = Path.GetFullPath(filePath);

            if (!fileFull.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Invalid path");
        }

        public static void EnsureNoReparsePoints(string path)
        {
            var dir = new DirectoryInfo(path);

            while (dir != null && dir.Exists)
            {
                if (dir.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    throw new UnauthorizedAccessException("Path contains reparse point");

                dir = dir.Parent;
            }
        }

        public static string Quote(string value)
        {
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }
    }
}
