using System;
using System.ComponentModel.DataAnnotations;

namespace AprossUtils.CustomAttributes
{
    public class CuitValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string cuit = value.ToString();
                bool res = false;
                try
                {
                    res = Validators.CuiltValidate(cuit);
                }
                catch (ArgumentNullException)
                {
                    res = true;
                }
                catch (Exception)
                {
                    res = false;
                }

                if (res)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Formato CUIL inválido");
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}