using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.Model
{
    public class UserResponseModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public int? CompanyId { get; set; }

        public StatusModel Status { get; set; }
    }
}
