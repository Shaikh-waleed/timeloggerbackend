using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Data.Entity
{
    public class ApplicationUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Picture { get; set; }

        public int? CompanyId { get; set; }
    }
}
