using Core;
using Core.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Util
{
    public class CPFAttribute : ValidationAttribute
    {

      
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var serviceProvider = validationContext.GetRequiredService<IServiceProvider>();
            var dbContext = serviceProvider.GetRequiredService<GrupoMusicalContext>();
            //string cpf = (string)value;

            var valueNoEspecial = Methods.RemoveSpecialsCaracts((string)value);



            var existe = dbContext.Pessoas.Any(p => p.Cpf == valueNoEspecial);
            if (existe)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
        public string GetErrorMessage() =>
            $"CPF Inválido";
    }
}
