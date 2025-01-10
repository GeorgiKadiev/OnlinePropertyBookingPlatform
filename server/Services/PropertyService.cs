public class PropertyService
{
    private readonly PropertyRepository _repository;

    public PropertyService(PropertyRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Property>> GetFilteredProperties(
        bool? isEcoFriendly,
        bool? isDigitalNomadFriendly,
        bool? hasParking,
        bool? hasHairDryer,
        bool? hasWiFi,
        bool? allowsPets,
        bool? hasAirConditioning,
        bool? isSmokeFree,
        bool? hasPool)
    {
        return _repository.GetFilteredProperties(
            isEcoFriendly,
            isDigitalNomadFriendly,
            hasParking,
            hasHairDryer,
            hasWiFi,
            allowsPets,
            hasAirConditioning,
            isSmokeFree,
            hasPool);
    }
}
