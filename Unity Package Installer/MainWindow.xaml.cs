using Ookii.Dialogs.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Serialization;

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
        private void OnClickInstallButton(object sender, RoutedEventArgs args)
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
                        bool result;
                        if (comboBoxCopyMethod.SelectedIndex == (int)CopyMethods.SymLink)
                            result = PackageUtility.LinkPackageToDestination(package, diag.SelectedPath);
                        else if (comboBoxCopyMethod.SelectedIndex == (int)CopyMethods.Copy)
                            result = PackageUtility.CopyPackageToDestination(package, diag.SelectedPath);
                        else throw new NotifyUserException("Unknown copy method detected.");

                        if(result)
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
        private void OnPackageCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as System.Windows.Controls.CheckBox;
            int index = dataGrid.SelectedIndex;
            if (checkBox == null || index < 0 || index > ModelView.Packages.Count)
                return;
            var selectedPackage = ModelView.Packages[index];

            if (checkBox.IsChecked == true)
            {
                //CHECKED
                //-check for dependencies of this package
                //-show warning if any found
                //-check them as well if user agrees (avoid recursive messages to this handler)

                var deps = ModelView.DependenciesOf(selectedPackage).ToList();
                if(deps != null && deps.Count > 0 && deps.Any(x => !x.Selected))
                {
                    using(var diag = new Ookii.Dialogs.Wpf.TaskDialog())
                    {
                        var cancelButton = new TaskDialogButton("Cancel");
                        var selectAllButton = new TaskDialogButton("Install All");
                        var selectOneButton = new TaskDialogButton(("Just this One"));

                        diag.Buttons.Add(cancelButton);
                        diag.Buttons.Add(selectAllButton);
                        diag.Buttons.Add(selectOneButton);
                        diag.Content = $"The package '{selectedPackage.name}' has one or more dependencies. Do you want to install all of them as well?";
                        var result = diag.ShowDialog(this);
                        if(result == cancelButton)
                        {
                            checkBox.IsChecked = false;
                            e.Handled = true;
                            return;
                        }
                        else if(result == selectAllButton)
                        {
                            foreach(var dep in deps)
                                dep.Selected = true;
                            dataGrid.Items.Refresh();
                        }
                    }
                }
            }
            else
            {
                //UNCHECKED
                //-check to see if anything depends on this
                //-display warning if they do
                //-uncheck those packages as well if the user decides to continue (avoid recursive messages to this handler)

                var deps = ModelView.DependentOn(ModelView.Packages[index]).ToList();
                if (deps != null && deps.Count > 0 && deps.Any(x => x.Selected))
                {
                    using (var diag = new Ookii.Dialogs.Wpf.TaskDialog())
                    {
                        var cancelButton = new TaskDialogButton("Cancel");
                        var selectAllButton = new TaskDialogButton("Un-innstall All");
                        var selectOneButton = new TaskDialogButton(("Just this One"));

                        diag.Buttons.Add(cancelButton);
                        diag.Buttons.Add(selectAllButton);
                        diag.Buttons.Add(selectOneButton);
                        diag.Content = $"The package '{selectedPackage.name}' has one or more dependencies. Do you want to un-install all of them as well?";
                        var result = diag.ShowDialog(this);
                        if (result == cancelButton)
                        {
                            checkBox.IsChecked = true;
                            e.Handled = true;
                            return;
                        }
                        else if (result == selectAllButton)
                        {
                            foreach (var dep in deps)
                                dep.Selected = false;
                            dataGrid.Items.Refresh();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Enumerates all checkboxes associated with each package.
        /// </summary>
        /// <param name="packages"></param>
        /// <returns></returns>
        IEnumerable<CheckBox> PackageCheckboxes(IEnumerable<Package> packages)
        {
            var rows = dataGrid.Items.OfType<CheckBox>();
            foreach (var package in packages)
            {
                foreach(var row in rows) 
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Invoked whenever the user unchecks a box on a package row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPackageSelectedUnchecked(object sender, RoutedEventArgs e)
        {
            //TODO:
        }
    }


    




}
