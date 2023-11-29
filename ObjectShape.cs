using Godot;
using Godot.NativeInterop;
using System;

[GlobalClass]
public partial class ObjectShape : Resource
{
    [Export]
    public Vector2[] Points {get; set;}

    [Export]
    public Color Color {get; set;}
}
