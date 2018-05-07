# import

The import keyword is used to import resources that might require compile-time conditioning, such as models, textures, fonts and sounds.

Using the import keyword instead of loading the resource directly from a file at runtime allows the compiler to prepare the resource for best possible compatibility, performance and memory footprint for each target platform. Through the import keyword, the compiler can for example apply texture compression, build font atlases and convert resource files into formats that the target platform supports.

Files processed by the import keyword are placed in the AppBundle. This means that for example when exporting for mobile OSes, the files will be embedded in the native download bundle format for apps.

It is recommended to use import instead of loading directly from files whenever you can, which is whenever the data file is available compile-time. Note that this is the only way to ensure that a file will be supported, load and behave correctly and optimally on all target platforms!

## Syntax

An import expression has the following syntax:

```csharp
import importerClass( [ importerArguments ... ] )
```

Where importerClass is the name of a class which derives from Uno.Compiler.ImportServices.Importer, and has a name that ends in 'Importer'. The 'Importer' suffix must be ommited from the importerClass.

The importer typically takes one or more arguments. All importer arguments must be compile-time constants.

## Technical behavior

When the compiler encounters an import expression, the specified importer class is invoked at compile time with the privided arguments. The returned object is serialized and stored in the AppBundle (see Uno.Platform.AppBundle). The import expression is then replaced with a corresponding call to the AppBundleRegistry class, which will retrieve the object at runtime.

If multiple import expressions are encountered in a project with exact same importer class and arguments, they will return the exact same object reference in all cases.

## Examples

There are multiple importer classes in the Uno standard library, such as:

```csharp
Uno.Graphics.Texture2DImporter
Uno.BundleFileImporter
Uno.Content.Models.ModelImporter
These can be used as follows:

using Uno.Graphics;
using Uno.Content.Models;

...

var tex = import Texture2D("image.png");
var model = import Model("model.fbx");
```

Note that the 'Importer' suffix to the class names should not be used in import expressions. The compiler will assume the referred class has this suffix.