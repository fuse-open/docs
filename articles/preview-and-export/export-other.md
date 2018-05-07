# Other export targets

Any other export targets besides iOS and Android should currently be considered experimental and may change or break at any time. That said, [please let us know about it in the forums](/community/forums) if you come across anything strange!

**Note:** FuseJS is currently **not supported for WebGL**.

## WebGL Export Note

When the build has completed using the WebGL target Fuse will open the app in your default browser.

Be aware that most browsers have default security settings that will disallow javascript access to files on the local file system and that this may prevent the application from working correctly. We can work around this either by running the app from a local server, or by changing the browsers security settings.

The security settings can be changed as follows:

* **Safari** - Select "Disable local file restrictions" in the Develop menu.
* **Chrome** - First close *all* running instances of the browser. Then restart it with the --allow-file-access-from-files command line parameter.
* **Firefox** - Open about:config in the browser and set security.fileuri.strict_origin_policy to false.

If anything does not behave as expected the best place to start is by opening up the browsers javascript console and checking for errors.
