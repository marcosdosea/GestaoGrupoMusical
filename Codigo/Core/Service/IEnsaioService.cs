﻿using Core.DTO;

namespace Core.Service
{
    public interface IEnsaioService
    {
        /// <summary>
        /// Cadastra uma ensaio no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// 401 - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Create(Ensaio ensaio);
        /// <summary>
        /// Editar uma ensaio no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// 401 - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Edit(Ensaio ensaio);
        Task<bool> Delete(int id);
        Task<Ensaio> Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO();
    }
}
