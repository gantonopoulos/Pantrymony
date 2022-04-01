namespace Pantrymony.Model;

public class Unit
{
    public string Name { get; set; } = string.Empty;

    public static Unit Kg = new() { Name = "kg" };
    public static Unit Ml = new() { Name = "ml" };
}