# Introduction 
This console app is a workaround for the GitVersionTask dotnet core cli issue. Its comments out or uncomments the GitVersionTask Package- and CommonTargets references in the csproj file.

# Getting Started
You need the following params to run the app:

To uncomment the GitVersionTask entries use this:
SwitchGitVersionTaskPackage.exe enable [PATH_TO_YOUR_SLN_FILE_FOLDER]

To comment out all the GitVersionTask entries use this:
SwitchGitVersionTaskPackage.exe enable [PATH_TO_YOUR_SLN_FILE_FOLDER]