
# How FuseJS compares to other frameworks

When learning FuseJS it can be useful to understand how its concepts compare to other frameworks you already know. This page compares FuseJS to other popular frameworks.

Contents:

 - Redux / React
 - Vue.js
 - Angular

## Redux / React

When using <a href="http://redux.js.org/">Redux</a> and <a href="https://reactjs.org/">React</a> together, you use a *Redux store* as a state container that holds the global state of the app. In addition, each React component can hold component-local state.

### Redux Stores

The **Redux Store** is analogous to the FuseJS **App Model**. 

You specify the app model *class* like this: 

```XML
<App Model="AppModelClass">
```

This makes the app model available globally in the UX markup data context, throughout the app. Any UX markup component or page can data-bind directly to properties and methods on the app model.

**Redux Reducers** are analogous to **methods on the model classes** in FuseJS. In FuseJS, you don't need to think about immutability, you simply mutate the class instances. 

**Redux Action records** are analogous to serializing **the method names and arguments** for each method call on the app model coming from the outside (i.e. when a method is called from UX Markup, or an async callback happens).  By running change detection after each method call from the outside, the exact effects of the actions on the state can also be logged for time travel debugging.

#### Example - Redux

```JS

var initialState = {
    todos: []
}

function todoApp(state = initialState, action) {
    switch (action.type) {
        case 'ADD_TODO': {
            return { todos: state.todos.concat( action.description )}
        }
        default:
            return state
    }
}

function addTodo(description) {
    return { type: 'ADD_TODO', description }
}
```

#### Same example - FuseJS

```JS
export class TodoApp {
    constructor() {
        this.todos = []
    }
    addTodo(description) {
        this.todos.push(description)
    }
}
```

### React Components

React's **JSX** is analogous to Fuse's **UX Markup**. 

React's **Components** is analogous to `ux:Class` in UX Markup. For the most part, components are used for visual styling and organization, and is of no consequence to business logic. This means they don't need a JavaScript counterpart. 

React's **Props** is analogous to `ux:Property` declarations on the `ux:Class`. In the Fuse world, UX components are native controls, living and breathing with their properties on the UI thread. This means all properties can be animated in smooth 60 FPS without JavaScript interference.

React **component state** is analogous to FuseJS **Component Models**. 

You specify the component model *class* by setting the `Model` property on your `ux:Class`:

```XML
<Panel ux:Class="MyComponent" Model="MyComponent(this)">
    <string ux:Property="Label" />
```

Where `MyComponent` is plain JavaScript class that holds the state and possible actions on this component. The properties and methods of the component class will now be available for data binding within the component.

Optionally, the *component model* accepts a `view` argument to the constructor which gives the JavaScript class access to the `ux:Property` and gives the illusion of synchronous access to it.

```JS
export class MyComponent {
    constructor(view) {
        this.view = view
    }
    get fullLabel() {
        return "The label is: " + this.view.Label
    }
}
```

Do not use JavaScript for animating properties. Use Fuse's trigger/animator system from UX Markup instead. Animation is usually of no consequence to your business logic and your JavaScript should therefore not be concerned with it.

## Vue.js

In <a href="https://vuejs.org/">Vue</a>, **components** are analogous to `ux:Class` in Fuse. 

The Vue **template** is analogous to the UX Markup code for the class. In Fuse, the template (UX Markup code) is all you need to create a component, including interaction and animation. A JavaScript counterpart is optional (only needed for stateful components).

The `data` property of the Vue component is analogous to all the properties of your *component model* (see explaination under Redux / React).

The `methods` property of the Vue component is analogous to the methods of your component model. FuseJS allows model classes to compose (within the same component), while in Vue you compose the components.

If you are using Redux with Vue, it relates to FuseJS like using Redux with React.

## Angular

In <a href="https://angular.io/">Angular</a>, **components** are analogous to `ux:Class` in Fuse.

The Angular **template** is analogous to the UX Markup code for the class. In Fuse, the template (UX Markup code) is all you need to create a component, including interaction and animation. A JavaScript counterpart is optional (only needed for stateful components).

The **properties** of the Angular component is analogous to the propreties of your *component model* (see explaination under Redux / React).

The **methods** of the Angular component is analogous to the properties of your component model. 