using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KelimeEzberleme.Validation
{
    public class RequiredIfExamManagerAttribute : ValidationAttribute
    {
        private readonly string _rolePropertyName;

        public RequiredIfExamManagerAttribute(string rolePropertyName)
        {
            _rolePropertyName = rolePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var roleProperty = validationContext.ObjectType.GetProperty(_rolePropertyName);
            if (roleProperty == null)
                return new ValidationResult($"Unknown property: {_rolePropertyName}");

            var roleValue = roleProperty.GetValue(validationContext.ObjectInstance, null) as string;

            if (roleValue == "ExamManager")
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? "Sınav sorumlusu için gizli şifre gereklidir.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
