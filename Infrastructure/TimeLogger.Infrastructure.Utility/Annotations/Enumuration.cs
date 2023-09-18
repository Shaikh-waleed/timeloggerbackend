using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Annotations
{
    public class Enumuration : ValidationAttribute
    {
        private readonly Type _enumType;

        public Enumuration(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return new ValidationResult($"The field {context.MemberName} is required.");

            var enums = Enum.GetValues(_enumType).OfType<Enum>().ToList();

            object enumValue = null;
            if (!Enum.TryParse(_enumType, value.ToString(), out enumValue))
                return new ValidationResult($"Please specify a valid value in {context.MemberName} field.");

            if (!enums.Any(x => x.Equals(enumValue)))
                return new ValidationResult($"Please specify a valid value in {context.MemberName} field.");

            return null;
        }
    }
}
