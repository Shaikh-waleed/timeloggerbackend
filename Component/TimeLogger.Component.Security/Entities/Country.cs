using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class Country
    {

        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Iso { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
