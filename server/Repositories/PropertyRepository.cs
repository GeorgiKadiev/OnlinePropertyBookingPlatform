using Microsoft.EntityFrameworkCore;

public class PropertyRepository
{
    private readonly PropertyManagementContext _context;

    public PropertyRepository(PropertyManagementContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> GetFilteredProperties(
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
        var query = _context.Properties.AsQueryable();

        if (isEcoFriendly.HasValue)
            query = query.Where(p => p.IsEcoFriendly == isEcoFriendly.Value);

        if (isDigitalNomadFriendly.HasValue)
            query = query.Where(p => p.IsDigitalNomadFriendly == isDigitalNomadFriendly.Value);

        if (hasParking.HasValue)
            query = query.Where(p => p.HasParking == hasParking.Value);

        if (hasHairDryer.HasValue)
            query = query.Where(p => p.HasHairDryer == hasHairDryer.Value);

        if (hasWiFi.HasValue)
            query = query.Where(p => p.HasWiFi == hasWiFi.Value);

        if (allowsPets.HasValue)
            query = query.Where(p => p.AllowsPets == allowsPets.Value);

        if (hasAirConditioning.HasValue)
            query = query.Where(p => p.HasAirConditioning == hasAirConditioning.Value);

        if (isSmokeFree.HasValue)
            query = query.Where(p => p.IsSmokeFree == isSmokeFree.Value);

        if (hasPool.HasValue)
            query = query.Where(p => p.HasPool == hasPool.Value);

        return await query.ToListAsync();
    }
}
