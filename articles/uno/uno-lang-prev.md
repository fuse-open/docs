# prev

The prev keyword is used in meta property contexts to refer to the previous definition of a meta property.

There are two usages for the prev keyword.

1) If used alone as a primary expression, the keyword refers to the previous definition of the meta property currently being defined. Example:

```csharp
int Foo: 10; 
Foo: prev * 2;    // Sets Foo to 20
Foo: prev * 3;  // Sets Foo to 60
```

2) If used as an operator in front of a meta property name, prev refers to the previous definition of that meta property. Putting multiple prev keywords in front of each other refers to an even earlier definition. Examples:

```csharp
int Foo: 10;

int Bar1: Foo;       // Sets Bar1 to 10

int Bar2: prev Foo; // Sets Bar2 to 60 (assuming we follow the previous code above in example 1)

Foo: prev * 2;        // Sets Foo to 20

Foo: prev * 2;        // Sets Foo to 40

int Bar3: prev Foo; // Sets Bar3 to 40

int Bar4: prev prev Foo; // Sets Bar4 to 20 - note the use of double 'prev'

int Bar5: prev prev prev Foo; // Sets Bar5 to 10 - note the use of triple 'prev'
```

Note that prev searches upward the draw path from the current meta property, regardless of how many times the meta property in question is defined later.

Using prev is useful in order to avoid circular meta property references. The following would cause a circular reference and compile time error:

```csharp
int Foo: 10;
Foo: Foo * 2;   // Circular reference - this is not allowed 
```