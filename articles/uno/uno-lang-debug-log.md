# debug_log

debug_log can be used to write messages to the debug output. In preview this output is captured in the Output tab.

For example:

```csharp
debug_log "Hello";
debug_log somObject;
```

debug_log is a shortcut to calling the Log functions from Uno.Diagnostics.Debug.
