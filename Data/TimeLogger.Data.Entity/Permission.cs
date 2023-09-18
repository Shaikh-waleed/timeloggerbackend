using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Data.Entity
{
    public partial class Permission
    {
        // [Key, ForeignKey("User")]
        public string UserId { get; set; }


        // Do not make relationShip with ApplicationUser becuase if we use SingleSignOn then it will not work
        //[ForeignKey("UserId")]
        //public ApplicationUser User { get; set; }
    }
}
