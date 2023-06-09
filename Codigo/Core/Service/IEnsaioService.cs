﻿using Core.DTO;

namespace Core.Service
{
    public interface IEnsaioService
    {
        Task<bool> Create(Ensaio ensaio);
        Task<bool> Edit(Ensaio ensaio);
        Task<bool> Delete(int id);
        Task<Ensaio> Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO();
    }
}
