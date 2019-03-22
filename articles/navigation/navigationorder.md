# Navigation Order

Navigation always has an "Active" page. Other pages in the navigation can be described to be either in front of, or behind that active page. As the active page changes the in front, or behind, status of each page changes accordingly.

> The actual Z-Order of the pages may not be the same as the navigation order. To understand however you can use a `PageControl` with default properties. The children of the `PageControl` have a matching Z-order index and navigation order: the first child is in the front, the last child in the back.

The ordering of navigation pages is important for triggers such as @EnteringAnimation, @ExitingAnimation, @WhileInEnterState and @WhileInExitState. In these names, "enter" refers to pages in front and "exit" refers to pages in the back.

For example, consider this `PageControl`:

```xml
	<PageControl>
		<Page ux:Name="A">...</Page>
		<Page ux:Name="B">...</Page>
		<Page ux:Name="C">...</Page>
		<Page ux:Name="D">...</Page>
	</PageControl>
```

If the active page is `C`, then both `A` and `B` are in front of it, and `D` is behind it. When the active page changes to `B`, then `A` remains in front, `C` moves to the back, and `D` remains in the back.


## Page Progress

The position of a page relative to the active one is referred to as it's progress. Pages behind the active one will have a negative progress, those in front will have a positive one, and the active page itself will have 0.

Many navigation controls, such as `Navigator`, will discretly switch the progress of a page. For example, `0` will become `1` when a back operation is performed, and `0` will become `-1` when a push operation is performed. This type of control only defines the positions `-1`, `0`, and `1`.

Some navigation controls, such as `PageControl`, manage the transition between pages internally. The progress of a page going forward will changed smoothly from `0` to `1`. This type of control also allows for positions beyond `1` and `-1`. A page that is two behind the active one will have progress `-2`.

For example, consider this `PageControl`:

```xml
	<PageControl>
		<Page ux:Name="A">...</Page>
		<Page ux:Name="B">...</Page>
		<Page ux:Name="C">...</Page>
		<Page ux:Name="D">...</Page>
	</PageControl>
```

Assume the user is in the middle of swiping from B -> C. The page progress of B will be `0.5` and C will be `-0.5`. This partial distance is used by the `Threshold` property of the @Fuse.Navigation.WhileNavigationTrigger triggers.

> How far a page is from the active one can also be exploited in the @Fuse.Navigation.NavigationAnimation triggers. The `Scale` property adjusts the activiation level of the trigger based on the distance.

Whether a control uses discrete, or continuous, changes changes how the @Fuse.Navigation.NavigationAnimation animations work. Since a `PageControl` controls the progress motion, an @EnteringAnimation doesn't need to specify any duration; the progress of the trigger is directly tied to the progress of the navigation. For discrete controls, like `Navigator`, an @EnteringAnimation requires a duration otherwise the switch will be instantaneous.


## History

Controls with history also have a navigation order to their pages. Operations on them may also use the terms "back" and "front".

Pages are ordered from the newest in the front to the oldest in the back. A "push" operation adds a new page to the front of the history. A "goto" page navigates backwards through the history. Like all navigation there is only one active page. 

For example, if the user visits pages `A` then `B` then `C`, the stack, from front to back is `C* B A` where `C*` is the active page. The pages `B` and `A` are behind the active one. If the user goes back the current page changes, `C B* A`, now `C` is in front of the active page and `A` is behind it.
