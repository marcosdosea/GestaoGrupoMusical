using Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class CNPJAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var serviceProvider = validationContext.GetRequiredService<IServiceProvider>();
            var dbContext = serviceProvider.GetRequiredService<GrupoMusicalContext>();
            var valueNoEspecial = Methods.RemoveSpecialsCaracts((string)value);
            var existe = dbContext.Grupomusicals.Any(p => p.Cnpj == valueNoEspecial);
            bool valido = Methods.ValidarCnpj(valueNoEspecial.ToString());
            if (existe)
            {
                return new ValidationResult("Este CNPJ já está cadastrado no sistema! ");
            }
            else if (!valido) 
            {
                return new ValidationResult("CNPJ inválido!");
            }
            return ValidationResult.Success;
        }
    }
}
