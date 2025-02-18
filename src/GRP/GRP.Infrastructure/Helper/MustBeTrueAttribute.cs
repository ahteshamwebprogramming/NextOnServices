using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Helper;

public class MustBeTrueAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is bool && (bool)value)
        {
            return ValidationResult.Success;
        }
        return new ValidationResult(ErrorMessage ?? "You must agree to the terms and conditions.");
    }
}
