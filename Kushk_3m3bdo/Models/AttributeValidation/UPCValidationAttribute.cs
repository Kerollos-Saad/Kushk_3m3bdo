using System.ComponentModel.DataAnnotations;

namespace Kushk_3m3bdo.Models.AttributeValidation
{
	public class UPCValidationAttribute: ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null)
			{
				long upc = (long)value;

				// Check if the number has exactly 12 digits
				if (upc.ToString().Length == 12)
				{
					return ValidationResult.Success;
				}
			}

			return new ValidationResult("The UPC number must be exactly 12 digits.");
		}
	}
}
