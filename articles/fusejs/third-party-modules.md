# Third-party modules

Fuse is working hard to make it possible to reuse exiting JavaScript libraries from third parties. To make more third party
libraries work, we are constantly growing and revising our polyfills to improve compatibility. We can read about currently supported polyfills [here](polyfills.md)

## Important note about JavaScript in Fuse

It is important to explain that Fuse is not a web or browser-based platform despite using markup and JavaScript. The ways Fuse handle data binding, layout and animation are all tailored for the Fuse platform. Much like how any Node.js library that depends on modules like `process` and `fs` wouldn't be expected to work in a web page, Fuse's approach to JS is the same. In short: you'll have greater success with Fuse (and more fun!) if you approach it as a new thing rather than a typical DOM-based JS framework. :)

With that in mind, this chapter contains a list of libraries that are known to work/not work, and explains workarounds/caveats.

If you have tested a library and found it to work/not work/have a work-around, please [share your experience on our forums](/community/forums) or [let us know via email!](/contact)

> This list will be expanded as FuseJS develops.

## Working

The following third party JavaScript libraries are tested and known to work

* [Parse SDK](https://github.com/parse-community/Parse-SDK-JS)
* [lodash](https://lodash.com)
* [bluebird](https://github.com/petkaantonov/bluebird)
* [Moment.js](http://momentjs.com)
* [Syncano](https://github.com/Syncano/syncano-js)

<!-- TODO: Add this section as we uncover incompatible libraries
## Known incompatibilities

(nothing yet)
-->
