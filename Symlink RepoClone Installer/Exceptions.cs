using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symlink_RepoClone_Installer
{

    /// <summary>
    /// 
    /// </summary>
    public class NotifyUserException : Exception
    {
        public NotifyUserException(string message) : base(message) { }
    }
}
