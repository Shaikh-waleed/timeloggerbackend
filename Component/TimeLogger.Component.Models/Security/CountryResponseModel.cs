using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class CountryResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Iso { get; set; }

        public List<CityModel> Cities { get; set; }
    }
}
