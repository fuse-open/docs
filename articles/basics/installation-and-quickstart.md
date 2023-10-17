# Installation and Quickstart

This is a step-by-step tutorial that takes you through setting up Fuse X and creating your first project. Should you happen to run into problems during installation, please [let us know](https://fuse-x.com/issues)!

## Installation

The latest version of Fuse X can always be downloaded from the <a href="https://fuse-x.com/download">downloads page</a>. Download the installer for your operating system and open it to start the installation procedure.

<blockquote class="callout-info">

<a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> is required to install this software.

</blockquote>

<blockquote class="callout-info">

The installers require a working internet connection in order to complete.

</blockquote>

### macOS Requirements

- OS X 10.10 Yosemite or newer
- macOS 13.2 Ventura (or latest) is recommended
- Intel processor (M1 support is planned)
- <a href="https://dotnet.microsoft.com/en-us/download/dotnet/6.0" target="_blank">.NET</a> 6.0 or newer
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 16 or newer

> Read the [step-by-step guide for macOS](installation/setup-install-mac.md).

### Windows Requirements

- Windows 10 or newer
- <a href="https://dotnet.microsoft.com/en-us/download/dotnet/6.0" target="_blank">.NET</a> 6.0 or newer
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 16 or newer

> Read the [step-by-step guide for Windows](installation/setup-install-win.md).

### Linux Requirements

- <a href="https://dotnet.microsoft.com/en-us/download/dotnet/6.0" target="_blank">.NET</a> 6.0 or newer
- <a href="https://nodejs.org/en/download/" target="_blank">Node.js</a> 16 or newer

<blockquote class="callout-info">

Linux users can use the <a href="https://www.npmjs.com/package/fuse-sdk" target="_blank">Fuse SDK</a>. Fuse X is not available for Linux.

</blockquote>

## Finding a text editor

Fuse X requires an external text editor in order to make changes to the UX markup in our app. To streamline your experience with Fuse X, we provide plugins for some of the more popular text editors which provide code completion, error lists, output logs and the ability to launch Fuse apps from within the text editor.

We currently provide plugins for the following text editors:

<table class="table">
  <thead>
    <tr>
      <th>Editor</th>
      <th>Installation</th>
      <th>Plugin homepage</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><a href="https://code.visualstudio.com/">Visual Studio Code</a></td>
      <td><a href="articles:basics/installation/vscode-extension.md">link</a></td>
      <td><a href="https://marketplace.visualstudio.com/items?itemName=fuseopen.fuse-vscode">link</a></td>
    </tr>
    <tr>
      <td><a href="https://atom.io/">Atom</a></td>
      <td><a href="articles:basics/installation/atom-plugin.md">link</a></td>
      <td><a href="https://atom.io/packages/fuse">link</a></td>
    </tr>
    <tr>
      <td><a href="https://www.sublimetext.com/3">Sublime Text 3</a></td>
      <td><a href="articles:basics/installation/sublime-plugin.md">link</a></td>
      <td><a href="https://packagecontrol.io/packages/Fuse">link</a></td>
    </tr>
  </tbody>
</table>

## Starting a new project

To open Fuse X, double click the the **fuse X** icon. On macOS it is located in the *Applications* folder. If you are on Windows, it can be located in the *Start Menu*.

<blockquote class="callout-info">

You can also start Fuse X by running `fuse` from terminal on macOS or command prompt on Windows.

</blockquote>

When opening Fuse X, the first thing you'll see is the dashboard:

![image of dashboard](../../media/installation_quickstart/fusedashboard.png)

To start a new project, we first have to pick a template. Fuse comes with a few templates which provide a good starting point when starting from scratch. For the purpose of this guide, we want to choose the "New Fuse project" template. This template gives a project with one file in which we can easily start building our app. Choose this template by clicking the box with its name, and then click the "create" button. You will then have to pick a name and a directory for your project.

![create project](../../media/installation_quickstart/createprojectname.png)

After clicking the "create" button, Fuse X starts a local preview viewport. You'll notice that this process take some time, as Fuse X has to download the packages containing the [framework code](http://github.com/fuse-open/fuselibs) used to create apps with Fuse X. This only happens the first time Fuse X is run after installation. You can see the download progress in the "Log" panel at the bottom of the window.

![preview](../../media/installation_quickstart/preview.png)

After Fuse X is done downloading all the required packages, the preview viewport is started, and you'll notice that all we have to start with is a white background.

### Making some changes

To open the project in your text editor, either click the "Project" tab and select `MainView.ux` or open the project folder directly from your text editor.

To get started, paste the following code directly into `MainView.ux`, replacing the existing code:

```xml
<App Background="#2196F3">
  <ClientPanel>
      <StackPanel ItemSpacing="10">
          <Text FontSize="30">Hello, world!</Text>
          <Slider />
          <Button Text="Button" />
          <Switch Alignment="Left" />
      </StackPanel>
  </ClientPanel>
</App>
```

As soon as you save the document, you should notice the preview viewport update to display some `Text`, a `Slider`, a `Button` and a `Switch` control, stacked on top of each other vertically. Fuse picks up any change you make to your documents, and updates automatically and immediately to reflect those changes. This even works on your devices which you can read about in the next section.

## Running preview on device

Fuse X can do live preview on both Android and iOS devices, even at the same time!

The simplest way to get started is with the [Fuse X Preview app](./preview-and-export.md#fuse-x-preview-app) so you should definitely try that one out first.

If you ever need to include non-standard packages or Uno code in your project then you can instead build your own [custom preview](./preview-and-export.md#custom-preview), but you can make that switch whenever you want to so there's no harm in starting out with the Fuse X Preview app.

## What next?

Now that you're all set up, it's time to get started learning the fundamentals of Fuse. A good next step is to take a look at the [Introduction to Fuse](articles:basics/introduction-to-fuse) module, which will take you through the fundamentals of Fuse. If you prefer learning by example, the [hikr tutorial](articles:tutorial/tutorial) will lead you through the creation of a simple hiking app, while explaining each step along the way. If you simply want to start digging into example code and play around on your own, there is a ton of content to look at on our [examples page](https://fuseopen.com/examples) as well as in the [fuse-samples github repo](https://github.com/fuse-open/fuse-samples/).
