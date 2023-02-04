# UnityPackageInstaller
A tool for installing Unity Custom Packages to project workspaces via Symlinks. Thus allowing for an easier workflow using a single source repo.

# Details
Custom Packages for Unity are pretty cool. They allow you to develop and share libraries and systems that can easily be imported into other projects. However, there is one huge problem that mars this system. The built-in package manager of Unity's isn't capable of easily installing these custom packages nor resolving their dependencies.

This tool was developed as a quick and dirty workaround for the first issue. Rather than having to re-clone/download packages and manually installing them into each project directory you are using, you can instead clone them all to a centralized location on your local machine. This app can then be used to scan the root directory containing all of these cloned package directories and provide a simple list where you can choose what to install. After that it is simply a matter of selecting the destination directory for project you want to 'install' these packages to. The app will create symbolic links to the project destination for each package selected.

This app was developed for WPF using .NET 7

# Unity Warnings
Unity does not recommend the use of Symlinked files and folders. And for good reason. If you are careful and know exactly what you are doing it will work. But if you are not sure then there is an option to simply copy the files rather than create symlinks.

# Future
Dependencies are now supported when they are included in the list of packages being scanned. However, there is currently now way to resolve external dependencies either on the local machine or online. If I can think of a good way to do this I'll implement it. It will likely require extra package info.

I'd also like to create a package collections system whereby you can provide a set of packages under a single entry that can be installed or uninstalled all in a single click.


# Dependencies:  
[Ookii.Dialogs](https://www.ookii.org/software/dialogs/)  
