﻿using Core.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IPessoaService
    {
        Task<int> Create(Pessoa pessoa);
        void Edit(Pessoa pessoa);
        void Delete(int id);
        Pessoa Get(int id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<AssociadoDTO> GetAllAssociadoDTO();

        bool GetCPFExistente(int id, string cpf);

        Task<bool> AddAdmGroup(Pessoa pessoa);
        Task<IEnumerable<AdministradorGrupoMusicalDTO>> GetAllAdmGroup(int id);
        Task<bool> RemoveAdmGroup(int id);

        Task<bool> ToCollaborator(int id);
        Task<bool> RemoveCollaborator(int id);

        IEnumerable<Papelgrupo> GetAllPapelGrupo();
        void RemoverAssociado(Pessoa pessoa, String? motivoSaida);

        Task<bool> NotificarCadastroAdmGrupoAsync(Pessoa pessoa);

        Task<Pessoa?> GetByCpf(string? cpf);

    }
}
