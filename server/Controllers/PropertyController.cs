using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly PropertyService _service;

    public PropertyController(PropertyService service)
    {
        _service = service;
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredProperties(
        [FromQuery] bool? isEcoFriendly,
        [FromQuery] bool? isDigitalNomadFriendly,
        [FromQuery] bool? hasParking,
        [FromQuery] bool? hasHairDryer,
        [FromQuery] bool? hasWiFi,
        [FromQuery] bool? allowsPets,
        [FromQuery] bool? hasAirConditioning,
        [FromQuery] bool? isSmokeFree,
        [FromQuery] bool? hasPool)
    {
        var properties = await _service.GetFilteredProperties(
            isEcoFriendly,
            isDigitalNomadFriendly,
            hasParking,
            hasHairDryer,
            hasWiFi,
            allowsPets,
            hasAirConditioning,
            isSmokeFree,
            hasPool);

        return Ok(properties);
    }
}
