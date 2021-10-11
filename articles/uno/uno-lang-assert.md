# assert

The assert keyword checks a condition and emits an error message when it is false.

```csharp
var a = 1;
assert a>5;
```

Results in the runtime message:

```s
Assertion Failed: 'a > 5' in /Path/MyApplication/App.uno(14)
```

A failed condition does not stop program flow, it only records an error message in the debug output.

## Assertion Handler

If you wish to intercept assertion statements, you can install an assertion handler. Use SetAssertionHandler in Uno.Diagnostics.Debug.

```csharp
Debug.SetAssertionHandler( MyAssert );
```

This handler records a message and raises an exception:

```csharp
void MyAssert(bool value, string expression, string filename, int line, params object[] operands)
{
    if( value )
        return;

    var msg = "Assertion failed: " + filename + ":" + line + ": " + expression;
    debug_log msg;
    throw new Exception( msg );
}
```