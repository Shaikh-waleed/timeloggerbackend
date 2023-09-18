using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Business.Model
{
    public class StatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public StatusTypes TypeId { get; set; }
    }
}
