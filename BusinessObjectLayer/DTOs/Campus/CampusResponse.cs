namespace BusinessObjectLayer.DTOs.Campus
{
    public class CampusResponse
    {
        public Guid CampusId { get; set; } = Guid.Empty;
        public string CampusName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
