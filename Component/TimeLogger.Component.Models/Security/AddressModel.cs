using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Component.Models.Security
{
    public class AddressModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? CompanyId { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool IsDefault { get; set; }
    }
}
