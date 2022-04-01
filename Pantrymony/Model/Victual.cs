using System.Text.Json;

namespace Pantrymony.Model;

public class Victual
{
    public Guid Identifier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public ushort Quantity { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }

    public Unit Unit { get; set; } = Unit.Kg;

    public override string ToString()
    {
        return $"{Identifier},{Name},{ImageUrl},{Quantity},{Calories},{Protein},{Fat},{Carbs},{Unit.Name}";
    }
}

