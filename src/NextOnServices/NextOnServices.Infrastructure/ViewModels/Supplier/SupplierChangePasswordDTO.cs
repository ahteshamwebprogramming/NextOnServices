using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Supplier;

public class SupplierChangePasswordDTO
{
    public int? Id { get; set; }
    public string? encId { get; set; }

    [Required(ErrorMessage = "Old password is required.")]
    public string? OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 8)]
    [InvalidCharacterPasswordValidation]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "The new password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Please confirm your new password.")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
}
public class InvalidCharacterPasswordValidation : ValidationAttribute
{
    // List of invalid characters with their specific error messages
    private readonly Dictionary<string, string> invalidCharacters = new Dictionary<string, string>
    {
        { "(", "The password cannot contain opening brackets ('(')." },
        { ")", "The password cannot contain closing brackets (')')." },
        { " ", "The password cannot contain spaces." },
        { ".", "The password cannot contain dots ('.')." },
        { ",", "The password cannot contain commas (',')." },
        { "=", "The password cannot contain equal signs ('=')." },
        { "+", "The password cannot contain plus signs ('+')." }
    };

    public override bool IsValid(object? value)
    {
        string? password = value as string;
        if (string.IsNullOrEmpty(password))
        {
            return false; // required validation will handle this
        }

        // Check for invalid characters
        foreach (var invalidChar in invalidCharacters.Keys)
        {
            if (password.Contains(invalidChar))
            {
                // Return the error message specific to the invalid character found
                ErrorMessage = invalidCharacters[invalidChar];
                return false; // Invalid if any invalid character is found
            }
        }

        return true; // Valid if no invalid characters are found
    }
}