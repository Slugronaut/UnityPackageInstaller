using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

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
                ModelView.ScanForPackages();
            }
            
        }

        /// <summary>
        /// Opens the folder dialog to get destination. Creates symbolic links for all packages
        /// in the datagrid that are selected in this destination folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickLinkButton(object sender, RoutedEventArgs args)
        {
            if(string.IsNullOrEmpty(ModelView.SrcPath))
            {
                MessageBox.Show("Please select a source directory to scan for packages.");
                return;
            }

            if(ModelView.Packages.Count < 1)
            {
                MessageBox.Show("There are no packages within the folder you have selected.");
                return;
            }

            int counter = 0;
            var diag = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog { Multiselect = false };
            if (diag.ShowDialog(this) == true)
            {
                bool skipAll = false;
                foreach (var package in ModelView.Packages)
                {
                    try
                    {
                        PackageParser.LinkPackageToDestination(package, diag.SelectedPath);
                        counter++;
                    }
                    catch (Exception e)
                    {
                        if (!skipAll)
                        {
                            //display a message about the problem and ask user if they want to continue.
                            using (var taskDiag = new Ookii.Dialogs.Wpf.TaskDialog())
                            {
                                var cancelButton = new TaskDialogButton("Cancel");
                                var skipButton = new TaskDialogButton("Don't Show Errors");
                                var continueButton = new TaskDialogButton("Continue");

                                taskDiag.Content = $"There was an error processing the package '{package.name}'.\n\n{e.Message}";
                                taskDiag.Buttons.Add(cancelButton);
                                taskDiag.Buttons.Add(skipButton);
                                taskDiag.Buttons.Add(continueButton);

                                var result = taskDiag.ShowDialog(this);
                                if (result == cancelButton)
                                {
                                    break;
                                }
                                else if (result == skipButton)
                                {
                                    skipAll = true;
                                }
                                else if (result == continueButton)
                                {
                                    continue;
                                }
                            }
                        }
                    }//end catch
                }//end foreach
            }

            MessageBox.Show($"Package linking complete. Total packages linked is {counter}.");
        }

    }


    




}
