# Fuse Plugin API

Do you want to make a Fuse plugin for your favorite editor, write a custom tool for Fuse, or integrate with it in some other way? By using `fuse daemon-client`, this can be achieved from virtually any language. Read ahead for a description of the communication protocol, as well as an example plugin written in JavaScript.

This guide starts with a technical introduction to our plugin API. However you may jump down to [usage](#usage) if you don't want a technical explanation. Still, we recommend you to read it, to have an understanding of what's going on behind the scenes.

## Fuse Protocol<a name="fuseprotocol"></a>
The Fuse plugin API is built on top of two abstraction layers.
The core layer is _Fuse Protocol_. _Fuse Protocol_ is a general purpose protocol, which can send various types of data with a minimal data overhead, and with simplicity in mind.

Data sent through the *Fuse Protocol* consists of two parts, a _header_ and a _payload_. Together they represent what we call a _message_. The header consists of two UTF-8 encoded strings, terminated by line breaks. The first string represents the type of the message, the second string represents the length of the payload.

Here is an example of how a message is structured:
```
MyMessageType\n
7\n
foo bar
```

## Daemon Protocol
On top of the _Fuse Protocol_ is the _Daemon protocol_. The daemon protocol defines three types of _Fuse Protocol_ messages, called **Event**, **Request** and **Response**. The payload for each of these types is UTF-8 encoded JSON, and looks as follows:

### Event
```javascript
{
	"Name": "ExampleEvent",
	"SubscriptionId": 32, // The id of the event subscription (set automatically by the daemon),
	"Data": { ... }, // An event-specific JSON-object with the event data
}
```

### Request
```javascript
{
	"Name": "MyRequest",
	"Id": 242, // Make this a unique number for each request, so we can recognize the matching response message
	"Arguments": { ... }, // A request-specific JSON-object with the request
}
```

### Response
```javascript
{
    "Id": 242, // The id of the request to which this is a response
    "Status": "Success", // Can be "Success", "Error" or "Unhandled"
    "Result": { ... }, // If status is "Succsess", a request-specific JSON-object with the response
    "Errors": [ ... ], // If status is "Error", an array of objects containing more error information 
}
```

### Request and Response
<a name="indepth-request"></a>The request and response system is based on RPC (remote procedure call). The Fuse daemon acts as a switch between requesters and responders. A request sent from a client to the daemon will be forwarded to the **last** client which provides a handler for a specific request type. A client can publish that it provides a service (handles a request type) by sending a [PublishService](#publishservice) request to the daemon. A service provider is supposed to handle one or more request types, by transforming the request to a response. The concept is similar to method invocation, where request message store the method name and arguments for the method to be invoked, and the return value of the method is either something valid or an error (similar to exceptions in exception based languages), which in turn is sent back as a response to the requester (response is sent to the daemon, which forwards it to the requester).

**A PublishService request**<a name="publishservice"></a>
```javascript
{
	"Name": "PublishService",
	"Arguments": {
		"RequestNames": ["MyRequest", ...], // An array of names of requests that we respond to
	}
}
```

### Events
Events are broadcasted on a message bus in a typical pub/sub fashion, where clients can subscribe to certain event types. Subscribing is done by sending a [subscribe](#subscriberequest) request to the daemon. A client can send events that are forwarded to the bus, by sending an event to the daemon.

One thing to note about our protocol is that there is never a direct connection between two clients, and they don't know about each other. All communication goes through the daemon, which works as a switch and a broadcaster.

**A Subscribe request**<a name="subscriberequest"></a>
```javascript
{
	"Name": "Subscribe",
	"Arguments": {
		"Filter": "<regex>", // .Net style regex filtering incoming events based on type
		"Replay": false, // Use replay if you want to receive messages that were sent before you connected
		"SubscriptionId": 32, // A locally unique number representing this subscription, so we can recognize the incoming events (and unsubscribe)
	}
}
```

The _Daemon Protocol_ itself does not define a set of message types; any user of the _Daemon Protocol_ is free to define types. Here's an example of a custom event message:

**A user-defined event**
```javascript
{
	"Name": "MyMouseEvent",
	"Data": {
		"MouseX": 200,
		"MouseY": 100
	}
}
```

## Usage<a name="usage"></a>
The command `fuse daemon-client <name-of-client>` will create a connection to the daemon and forward all data written to and read from stdin/stdout. Since this only requires you to have access to an API for starting processes, it should be usable from all major languages. In other words to send data to the daemon write to stdin and to receive data read from stdout. Also, protocol errors can be read from stderr. In addition to that `fuse daemon-client` quits with exit code 1 when the connection is considered lost. Any other error codes are considered fatal failures, which should not occur (please report to us if it happens).

## Requests and Events
One of the first thing you probably want to do after you have established a connection to the daemon, is to send a request. The two most important request types are [PublishService](#publishservice) and [Subscribe](#subscriberequest), more are listed in our [api reference](#fuse-api-reference). One example is to subscribe to all build logged events, which can be done by sending the following data (full test example [here](#example)):

```javascript
Request // Message type ended with a newline as described in the `Fuse Protocol` paragraph at the top
106 // Size of payload in bytes ended with a newline as described in the `Fuse Protocol` paragraph at the top
{"Name":"Subscribe","Id":101,"Arguments":{"Filter":"Fuse.BuildLogged","Replay":false,"SubscriptionId":42}}
```

After sending the `Subscribe` request above, you should receive all events named `Fuse.BuildLogged`. (Run `fuse preview` of a project to trigger this event). The events received should look similar to:
```javascript
Event
144
{"Name":"Fuse.BuildLogged","Data":{"BuildId":"6c7e6f55-74de-45d1-bdb5-9f9cf2bafbf7","Message":"Generating code and data\n"},"SubscriptionId":42}
```

You may also broadcast an event, which is done by sending it to the daemon. The daemon will handle the broadcasting of the event to all registered listeners. For example you may send a `Fuse.BuildLogged` event yourself (altough it's **not** recommended to broadcast custom events named the same as existing events). This is how your `Fuse.BuildLogged` event might look like:

```javascript
{
	"Name":"Fuse.BuildLogged",
    "Data": {
    	"BuildId": "1c7e6f55-74de-45d1-bdb5-9f9cf2bafbf7",
    	"Message": "My own build log event!\n"
    }
}
```

Note that we have omitted the _Fuse Protocol_ header and we will continue doing it for the rest of the article for clarity. However keep in mind that a message also have a header as explained [here](#fuseprotocol).

_PublishService_ is a request you may consider sending when you want to respond to custom request types. This means you provide functionality which other clients may request. For instance say that you provide a feature called `GetAgeOfStudent`, which means that something like the following should be sent to the daemon:
```javascript
{
	"Name": "PublishService",
	"Arguments": {
		"RequestNames": ["GetAgeOfStudent"]
	}
}
```

All `GetAgeOfStudent` requests will be routed to you from this point. If for instance a client connected to the daemon sends a `GetAgeOfStudent` request, you will receive it and it will also be your responsibility to send a response back.

```javascript
{
	"Name": "GetAgeOfStudent",
    "Id": 2,
	"Arguments": {
		"StudentName": "Bob"
	}
}
```

You may receive a request as above. However how you respond to it is up to you, for example:

```javascript
{
    "Id": 2,
    "Status": "Success",
	"Result": {
		"Age": "24"
	},
    "Errors": null
}
```

The client who requested `GetAgeOfStudent` will receive the response, and also be the only one receiving it.

## Example<a name="example"></a>

This article is not really heading anywhere with just words so here is a Javascript/Node.js example showing how to create a simple communication with the daemon, which spits out the build log. Copy and paste the code to a file and run `node file`, also make sure you have [nodejs](https://nodejs.org/en/).

```javascript
var spawn = require('child_process');

// Spawn daemon client
var fuseClient = spawn.spawn("fuse", ['daemon-client', 'Simple Client']);

var buffer = new Buffer(0);
fuseClient.stdout.on('data', function (data) {
  // Data is a stream and must be parsed as that
  var latestBuf = Buffer.concat([buffer, data]);
  buffer = parseMsgFromBuffer(latestBuf, function (message) {
    console.log("Message received: " + message);
  });
});

fuseClient.stderr.on('data', function (data) {
  console.log(data.toString('utf-8'));
});

fuseClient.on('close', function (code) {
  console.log('fuse: daemon client closed with code ' + code);
});

// This is how you may subscribe to all "Fuse.BuildLogged" events
// In other words you will receive build output, by sending this request
var subRequest = JSON.stringify({
  Name: "Subscribe",
  Id: 101, // Arbitrary identifier to map responses back to requests
  Arguments: {
    Filter: "Fuse.BuildLogged",
    Replay: false, // Use replay to receive messages sent before you connected
    SubscriptionId: 42 // Arbitrary ID to map events to a particular subscription
  }
});

console.log("Sending: " + subRequest);
send(fuseClient, "Request", subRequest);

function parseMsgFromBuffer(buffer, msgCallback) {
  var start = 0;

  while (start < buffer.length) {
    var endOfMsgType = buffer.indexOf('\n', start);
    if (endOfMsgType < 0)
      break; // Incomplete or corrupt data

    var startOfLength = endOfMsgType + 1;
    var endOfLength = buffer.indexOf('\n', startOfLength);
    if (endOfLength < 0)
      break; // Incomplete or corrupt data

    var msgType = buffer.toString('utf-8', start, endOfMsgType);
    var length = parseInt(buffer.toString('utf-8', startOfLength, endOfLength));
    if (isNaN(length)) {
      console.log('fuse: Corrupt length in data received from Fuse.');
      // Try to recover by starting from the beginning
      start = endOfLength + 1;
      continue;
    }

    var startOfData = endOfLength + 1;
    var endOfData = startOfData + length;
    if (buffer.length < endOfData)
      break; // Incomplete data

    var jsonData = buffer.toString('utf-8', startOfData, endOfData);
    msgCallback(jsonData);
    start = endOfData;
  }

  return buffer.slice(start, buffer.length);
}

function send(fuseClient, msgType, serializedMsg) {
  // Pack the message to be compatible with Fuse Protocol.
  // As:
  // '''
  // MessageType (msgType)
  // Length (length)
  // JSON(serializedMsg)
  // '''
  // For example:
  // '''
  // Event
  // 50
  // {
  //   "Name": "Test",
  //   "Data":
  //   {
  //     "Foo": "Bar"
  //   }
  // }
  // '''
  var length = Buffer.byteLength(serializedMsg, 'utf-8');
  var packedMsg = msgType + '\n' + length + '\n' + serializedMsg;
  try {
    fuseClient.stdin.write(packedMsg);
  }
  catch (e) {
    console.log(e);
  }
}
```

For a full real-world example, please refer to the official open source [Atom Plugin](https://github.com/fusetools/Fuse.AtomPlugin).

## Fuse API Reference<a name="fuse-api-reference"></a>

This is a list consisting of examples of messages that we have in Fuse. However be aware that some of these messages are subject to change (but they will be deprecated before removed). You should also have a look at [PublishService](#publishservice) and [Subscribe](#subscriberequest).

### Build
There are a few build events that you may want to subscribe to, when you for example are making an error list.

**Fuse.BuildStarted - Event**

Sent after a build was started, and contains information related to the build.

```javascript
{
	"Name": "Fuse.BuildStarted",
	"SubscriptionId": 32, // The id provided in the subscribe response
	"Data":
	{
		"BuildType": "FullCompile", // This means the whole project is built. Can also be "LoadMarkup", which means that we're live-reloading an already built app
		"BuildId": "a19de798-6d22-45f8-b453-0a44ea606025", // A GUID that identifies this build, keep it around if you're intereseted in which build events are related to this build
		"BuildTag": "DaveWasHere", // Build-tag is a way to recognize build events from a build you started yourself. We can set it using `fuse preview --name=<YourTag>`. The default value is emtpy or null.
		"PreviewId": "cceb4dbc-e515-4dda-bff2-46cda79a9696", // If build type is "LoadMarkup", the BuildId of the initial full compile used by preview
		"ProjectPath": "C:\\FuseProjects\\MyProject.unoproj", // The native path to the project file being built
		"Target": "DotNetDll", // The target of the build. The supported targets are "DotNetDll" (local preview), "iOS" and "Android". Other targets include "DotNetExe", "CMake", "MSVC12", "WebGL" and "Unknown".
    }
}
```

**Fuse.BuildLogged - Event**

Sent after build has logged something.
```javascript
{
	"Name": "Fuse.BuildLogged",
	"SubscriptionId": 32, 
	"Data":
	{
		"BuildId":  "a19de798-6d22-45f8-b453-0a44ea606025",
		"Message": "...", // A human readable message logged during build
	}
}
```

**Fuse.BuildIssueDetected - Event**

Sent when a build issue was detected.
```javascript
{
	"Name": "Fuse.BuildIssueDetected",
	"SubscriptionId": 32, 
	"Data":
	{
		"BuildId":  "a19de798-6d22-45f8-b453-0a44ea606025",
		"IssueType": "Error", // Can be "Unknown", "FatalError", "Error", "Warning" or "Message"
		"Path": "C:\\FuseProjects\\MyClass.uno", // Path to the file where the error supposedly occurred. Can be empty in other words optional.
		"StartPosition": {"Line": 1, "Character": 1}, // 1-indexed start position in the file where the error is occurred (Optional)
		"EndPosition": {"Line": 1, "Character": 1}, // 1-indexed end position in the file where the error occurred (Optional)
		"ErrorCode": "E0001", // Error code generated by the Uno compiler
		"Message": "..." // A human readable description of the issue
	}
}
```

**Fuse.BuildEnded - Event**

Sent after build completed (successfully or not).
```javascript
{
	"Name": "Fuse.BuildEnded",
	"SubscriptionId": 32, 
	"Data":
	{
		"BuildId":  "a19de798-6d22-45f8-b453-0a44ea606025",
		"Status": "Success", // Status of build. Valid statues are "InternalError", "Error" and "Success".
	}
}
```

### Code Completion

**Fuse.GetCodeSuggestions - Method**

**Request example**

```javascript
{
	"Id": 42, // Unique request id
	"Name": "Fuse.GetCodeSuggestions",
	"Arguments": 
	{
		"SyntaxType": "UX", // Typically "UX" or "Uno"
		"Path": "C:\\FuseProjects\\MainView.ux", // Path to document where suggestion is requested
		"Text": "<App>\n\t<Button />\n</App>", // Full source of document where suggestion is requested 
		"CaretPosition": { "Line": 2, "Character": 9 } // 1-indexed text position within Text where suggestion is requested 
	}
}
```

**Response example**

```javascript
{
	"Id": 42, // Id of request
	"Status": "Success",
	"Result": 
	{
		"IsUpdatingCache": false, // If true you should consider trying again later
		"CodeSuggestions": 
		[
			{
				"Suggestion": "<suggestion>",
				"PreText": "<pretext>",
				"PostText": "<posttext>",
				"Type": "<datatype>",
				"ReturnType": "<datatype>",
				"AccessModifiers": [ "<accessmodifier>", ... ],
				"FieldModifiers": [ "<fieldmodifier>", ... ],
				"MethodArguments": 
				[
					{ "Name": "<name>", "ArgType": "<datatype>", "IsOut": "<false|true>" },
					...
				],
			},
			...
	    	],
	}
}
```

## Monitor

**Fuse.LogEvent - Event**

Sent from a preview instance as a result of calling *debug_log*, *console.log* or using *DebugAction*.
```javascript
{
	"Name": "Fuse.LogEvent",
	"Data": 
	{
		"ClientId": "b564e85a-fd37-46aa-a8d8-e601e08fe4f5", // Unique id of client, generated during build
		"ClientName": "MyPhone 5", // Friendly client name
		"Message": "...", // Message that was logged (using debug_log)
		"ConsoleType": "DebugLog", // Reserved
	},
}
```

**Fuse.ExceptionEvent - Event**

Sent after an exception occured in a preview instance.
```javascript
{
	"Name": "Fuse.ExceptionEvent",
	"Data": 
	{
		"ClientId": "b564e85a-fd37-46aa-a8d8-e601e08fe4f5", // Unique id of client, generated during build
		"ClientName": "MyPhone 5", // Friendly client name
		"Type": "Uno.NullReferenceException", // Name of exception Uno-type
		"Message": "Object reference not set to an instance of an object.", // A human readable description of the issue
		"StackTrace": "...", // String containing the stacktrace for where the exception was thrown. Run `fuse preview --enable-cppstacktrace` if you want stacktrace on iOS and Android
	}
}
```

## Preview

**Fuse.SelectionChanged - Event**

Send this event if you want to set selection in preview.
```javascript
{
	"Name": "Fuse.Preview.SelectionChanged",
    "Data":
    {
    	"Path": "C:\\FuseProjects\\MainView.ux", // Path to the file where selection was changed
        "Text": "<App>\n\t<Button />\n</App>", // Full source of document
        "CaretPosition": { "Line": 2, "Character": 9 } // 1-indexed text position within Text where selection was changed
    }
}
```
