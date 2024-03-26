using System;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Name { get; private set; }

    public CommandAttribute(string name)
    {
        Name = name;
    }
}