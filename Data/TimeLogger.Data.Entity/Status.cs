using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Data.Entity
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public StatusTypes TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual StatusType StatusType { get; set; }
    }
}
