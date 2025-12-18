using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer.DTOs.ServiceLocationRequest
{
    public class ServiceLocationServiceRequest
    {
        [Required(ErrorMessage = "Campus name is required")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Campus name must be between 2 and 255 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 500 characters")]
        public string Address { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Campus ID is required")]
        [GuidNotEmpty(ErrorMessage = "Campus ID must be a valid GUID")]
        public Guid CampusId { get; set; }
    }
    public class GuidNotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is Guid guid)
            {
                return guid != Guid.Empty;
            }
            return false;
        }
    }
}
