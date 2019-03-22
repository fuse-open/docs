# Dependencies (ux:Dependency)

The `ux:Dependency` attribute in UX Markup defines a new hard dependency (Uno constructor argument) that the containing `ux:Class` requires in order to be constructable.

## Syntax

```xml
	<type ux:Dependency="dependency_name" />
```

Where `type` is any type accessible in UX Markup, and `dependency_name` is a valid Uno identifier.

## Remarks

Components (defined with `ux:Class`) often require access to certain objects or services in it's environment to work. For example, a component may require access to the @App's @Router. 

We can declare a dependency in our component using the `ux:Dependency` attribute as follows:

```xml
	<Panel ux:Class="MyBackButton">
		<Router ux:Dependency="router" />
		<Panel ux:Dependency="panel" />
		
		<JavaScript>
			function clicked() {
				router.goBack();
			}
			
			module.exports = { clicked: clicked };
		<JavaScript>
		
		<Clicked>
			<Callback Handler="{clicked}">
		</Clicked>
		
		<WhilePressed>
			<Change panel.Opacity="0.5" Duration="0.3" />
		</WhilePressed>
	</Panel>
```

The above example declares two dependencies, `router` and `panel`. The `router` will be used to `.goBack()` when the component is clicked. The panel dubbed `panel` will be faded to half opacity while the component is pressed.

Dependencies are equivalent to constructor arguments in Uno, stored in `readonly` fields. This means the object is always known at initialization time and will never change, so we can safely use the object directly in @JavaScript or in animators such as `Change` by its given name.

When instantiating a component with dependencies, you have to provide objects for each dependency (i.e. dependency injection), otherwise a compile time error will be generated. 

```xml
	<App>
		<Router ux:Name="router" />
		<Panel ux:Name="p1" />
		<MyBackButton router="router" panel="p1" />
	</App>
```

A component can not provide a default value for its dependencies.

## Inheriting dependencies

Dependencies are not forwarded when you subclass. Therefore, you have to manually forward them to the baseclass you are sublcassing:

```xml
	<Page ux:Class="A">
		<Router ux:Dependency="router" />
	</Page>
	<A ux:Class="B">
		<Router ux:Dependency="router" ux:Binding="router" />
	</A>
```

### How is `ux:Dependency` different from `ux:Property` ?

Dependencies work similarly to properties, however there are a few key differences:

- Dependencies are immutable, meaning their value cannot change over time.
- Dependencies must be provided with a value at time of instantiation.
- Dependencies cannot have a default value.
- Dependencies are available as local named objects direclty in `<JavaScript>` tags (not as Observables).
- Dependencies are available as local names in the scope of the class, meaning you *don't* need to use a binding such as `{Property foo}` to access them, you can simply use `foo`.

