using Ookii.Dialogs.Wpf;
using System;
using System.Windows;

namespace Symlink_RepoClone_Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum CopyMethods
        {
            SymLink,
            Copy,
        }

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
                        if (comboBoxCopyMethod.SelectedIndex == (int)CopyMethods.SymLink)
                            PackageParser.LinkPackageToDestination(package, diag.SelectedPath);
                        else if (comboBoxCopyMethod.SelectedIndex == (int)CopyMethods.Copy)
                            PackageParser.CopyPackageToDestination(package, diag.SelectedPath);
                        else throw new NotifyUserException("Unknown copy method detected.");

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

            MessageBox.Show($"Package installation complete.\n\nTotal packages installed is {counter}.");
        }

        /// <summary>
        /// Invoked whenever the user checks a box on a package row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPackageSelectedChecked(object sender, RoutedEventArgs e)
        {
            //TODO:
            //-check for dependencies
            //-show warning if any found
            //-check them as well if user agrees (avoid recursive messages to this handler)

        }

        /// <summary>
        /// Invoked whenever the user unchecks a box on a package row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPackageSelectedUnchecked(object sender, RoutedEventArgs e)
        {
            //TODO:
            //-check to see if anything depends on this
            //-display warning if they do
            //-uncheck those packages as well if the user decides to continue (avoid recursive messages to this handler)
        }
    }


    




}
