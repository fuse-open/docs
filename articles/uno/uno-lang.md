# Uno Language Reference

The Uno langauge is a dialect of C#, designed for cross-compilation to C++ and other languages. Uno does not require the .NET Framework, but has instead a more lightweight library called UnoCore.

The syntax of Uno is more or less identical to C#, with deviations documented here.

## Memory management

Like C# (and Java, JavaScript etc.), Uno features automatic memory management. This means you generally don't need to worry about manually reclaiming memory.

## Uno-specific keywords

These keywords are specific to Uno or have semantics that differ from their C# equivalents.

* [apply](uno-lang-apply.md)
* [assert](uno-lang-assert.md)
* [block](uno-lang-block.md)
* [debug_log](uno-lang-debug-log.md)
* [defined](uno-lang-defined.md)
* [draw](uno-lang-draw.md)
* [draw_dispose](uno-lang-draw-dispose.md)
* [drawable block](uno-lang-drawable-block.md)
* extern
* [fixed (arrays)](uno-lang-fixed-arrays.md)
* [import](uno-lang-import.md)
* intrinsic
* immutable
* [meta_block](uno-lang-meta-block.md)
* norm
* [pixel](uno-lang-pixel.md)
* [prev](uno-lang-prev.md)
* [req](uno-lang-req.md)
* [sample](uno-lang-sample.md)
* strict
* swizzler
* [undefined](uno-lang-undefined.md)
* [vertex](uno-lang-vertex.md)
* [vertex_attrib](uno-lang-vertex-attrib.md)
* [virtual (apply)](uno-lang-virtual-apply.md)
* [virtual (members)](https://msdn.microsoft.com/en-us/library/9fkccyh4.aspx)
* volatile

## C#-equivalent keywords

The meaning of keywords are inherited from the C# specification, with a few minor deviations as specified in the footnotes.

* [abstract](http://msdn.microsoft.com/en-us/library/sf985hc5.aspx)
* [as](http://msdn.microsoft.com/en-us/library/cscsdfbt.aspx)
* [base](http://msdn.microsoft.com/en-us/library/hfw7t1ce.aspx)
* [break](http://msdn.microsoft.com/en-us/library/adbctzc4.aspx)
* [case](http://msdn.microsoft.com/en-us/library/06tc147t.aspx)
* [catch](http://msdn.microsoft.com/en-us/library/0yd65esw.aspx)
* [checked](http://msdn.microsoft.com/en-us/library/74b4xzyw.aspx)<sup>x</sup>
* [class](http://msdn.microsoft.com/en-us/library/0b0thckt.aspx)<sup>6</sup>
* [const](http://msdn.microsoft.com/en-us/library/e6w8fe1b.aspx)
* [continue](http://msdn.microsoft.com/en-us/library/923ahwt1.aspx)
* [default](http://msdn.microsoft.com/en-us/library/25tdedf5.aspx)
* [delegate](http://msdn.microsoft.com/en-us/library/ms173171.aspx)
* [do](http://msdn.microsoft.com/en-us/library/370s1zax.aspx)
* [else](http://msdn.microsoft.com/en-us/library/5011f09h.aspx)
* [enum](http://msdn.microsoft.com/en-us/library/sbbt4032.aspx)
* [event](http://msdn.microsoft.com/en-us/library/8627sbea.aspx)
* [explicit](http://msdn.microsoft.com/en-us/library/xhbhezf4.aspx)
* [false](http://msdn.microsoft.com/en-us/library/67bxt5ee.aspx)
* [finally](http://msdn.microsoft.com/en-us/library/zwc8s4fz.aspx)
* [for](http://msdn.microsoft.com/en-us/library/ch45axte.aspx)
* [foreach](http://msdn.microsoft.com/en-us/library/ttw7t8t6.aspx)
* [if](http://msdn.microsoft.com/en-us/library/5011f09h.aspx)
* [implicit](http://msdn.microsoft.com/en-us/library/z5z9kes2.aspx)
* [in](http://msdn.microsoft.com/en-us/library/ttw7t8t6.aspx)
* [interface](http://msdn.microsoft.com/en-us/library/87d83y5b.aspx)
* [internal](http://msdn.microsoft.com/en-us/library/7c5ka91b.aspx)<sup>1</sup>
* [is](http://msdn.microsoft.com/en-us/library/scekt9xw.aspx)
* [new](http://msdn.microsoft.com/en-us/library/51y09td4.aspx)<sup>2</sup>
* [null](http://msdn.microsoft.com/en-us/library/edakx9da.aspx)
* [operator](http://msdn.microsoft.com/en-us/library/s53ehcz3.aspx)
* [out](http://msdn.microsoft.com/en-us/library/ee332485.aspx)<sup>3</sup>
* [override](http://msdn.microsoft.com/en-us/library/ebca9ah3.aspx)
* [params](http://msdn.microsoft.com/en-us/library/w5zay9db.aspx)
* [partial](http://msdn.microsoft.com/en-us/library/wbx7zzdd.aspx)<sup>4</sup>
* [private](http://msdn.microsoft.com/en-us/library/st6sy9xe.aspx)
* [protected](http://msdn.microsoft.com/en-us/library/bcd5672a.aspx)
* [public](http://msdn.microsoft.com/en-us/library/yzh058ae.aspx)
* [readonly](http://msdn.microsoft.com/en-us/library/acdd6hb7.aspx)
* [ref](http://msdn.microsoft.com/en-us/library/14akc2c7.aspx)<sup>3</sup>
* [return](http://msdn.microsoft.com/en-us/library/1h3swy84.aspx)
* [sealed](http://msdn.microsoft.com/en-us/library/88c54tsw.aspx)
* [static](http://msdn.microsoft.com/en-us/library/98f28cdx.aspx)
* [struct](http://msdn.microsoft.com/en-us/library/ah19swz4.aspx)
* [switch](http://msdn.microsoft.com/en-us/library/06tc147t.aspx)
* [this](http://msdn.microsoft.com/en-us/library/dk1507sz.aspx)
* [throw](http://msdn.microsoft.com/en-us/library/1ah5wsex.aspx)
* [true](http://msdn.microsoft.com/en-us/library/eahhcxk2.aspx)
* [try](http://msdn.microsoft.com/en-us/library/0yd65esw.aspx)
* [unchecked](http://msdn.microsoft.com/en-us/library/a569z7k8.aspx)<sup>x</sup>
* [using](http://msdn.microsoft.com/en-us/library/sf0df423.aspx)
* [var](http://msdn.microsoft.com/en-us/library/bb383973.aspx)<sup>5</sup>
* [void](http://msdn.microsoft.com/en-us/library/yah0tteb.aspx)
* [while](http://msdn.microsoft.com/en-us/library/2aeyhxcd.aspx)

Footnotes:
* x : These keywords are not currently supported, but reserved for future improved compatiblity with C#.
* 1 : A package in Uno is equivalent to a assembly in C#.
* 2 : Uno does currenlty not support generic constraints (i.e. the new keyword cannot be used in this context)
* 3 : In Uno, out and ref cannot be used as a generic type parameter modifier.
* 4 : Uno only supports partial types, not partial methods (C# 5.0). Also note that when using meta properties in partial classes, the order in which the partial classes are combined becomes undefined, hence the order in which the meta properties are defined also becomes undefined in some cases.
* 5 : In Uno, using var is mandatory in cases where no assign-cast is performed, otherwise a warning is generated.
* 6 : In Uno, classes may also specify blocks in the class base list, where this indicates the class will use meta property signatures from (but not apply) the specified blocks.
