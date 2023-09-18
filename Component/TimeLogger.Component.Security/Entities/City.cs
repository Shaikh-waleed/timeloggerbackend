using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class City
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Iso { get; set; }
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
    }
}
