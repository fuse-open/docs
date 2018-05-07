# Native interop

While using UX and JavaScript is the recommended way of developing apps with Fuse, Fuse is built on native code, APIs and technologies, and allows you to extend with custom native code and features.

## Uno

Uno is a C#-like programming language that powers Fuse. All of the Fuse core classes are written in Uno, which is then compiled to C++ when running on devices. Uno shares most of its syntax and type system with C#, so learning Uno is almost identical to learning C#.

You can add Uno code to your projects in addition to your regular UX markup and JavaScript files.

Check out the [Uno Language Reference](../uno/uno-lang.md) for information on similarities and differences between Uno and C#.

### What to use Uno for

*  Wrapping new native APIs or third-party SDKs and exposing new components to UX markup or new APIs to JavaScript
* Interop with existing Java or Objective-C/Swift code
* Small pieces of code that need to be extremely high performance
* Custom GPU graphics rendering and effects

### What NOT to use Uno for

* Business logic. Use JavaScript instead. Don't worry, JavaScript runs on its own thread and cannot impair UI performance.
* Composing UX components. Use UX markup instead. Don't worry, all UX markup is converted to Uno code when compiled and is just as fast.

## Adding Uno files to a project

To add an Uno file, simply create an empty text file with the `.uno` extension in your project folder, and adding it to `Includes` array in the project file (`.unoproj`). The entry should be in the format `"<filename>:SourceFile"`.

Alternatively type `fuse create uno <filename>`. This will automatically create an uno file with a template, and add it to the project.

Unlike JavaScript, Uno code is compiled to C++ and can *not* be changed while the app is running in preview. If you make changes to an Uno file, you have to restart fuse preview for the changes to take effect.

## Uno API documentation

The Uno APIs and the UX Markup APIs are literally the same thing. We can browse the APIs by namespace in the [full class reference](../full-ux-class-reference.html).

By default the doc pages only show members that are accessible in UX markup and JavaScript. There is a "Show advanced things" checkbox which will display all the members of the class as seen from a Uno programmers point of view.

## Foreign code

Uno interops smoothly with Java, Objective-C and C++ using a superset of C# called Foreign code, to allow you to easily wrap native APIs or existing code bases.

See [the foreign code section](foreign-code.md) for more information.
