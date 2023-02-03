using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Symlink_RepoClone_Installer
{
    /// <summary>
    /// 
    /// </summary>
    public class Model
    {
        public string SrcPath { get; set; } = string.Empty;
        public ObservableCollection<Package> Packages { get; set; } = new();


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotifyUserException"></exception>
        public void ScanForPackages()
        {
            Packages.Clear();
            if (string.IsNullOrEmpty(SrcPath))
                throw new NotifyUserException("Please select a directory to scan for packages.");

            foreach (var dir in PackageUtility.ScanForPackageFiles(SrcPath))
                Packages.Add(PackageUtility.ParsePackageJson(dir, PackageUtility.PackageFilename));


        }
    }
}
