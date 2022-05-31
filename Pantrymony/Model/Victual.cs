namespace Pantrymony.Model;

public class Victual
{
    public string UserId { get; set; }
    
    public Guid VictualId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public ushort Quantity { get; set; }
    
    public double Calories { get; set; }
    
    public double Protein { get; set; }
    
    public double Fat { get; set; }
    
    public DateTime Expires { get; set; }
    
    public double Carbs { get; set; }

    public string Unit { get; set; }

    public override string ToString()
    {
        return $"{UserId},{VictualId},{Name},{ImageUrl},{Quantity},{Calories},{Protein},{Fat},{Carbs},{Expires},{Unit}";
    }
}

