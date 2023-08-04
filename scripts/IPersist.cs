using Godot;
using System;

public interface IPersist
{
    Godot.Collections.Dictionary<string,object> Save();
}
