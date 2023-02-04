using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        /// <summary>
        /// Returns an enumerable list of all packages the given package is dependent on.
        /// It can only return dependencies that are currently present in the model.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public IEnumerable<Package> DependenciesOf(Package package)
        {
            foreach(var kvp in package.dependencies)
            {
                foreach(var checkPack in Packages)
                {
                    if (checkPack.name == kvp.Key)
                        yield return checkPack;
                }
            }
        }

        /// <summary>
        /// Returns an enumerablelist of all packages that are dependent on the given package.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public IEnumerable<Package> DependentOn(Package package)
        {
            foreach(var checkPack in Packages)
            {
                if(checkPack.dependencies.ContainsKey(package.name))
                    yield return checkPack;
            }
        }
    }
}
