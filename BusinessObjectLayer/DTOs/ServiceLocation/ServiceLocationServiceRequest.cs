using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.DTOs.ServiceLocationRequest
{
    public class ServiceLocationServiceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Addres { get; set; } = string.Empty;
        public Guid CampusId { get; set; }
    }
}
