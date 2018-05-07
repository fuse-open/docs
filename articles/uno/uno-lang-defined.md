# defined

The defined operator can be used to write code that compiles or behaves differently based on target platform.

## Syntax

```
defined(variable)
```

Where variable is one of:

* Debug
* WebGL
* Android
* Designer
* iOS
* OpenGL
* CIL
* CSharp
* CPlusPlus
* JavaScript

The defined operator returns a boolean compile-time constant, which means that the compiler may eliminate platform-irrelevant dead code if the expression is used as a conditional.

```csharp
Example
if (defined(iOS))
{
    button.Text = "I love my iOS device";
}
else if (defined(Android))
{
    button.Text = "I wish i had an iOS device";
}
```