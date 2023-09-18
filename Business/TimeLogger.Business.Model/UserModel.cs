using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeLogger.Business.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public int? CompanyId { get; set; }
        public int StatusId { get; set; }
    }
}
