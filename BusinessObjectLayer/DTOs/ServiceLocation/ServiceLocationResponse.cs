using BusinessObjectLayer.DTOs.Campus;

namespace BusinessObjectLayer.DTOs.ServiceLocation
{
    public class ServiceLocationResponse
    {
        public Guid ServiceLocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string Address { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Description { get; set; } = null!;
        public CampusResponse Campus { get; set; } = null!;
    }
}
