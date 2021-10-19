# Supported platforms

This page describes what is needed in order to use Fuse on your computer.

## Operating system

* [Fuse X](articles:basics/installation-and-quickstart) (visual tooling) is available for **macOS** and **Windows**.
* [Fuse SDK](https://www.npmjs.com/package/fuse-sdk) (command-line) is available for **Linux**, in addition to **macOS** and **Windows**.

<blockquote class="callout-info">

Please note that beta versions of both macOS and Windows are not officially supported.

</blockquote>

### macOS Requirements

- OS X 10.10 Yosemite or newer
- macOS 11.2 Big Sur (or latest) is recommended
- Intel processor (M1 support is planned)
- <a href="https://www.mono-project.com/download/stable/" target="_blank">Mono</a> 6.10 or newer
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 12 or newer

> Read the [step-by-step guide for macOS](installation/setup-install-mac.md).

### Windows Requirements

- Windows 7 or newer
- Windows 10 is recommended
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 12 or newer

> Read the [step-by-step guide for Windows](installation/setup-install-win.md).

### Linux Requirements

- <a href="https://www.mono-project.com/download/stable/" target="_blank">Mono</a> 6.0 or newer
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 12 or newer

<blockquote class="callout-info">

Linux users can use the <a href="https://www.npmjs.com/package/fuse-sdk" target="_blank">Fuse SDK</a>. Fuse X is not available for Linux.

</blockquote>

## Hardware

* OpenGL 2.1-capable GPU is required. Intel GMA is an example of a GPU that is _not_ sufficient.

<blockquote class="callout-info">

Please note that some computers have multiple GPUs, and that [a problem with Windows](http://support.displaylink.com/forums/287786-displaylink-feature-suggestions/suggestions/15869604-fix-some-applications-not-running-on-laptop-gpu) might cause it to pick the wrong one. If this happens to you, please try to disable the low end GPU, as described in the linked article.

</blockquote>

## Export targets

* Android 4.1 (Jelly Bean, API 16) and newer
* iOS 9.0 and newer

<blockquote class="callout-info">

Fuse apps can also be exported to Linux, macOS and Windows via the `native` or `dotnet` build targets.

</blockquote>
