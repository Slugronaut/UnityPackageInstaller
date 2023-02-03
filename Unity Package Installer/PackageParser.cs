using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Symlink_RepoClone_Installer
{
    /// <summary>
    /// Helper class with a few static methods for finding directories containing package files and parsing them.
    /// </summary>
    public static class PackageParser
    {
        public static readonly string PackageFilename = "package.json";

        /// <summary>
        /// Provide a path to a package.json file and this will parse the contents
        /// and return a Packageobject representing the key data for that package file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Package ParsePackageJson(string directoryPath, string fileName)
        {
            Package output;
            try
            {
                string txt = File.ReadAllText(Path.Combine(directoryPath, fileName));
                output = JsonSerializer.Deserialize<Package>(txt);
            }
            catch(Exception e)
            {
                throw new NotifyUserException(e.Message);
            }

            if(output == null)
                throw new NotifyUserException("Package json file failed to parse.");

            output.SrcPath = directoryPath;
            return output;
        }

        /// <summary>
        /// Supply with a root path that contains subdirectories for each package. This method will
        /// find the package.json file within each sub-directory and return a string for the path
        /// to that package.json file.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static IEnumerable<string> ScanForPackageFiles(string rootPath)
        {
            var dirs = Directory.GetDirectories(rootPath);
            foreach(var dir in dirs)
            {
                if (File.Exists(Path.Combine(dir, PackageFilename)))
                    yield return dir;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="destPath"></param>
        public static void LinkPackageToDestination(Package package, string destPath)
        {
            if (package == null) throw new NotifyUserException("Invalid package object.");
            if (string.IsNullOrEmpty(package.SrcPath)) throw new NotifyUserException($"The package '{package.name}' has an empty source path.");
            if (string.IsNullOrEmpty(destPath)) throw new NotifyUserException("A destination path must be provided.");

            string packageDir = package.SrcPath.Substring(package.SrcPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (string.IsNullOrEmpty(packageDir)) throw new Exception($"The package '{package.name}' has an invalid source path. It should not end with a '{Path.DirectorySeparatorChar}' symbol.");

            string destSubPath = Path.Combine(destPath, packageDir);

            if(package.Selected)
                Directory.CreateSymbolicLink(destSubPath, package.SrcPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="destPath"></param>
        public static void CopyPackageToDestination(Package package, string destPath)
        {
            if (package == null) throw new NotifyUserException("Invalid package object.");
            if (string.IsNullOrEmpty(package.SrcPath)) throw new NotifyUserException($"The package '{package.name}' has an empty source path.");
            if (string.IsNullOrEmpty(destPath)) throw new NotifyUserException("A destination path must be provided.");

            string packageDir = package.SrcPath.Substring(package.SrcPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (string.IsNullOrEmpty(packageDir)) throw new Exception($"The package '{package.name}' has an invalid source path. It should not end with a '{Path.DirectorySeparatorChar}' symbol.");

            string destSubPath = Path.Combine(destPath, packageDir);

            if (package.Selected)
                new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory(package.SrcPath, destSubPath);
        }
    }
}
