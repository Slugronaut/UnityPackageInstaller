# UnityPackageInstaller
A tool for installing Unity Custom Packages to project workspaces via Symlinks. Thus allowing for an easier workflow using a single source repo.

# Details
Custom Packages for Unity are pretty cool. They allow you to develop and share libraries and systems that can easily be imported into other projects. However, there is one huge problem that mars this system. The built-in package manager of Unity's isn't capable of easily installing these custom packages nor resolving their dependencies.

This tool was developed as a quick and dirty workaround for the first issue. Rather than having to re-clone/download packages and manually installing them into each project directory you are using, you can instead clone them all to a centralized location on your local machine. This app can then be used to scan the root directory containing all of these cloned package directories and provide a simple list where you can choose what to install. After that it is simply a matter of selecting the destination directory for project you want to 'install' these packages to. The app will create symbolic links to the project destination for each package selected.

This app was developed for WPF using .NET 7

# Future
Eventually I'd like to also provide a means of handling dependencies but that will for a later time.


# Dependencies:  
[Ookii.Dialogs](https://www.ookii.org/software/dialogs/)  
