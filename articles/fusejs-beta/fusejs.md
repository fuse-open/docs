
# FuseJS


> [callout_warn] Although included in the most recent public Fuse release, the features outlined in this document are still considered experimental and not yet ready for production use. That being said, we encourage you to try it out for yourself. Feedback will always be welcome!


FuseJS allows you to write your Fuse app logic in minimalistic, modern, testable and scaleable ECMAScript 6.

> Note: If you are curious about how FuseJS compares to existing JavaScript frameworks like Angular, Vue.js or React+Redux, see [this article](./comparing-to-other-frameworks.md).

## Hello, FuseJS

Here's a simple Todo app implemented in FuseJS.

> Note: For a more complete code-example, see [this project](https://github.com/Duckers/FuseJS-TodoApp).

`TodoApp.js`
```JS
class TodoItem {
	constructor(description) {
		this.description = description
		this.isDone = false
	}
}

export default class TodoApp {
	constructor() {
		this.todos = [ new TodoItem("Buy milk") ]
		this.newTodo = ""
	}

	addTodo() {
		this.todos.push(new TodoItem(this.newTodo))
		this.newTodo = ""
	}

	get todosRemaining() {
		return this.todos.filter(x => !x.isDone).length
	}
}
```

And here is a simple UX Markup view for this app:

`TodoApp.ux`

```XML
<App Model="TodoApp">
	<StackPanel>
		<Each Items="{todos}">
			<DockPanel>
				<Text>{description}</Text>
				<Switch Dock="Right" Value="{isDone}">
			</DockPanel>
		</Each>
		<Text>You have {todosRemaining} things left to do</Text>
		<TextBox Value="{newTodo}" />
		<Button Text="Add new todo" Clicked="{addTodo}" />
	</StackPanel>
</App>
```

Finally make sure we include `TodoApp.js` (or all `**.js` files) as a `FuseJS` item in the project includes:

`Todo.unoproj`

```JSON
{
  "Packages": [
    "Fuse",
    "FuseJS"
  ],
  "Includes": [
    "*",
    "**.js:FuseJS"
  ]
}
```

That's the gist of it. The rest of this documents explains all the concepts in more detail.

## App models  (state container)

*Models* are simply plain ES6 classes composed into an object graph like in the above example. 

As a rule of thumb, all state that is specific to your app should live in a singular *state container*, or *app model*. The `TodoApp` class in the above example acts as a app model for the entire todo app. 

This has many benefits, most notably:

* **Single source of truth**. The entire app state and event history can be inspected or serialized for debugging.
* **Separation of logic and view**. The different model classes can be tested in isolation or combination without any hard ties to a view heirarchy. 

We specify what the app model is by setting the `Model` property on the `App` tag to point to the `FuseJS` module that exports the class.

```XML
<App Model="TodoApp">
```

This will look for `TodoApp.js` among the files included as `FuseJS` in the project, and instantiate the exported `default` class. The resulting object is added to the data context for the UX Markup subtree.

## Automatic change detection

FuseJS will automatically detect changes to your models and update the view (declared in UX Markup) accordingly. There is no need to use observables or send change notifications manually.

In our above example, we do this:

```JS
addTodo() {
	this.todos.push(new TodoItem(this.newTodo))
	this.newTodo = ""
}
```

What will happen is that FuseJS notices you called the `addTodo` method on this particular object, which will mark this particular object as *dirty* and schedule it for change detection later. When your JS code is idle, scheduled dirty objects are scanned for changes and the UI is notified about the changes since last scan.

Things to note:

* Always use a method to manipulate a class instance's state. Writing directly to a field of another class instance will not trigger change detection on that object. Always consider fields private, unless it is a plain JSON structure.
* You can safely do batch changes without any unreasonable performance cost. Change detection will only happen *once*, when the call stack is clean (all your changes are done).
* Simple, individual list operations like `push`, `pop`, `splice` etc. will trigger the appropriate `Adding`/`RemovingAnimation` in UX Markup. Complex list changes, however, may not animate correctly. For such scenarios, consider using [Each.IdentityKey](api:fuse/reactive/instantiator/identitykey) or manipulating a `FuseJS/Observable` directly to achieve the desired changes.


### What about async?

Doing changes in asynchronous callbacks is also (mostly) fine. FuseJS uses <a href="https://github.com/angular/zone.js">Zone.js</a> (borrowed from <a href="https://angular.io/">Angular</a>). FuseJS will note what object is expecting the callback and mark it as dirty when the callback happens.

So this is perfectly fine; FuseJS will detect the changes to `this.items`:

```JS
fetchMore() {
	fetch(some_url)
		.then(data => data.json())
		.then(newItems => { this.items = items.concat(newItems); })
}
```

Please note that, while this works most of the time, Zone.js cannot intercept _every_ imaginable async event.
If you encounter a situation where the UI is not updating after assigning to a field from an asynchronous callback,
you can move the assignment to its own method to force change detection.

**Before:**

```js
getNewData() {
	doSomethingAsync((result) => {
		this.result = result;
	});
}
```

**After:**

```js
onGotResult(result) {
	this.result = result;
}

getNewData() {
	doSomethingAsync((result) => {
		this.onGotResult(result);
	});
}
```

## Derived state

You can add *derived state* to your models by using simple property `get`'ers:

```JS
get todosRemaining() {
	return this.todos.filter(x => !x.isDone).length
}
```

Derived state is not explicitly stored in the model, but computed on demand based on other state. If the change detector detects a change in the object's state, all derived state for that object, as well as any object that holds a reference to it, is recomputed for the UI.

## Two-way data binding

Note that `isDone` and `newTodo` in our above `TodoApp` example are bound to interactive UI controls (`Switch` and `TextBox` respectively). 

```XML
<Switch Dock="Right" Value="{isDone}">
```
By default, this creates a two-way data binding to the model. If the user manipulates the control, the model is updated, and vice versa. Derived properties are automatically re-calculated.

You can disable two-way data binding by adding `Read` to the binding:

```XML
<Switch Dock="Right" Value="{Read isDone}" >
```
You also two-way-bind to *derived state* by adding a corresponding `set` method for your `get` methods:

```JS
get radians() {
	return this.degrees / 180 * Math.PI
}
set radians(value) {
	this.degrees = value / Math.PI * 180
}
```

## Splitting into files and folders

You can safely split your FuseJS code into as many files and folders you want, and use EcmaScript6 `import`/`export` syntax to stitch it back together. It is recommended to do so on a class-by-class basis, so each class can be tested in isolation.

`Model/TodoItem.js`
```JS
export default class TodoItem {
	constructor(description) {
		this.description = description
		this.isDone = false
	}
}
```

```JS
import TodoItem from './TodoItem'

export default class TodoList {
	...
```

`TodoApp.ux`
```XML
<App Model="Model/TodoList">
```

A typical project structure may look like this:

```
Assets/
	done.png
Model/
	TodoItem.js
	ToodList.js
Components/
	MyComponent.js
	MyComponent.ux
Services/
	NavigationService.js
Pages/
	TodoListPage.ux
	TodoListPage.js
TodoApp.ux
Todo.unoproj
```

You can learn more about *pages*, *services* and *components* in the sections below.

## App navigation

FuseJS greatly simplifies app navigation in Fuse (compared to using `Router`) by moving all the navigation state into the app model (state container). You can express any complex transition by simply manipulating the variables that represent the navigation state as you want.

> Although not needed or recommended, you *can* still use the `Router` in combination with FuseJS. Read more in the `Router` docs.

### Page models

For each page, we create a class to hold the state for that page. We can inject any dependencies the page needs to the class constructor.

As an example, let's create a new page class that can display details about a particular todo, and some relevant derived stats.

`Pages/TodoItemPage.js`
```JS
export default class TodoItemPage {
	constructor(todoItem, todoList) {
		this.todoItem = todoItem
		this.todoList = todoList
	}

	get label() {
		return this.todoItem.description
	}

	get isDone() {
		return this.todoItem.isDone
	}

	// Computes a list of other todos that needs doing 
	// or is done (depending on what this one is)
	get similarTodos() {
		return this.todoList.filter(x => x.isDone === this.todoItem.isDone)
	}
}
```

### Page stacks (hierarchical navigation)

A common way to structure navigation in an app is as a *stack* of pages, where pages can be *pushed* (when we *go to* a page)  and *popped* (when we *go back*).

In FuseJS we represent a page stack with a simple array. We initialize the array with the default page.

`TodoApp.js`
```JS
import TodoList from 'Models/TodoList'
import TodoListPage from 'Pages/TodoListPage'
import TodoItemPage from 'Pages/TodoItemPage'

export class TodoApp {
	constructor() {
		this.todoList = new TodoList()
		this.pages = [ new TodoListPage(this.todoList) ]
	}

	gotoTodo(e) {
		this.pages.push(new TodoItemPage(e.data, this.todoList))
	}

	goBack() {
		this.pages.pop()
	}
}
```

By linking the `pages` array to a `Navigator`'s `Pages` property, we have a working navigation system:

```XML
<App Model="TodoApp">
	<Navigator Pages="{pages}">
		<TodoListPage ux:Template="TodoListPage" />
		<TodoItemPage ux:Template="TodoItemPage" />
	</Navigator>
</App>
```

The appropriate `ux:Template` is chosen based on the name of the ES6 class.

We can now create a corresponding view for `TodoItemPage.ux` that allows us to navigate "deeper" into related todos:

`Pages/TodoItemPage.ux`
```XML
<Page ux:Class="TodoItemPage">
	<StackPanel>
		<Text>{label}</Text>
		<Text>{isDone ? 'This todo is done' : 'This todo needs doing'}<Text>
		<Button Text="Go back" Clicked="{goBack}" />
		
		<Text Margin="0,20,0,0">Other things that {isDone ? 'is done' : 'needs doing'}:</Text>
		<Each Items="{similarTodos}">
			<Button Clicked="{gotoTodo}" Text="{description}">
		</Each>
	</StackPanel>
</Page>
```

We can navigate back to the default state (or any other state) from anywhere by simply rewriting the `pages` array to something appropriate:

```JS
goHome() {
	this.pages = [ new TodoListPage(this.todoList) ]
}
```

### Page lists (linear navigation)

You can also navigate linearly among a set of pages by using a `PageControl` and a plain array of pages:

```JS
this.pages = [ new HomePage(), new ContactsPage(), new SettingsPage() ]
```

```XML
<PageControl Pages="{pages}">
	<HomePage ux:Template="HomePage" />
	<ContactsPage ux:Template="ContactsPage" />
	<SettingsPage ux:Template="SettingsPage" />
</PageControl>
```

### Multi-level navigation

You can create multi-level navigation by using `pages`-arrays within pages and nest navigators and page controls arbitrarily.

## Component models (advanced)

Ideally, all your components (`ux:Class`'es) should be stateless; pure UX markup and no JavaScript. The state is fed into the component from the state container through its UX properties. However, some complex components, like forms or advanced pickers, may require internal state. 

To create a component with internal state, you can simply set the `Model` property on any visual UX element:

`Components/MyCompontent.ux`
```XML
<StackPanel ux:Class="MyComponent" Model="Components/MyComponent">
	...
</StackPanel>
```

This looks for `Components/MyComponent.js` and instantiates either the exported `default` class. Note that `Model` paths are *not* relative to the UX file.

### Accessing the view

#### ModelArgs

Component models can take a comma-separated list of arguments from UX, using the `ModelArgs` property.
These are passed on to your model's constructor.

```xml
<Panel ux:Class="MyComponent" Model="Components/MyComponent" ModelArgs="router, scrollView">
	<Router ux:Dependency="router" />
	<ScrollView ux:ame="scrollView" />
</Panel>
```

These are then available in your model class' constructor:

```js
export default class MyComponent {
	constructor(router, scrollView) {
		this.router = router;
		this.scrollView = scrollView;
	}
}
```

> [callout_warn] *Warning:* The JavaScript module you specify using the `Model` attribute will be re-evaluated every time one of the `ModelArgs` change.


#### Automatic ux:Property binding

Inside component models, we can also access any user-defined `ux:Property` on our component.

Say, for instance, that our component has a `Title` property:

`Components/MyComponent.ux`
```xml
<StackPanel ux:Class="MyComponent" Model="Components/MyComponent">
	<string ux:Property="Title" />
</StackPanel>
```

To use this property inside our model class, we must first declare it in our constructor, thereby giving it a default value.


> **Note:** Since Fuse runs JavaScript on a separate thread, the constructor can only assign a _default value_ to the UX property. Its effective value will not be available until sometime after our constructor has been called.
> If we give the property a default value in UX, or one is explicitly set when instantiating the component, that value will be used instead of the value provided from JavaScript.
>
> Unfortunately, it is currently not possible to wait for the value of a UX property to be ready. This problem will be addressed in an upcoming release.

`Components/MyComponent.js`
```js
export default class MyComponent {
	constructor() {
		this.Title = "";
	}
}
```

We can now read and write to the `ux:Property` by accessing the corresponding field on our model:

`Components/MyComponent.js`
```js
export class MyComponent {
	constructor() {
		this.Title = "";
	}

	get label() {
		return "The title is: " + this.Title
	}

	changeTitle() {
		this.Title = "Different title"
	}
}
```

`Components/MyComponent.ux`
```xml
<StackPanel ux:Class="MyComponent" Model="Components/MyComponent">
	<string ux:Property="Title" />
	<Text Value="{label}" />
	<Button Clicked="{changeTitle}" />
</StackPanel>
```
