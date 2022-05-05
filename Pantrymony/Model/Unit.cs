namespace Pantrymony.Model;

public class Unit
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Symbol}({Name})";
    }
}