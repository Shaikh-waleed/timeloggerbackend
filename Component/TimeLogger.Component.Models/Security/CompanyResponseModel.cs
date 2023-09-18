using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class CompanyResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }

        public List<AddressModel> Addresses { get; set; }
    }
}
