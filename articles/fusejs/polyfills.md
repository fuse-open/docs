# Polyfills

FuseJS executes in a (minimal) EcmaScript 5.1 environment on all supported platforms. Because there is no web browser involved, FuseJS provides polyfills for some browser functionality required in order to make third party libraries work. These implementations are not necessarily complete at this point, so please report any bugs and feature requests in the [forum](https://forums.fusetools.com/). If you don't want these polyfills injected by default, contact us and we will help you change your project file.

## Currently supported polyfills

 * `fetch` - The preferred way of doing HTTP requests ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API/Using_Fetch) )
 * `XMLHttpRequest` - API providing more functionality on data transfer to and from a server ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) )
 * `Promise` - Very common concept in async JavaScript programming. We conform to the [A+ promise standard](https://promisesaplus.com/)
 * `setTimeout` / `clearTimeout` - Schedules a function to be run in the future ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/WindowTimers/setTimeout) )
 * `setInterval` / `clearInterval` - Schedules a function to be run in intervals ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/WindowTimers/setInterval) )
 * `localStorage` - Perisistent local storage ( [MDN docs](https://developer.mozilla.org/en/docs/Web/API/Window/localStorage) )
 * `atob` / `btoa` - Encodes data to and from base64 ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/WindowBase64/atob) )
 * `FileReader` - Allows JavaScript to load the contents of files ( [MDN docs](https://developer.mozilla.org/en/docs/Web/API/FileReader) )
 * `EventTarget` - Allows JavaScript to listen to events ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/addEventListener) )
 * `WebSocket` - API for the WebSocket protocol. ( [MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/WebSocket) )
