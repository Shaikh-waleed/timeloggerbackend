using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class Addresses
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


        [ForeignKey("UserId")]
        public virtual UserIdentity User { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }
    }
}
