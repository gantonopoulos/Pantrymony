namespace Pantrymony.Model;

public class Unit
{
    public string UnitName { get; set; }

    public static Unit Kg = new() { UnitName = "kg" };
    public static Unit Ml = new() { UnitName = "ml" };
}