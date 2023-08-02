using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IFigurinoService
    {
        Task<int> Create(Figurino figurino);
        Task<int> Edit(Figurino figurino);
        Task<int> Delete(int id);
        Figurino Get(int id);

        /// <summary>
        /// Retorna uma lista contendo todos os figurinos
        /// naquele grupo musical
        /// </summary>
        /// <param name="cpf">cpf da pessoa autenticada para poder filtrar os figurinos</param>
        /// <returns>lista contendo todos os figurinos</returns>
        Task<IEnumerable<Figurino>> GetAll(string cpf);

        Task<Figurino> GetByName(string name);
    }
}
