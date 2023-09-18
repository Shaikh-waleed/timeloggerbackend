using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Annotations
{
    public class StartDate : ValidationAttribute
    {
        private readonly string _endDatePropertyName;

        public StartDate(string endDatePropertyName)
        {
            _endDatePropertyName = endDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return new ValidationResult($"The field {context.MemberName} is required.");

            DateTime startDate;
            if (!DateTime.TryParse(value.ToString(), out startDate))
                return new ValidationResult($"Invalid date, please specify a valid date in {context.MemberName} field.");

            var endDateProperty = context.ObjectType.GetProperty(_endDatePropertyName);
            if (endDateProperty == null)
                return null; // Already validating it by appliing this attribute on other property

            var endDateValue = endDateProperty.GetValue(context.ObjectInstance);
            DateTime endDate;
            if (!DateTime.TryParse(endDateValue.ToString(), out endDate))
                return null; // Already validating it by appliing this attribute on other property

            if (startDate >= endDate)
                return new ValidationResult($"Invalid date, please make sure that {context.MemberName} field value must be less then {_endDatePropertyName}.");

            return null;
        }
    }
}
