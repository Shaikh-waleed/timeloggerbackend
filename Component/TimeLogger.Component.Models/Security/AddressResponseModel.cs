using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class AddressResponseModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; }


        public IdentityUserModel User { get; set; }
        public CompanyModel Company { get; set; }
        public CountryModel Country { get; set; }
        public CityModel City { get; set; }
    }
}
