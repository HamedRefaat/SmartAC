using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System;
using System.Reflection;

namespace Theoremone.SmartAc.Api.Validations.Device
{
    public class SemanticVersion : ValidationAttribute
    {

        private const string SemanticVersionRegexPattern = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
        private new const string ErrorMessage = "The firmware value does not match semantic versioning format.";

        public SemanticVersion():base()
        {

        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            Regex _regexValidator = new(SemanticVersionRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool _valueMatched = _regexValidator.IsMatch(input: $"{value}");
            if (!_valueMatched)
                return new ValidationResult(ErrorMessage, new List<string>() { validationContext.DisplayName });
            return ValidationResult.Success;
        }

    }
}
