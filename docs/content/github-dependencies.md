# GitHub dependencies

Paket allows one to automatically manage the linking of files from [github.com](http://www.github.com) into your projects.

## Referencing a single file

You can reference a single file from [github.com](http://www.github.com) simply by specifying the source repository and the file name in the [`paket.dependencies` file](dependencies-file.html):

    github forki/FsUnit FsUnit.fs

If you run the [`paket update` command](paket-update.html), it will add a new section to your [`paket.lock` file](lock-file.html):

    GITHUB
      remote: forki/FsUnit
      specs:
        FsUnit.fs (7623fc13439f0e60bd05c1ed3b5f6dcb937fe468)

As you can see the file is pinned to a concrete commit. This allows you to reliably use the same file version in succeeding builds until you elect to perform a [`paket update` command](paket-update.html) at a time of your choosing.

If you want to reference the file in one of your project files then add an entry to the project's [`paket.references` file.](references-files.html):

    [lang=batchfile]
    File:FsUnit.fs

This will reference the linked file directly into your project.
By default the linked file will be visible under ``paket-files`` folder in project.

![alt text](img/github_ref_default_link.png "GitHub file referenced in project with default link")

You can specify custom folder for the file:

    [lang=batchfile]
    File:FsUnit.fs Tests\FsUnit

![alt text](img/github_ref_custom_link.png "GitHub file referenced in project with custom link")

Or if you use ``.`` for the directory, the file will be placed under the root of the project:

    [lang=batchfile]
    File:FsUnit.fs .

![alt text](img/github_ref_root.png "GitHub file referenced in project under root of project")

## Recognizing Build Action

Paket will recognize build action for referenced file based on the project type. 
As example, for a ``*.csproj`` project file, it will use ``Compile`` Build Action if you reference ``*.cs`` file 
and ``Content`` Build Action if you reference file with any other extension.

## Remote dependencies

If the remote file needs further dependencies then you can just put a [`paket.dependencies` file.](dependencies-file.html) into the same GitHub repo folder.
Let's look at a sample:

![alt text](img/octokit-module.png "Octokit module")

And we reference this in our own [`paket.dependencies` file.](dependencies-file.html):

    github fsharp/FAKE modules/Octokit/Octokit.fsx


This generates the following [`paket.lock` file](lock-file.html):

	NUGET
	  remote: http://nuget.org/api/v2
	  specs:
		Microsoft.Bcl (1.1.9)
		  Microsoft.Bcl.Build (>= 1.0.14)
		Microsoft.Bcl.Build (1.0.21)
		Microsoft.Net.Http (2.2.28)
		  Microsoft.Bcl (>= 1.1.9)
		  Microsoft.Bcl.Build (>= 1.0.14)
		Octokit (0.4.1)
		  Microsoft.Net.Http (>= 0)
	GITHUB
	  remote: fsharp/FAKE
	  specs:
		modules/Octokit/Octokit.fsx (a25c2f256a99242c1106b5a3478aae6bb68c7a93)
		  Octokit (>= 0)

As you can see Paket also resolved the Octokit dependency.