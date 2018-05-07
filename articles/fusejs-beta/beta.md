## FuseJS Beta

This is the first official release that ships with new experimental FuseJS features that have been out in beta for some time. **Please keep in mind that these features are still experimental, and not yet ready for production use.** That being said, we still encourage you to try it out, and leave feedback if you encounter any issues.

- [Read the documentation](./fusejs.md)
- [To-do app example](https://github.com/Duckers/FuseJS-TodoApp)

## Prerequisites

- [Node.js + NPM](https://nodejs.org/en/) is required for ES6 transpiling.

## Known issues with this release

- Passing objects between models (via `ux:Property`) is known to have issues with change detection. For now, be careful not to mutate any object passed from one `Model` to another.
- Line numbers in stack traces correspond to the _transpiled_ ES5 code that is actually running in your app, not your ES6 source code.
- Properties with a leading underscore (`_myProperty`, for example) are considered private. However, this feature is not yet fully implemented. Avoid referencing private properties from computed getters.
- `setTimeout` is known to cause unexpected behavior. We recommend using [FuseJS/Timer](api:fuse/reactive/fusejs/timermodule) for the time being.
- Plain Observables have not been thoroughly tested together with models. Use at your own discretion.
- In preview, removing or changing the value of the `Model` attribute might not have an effect until rebuild.
- Most, but not all asynchronous APIs are intercepted to trigger change detection.
