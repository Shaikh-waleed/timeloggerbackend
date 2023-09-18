using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class PreviousPassword
    {
        // [Key, Column(Order = 0)] Created using fluent API
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        //[Key, Column(Order = 1)]  Created using fluent API
        public string UserId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }


        [ForeignKey("UserId")]
        public UserIdentity User { get; set; }
    }
}
