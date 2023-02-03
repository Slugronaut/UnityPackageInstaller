using System;
using System.Collections.Generic;
using System.IO;

namespace Symlink_RepoClone_Installer
{
    /// <summary>
    /// 
    /// </summary>
    public class Package
    {
        #region Json Data
        public string? name { get; set; }
        public string? displayName { get; set; }
        public string? description { get; set; }
        public Dictionary<string, string>? dependencies { get; set; }
        #endregion


        #region Meta Data
        public string SrcPath { get; set; }
        public bool Selected { get; set; } = true;
        #endregion

    }
}
