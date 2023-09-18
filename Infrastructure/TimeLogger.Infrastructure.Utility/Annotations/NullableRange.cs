using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Annotations
{
    public class NullableRange : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        public NullableRange(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return null;

            int inputValue;
            if (!int.TryParse(value.ToString(), out inputValue))
                return new ValidationResult($"Invalid value, please specify a valid number value in {context.MemberName} field.");

            string message = _maxValue == int.MaxValue
                                            ? $"{context.MemberName} field value must be greater then {_minValue - 1}."
                                            : $"{context.MemberName} field value must be between {_minValue} and {_maxValue}.";

            if (inputValue < _minValue || inputValue > _maxValue)
                return new ValidationResult($"{message}");

            return null;
        }
    }
}
