﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Annotations
{
    public class NullableEndDate : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public NullableEndDate(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return null;

            DateTime endDate;
            if (!DateTime.TryParse(value.ToString(), out endDate))
                return new ValidationResult($"Invalid date, please specify a valid date in {context.MemberName} field.");

            var startDateProperty = context.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
                return null; // Already validating it by appliing this attribute on other property

            var startDateValue = startDateProperty.GetValue(context.ObjectInstance);
            if (startDateValue == null)
                return null; // Already validating it by appliing this attribute on other property

            DateTime startDate;
            if (!DateTime.TryParse(startDateValue.ToString(), out startDate))
                return null; // Already validating it by appliing this attribute on other property

            if (endDate <= startDate || endDate < DateTime.UtcNow)
                return new ValidationResult($"Invalid date, please make sure that {context.MemberName} field value must greater then {_startDatePropertyName} and current date.");

            return null;
        }
    }
}
