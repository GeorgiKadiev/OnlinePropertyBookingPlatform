using System.ComponentModel.DataAnnotations;

public class Property
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsEcoFriendly { get; set; }
    public bool IsDigitalNomadFriendly { get; set; }
    public bool HasParking { get; set; }
    public bool HasHairDryer { get; set; }
    public bool HasWiFi { get; set; }
    public bool AllowsPets { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool IsSmokeFree { get; set; }
    public bool HasPool { get; set; }
}
