# Physics

Some types of interaction and animation are easiest to describe using physics simulation. Fuse comes with a set of physics based triggers and behaviors which can be used for these situations.

Each physics behavior is designed to simulate a certain kind of force. They apply these forces to each frame of the animation. Affected elements can respond to multiple forces at a time. Check out the [swipe places](/examples/swipe-places) example for a good introduction to the Fuse physics API.

## Physics rules

Each physics behavior in Fuse implements its own physics rule. There are currently only a few rules to choose from with more coming in the future.

## PointAttractor

The @PointAttractor is one of the most basic physics based behaviors. It creates a force field which attracts elements within a certain radius by a given force.

	<PointAttractor Radius="400" Strength="250" />


## Draggable

The @Draggable behavior lets you move an element by dragging it with the pointer. It is as simple as adding the @Draggable behavior directly to the element one wants to move.

	<Rectangle Color="Red" Width="100" Height="100">
		<Draggable/>
	</Rectangle>

