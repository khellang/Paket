# paket add

Adds a new package to your [`paket.dependencies` file](dependencies-file.html) and runs the [update process](paket-update.html).

    [lang=batchfile]
    $ paket add nuget PACKAGENAME [version VERSION] [--interactive] [--force] [--hard] [--dependencies-file FILE]

Options:

  `--interactive`: Asks the user for every project if he or she wants to add the package to the projects's [`paket.references` file](references-file.html).

  `--force`: Forces the download and reinstallation of all packages.

  `--hard`: Replaces package references within project files even if they are not yet adhering to to Paket's conventions (and hence considered manually managed). See [convert from NuGet](convert-from-nuget.html).

  `--dependencies-file`: Use the specified file instead of [`paket.dependencies`](dependencies-file.html).

## Sample

Consider the following [`paket.dependencies` file](dependencies-file.html):

	source http://nuget.org/api/v2

	nuget FAKE

Now we run `paket add nuget xunit --interactive` install the package:

![alt text](img/interactive-add.png "Interactive paket add")

This will add the package to the selected [`paket.references` files](references-file.html) and also to the [`paket.dependencies` file](dependencies-file.html):

	source http://nuget.org/api/v2

	nuget FAKE
	nuget xunit