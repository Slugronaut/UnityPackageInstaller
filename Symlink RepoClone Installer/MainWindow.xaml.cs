using System;
using System.IO;
using System.Windows;

namespace Symlink_RepoClone_Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ModelView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFolderSelectButton(object sender, RoutedEventArgs args)
        {
            var diag = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog { Multiselect = false };
            if (diag.ShowDialog(this) == true)
            {
                ModelView.SrcPath = diag.SelectedPath;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickScanButton(object sender, RoutedEventArgs args)
        {
            ModelView.ScanForPackages();
        }

        /// <summary>
        /// Opens the folder dialog to get destination. Creates symbolic links for all packages
        /// in the datagrid that are selected in this destination folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickLinkButton(object sender, RoutedEventArgs args)
        {
            int counter = 0;
            var diag = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog { Multiselect = false };
            if (diag.ShowDialog(this) == true)
            {
                try
                {
                    foreach (var package in ModelView.Packages)
                    {
                        PackageParser.LinkPackageToDestination(package, diag.SelectedPath);
                        counter++;
                    }

                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }


            MessageBox.Show($"Package linking complete. Total packages linked is {counter}.");
        }

    }


    




}
