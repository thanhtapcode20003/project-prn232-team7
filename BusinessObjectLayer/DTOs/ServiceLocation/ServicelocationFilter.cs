namespace BusinessObjectLayer.DTOs.ServiceLocation
{
    public class ServicelocationFilter
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CampusName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
