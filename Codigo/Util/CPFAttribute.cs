using Core.Service;
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

        private readonly IPessoaService _pessoaService;

        public CPFAttribute(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

           
        }
    }
}
