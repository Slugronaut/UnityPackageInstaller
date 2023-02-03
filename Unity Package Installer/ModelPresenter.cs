using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Symlink_RepoClone_Installer
{

    /// <summary>
    /// 
    /// </summary>
    public class ModelPresenter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        readonly Model Model = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        public void PropertyUpdated(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public string SrcPath
        {
            get => Model.SrcPath;
            set
            {
                Model.SrcPath = value;
                PropertyUpdated(nameof(SrcPath));
            }
        }

        public ObservableCollection<Package> Packages
        {
            get => Model.Packages;
            set
            {
                Model.Packages = value;
                PropertyUpdated(nameof(Packages));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        public void ScanForPackages()
        {
            try
            {
                Model.ScanForPackages();
                PropertyUpdated(nameof(Packages));
            }
            catch (NotifyUserException e)
            {
                MessageBox.Show(e.Message);
            }

        }


    }
}
