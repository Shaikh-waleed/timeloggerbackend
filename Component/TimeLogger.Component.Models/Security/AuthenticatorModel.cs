using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TimeLogger.Component.Models.Security
{
    public class AuthenticatorModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        public string SharedKey { get; set; }

        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        public string AuthenticatorUri { get; set; }
    }
}
