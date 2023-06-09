﻿using System;
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

        Task<bool> AddAdmGroup(Pessoa pessoa);
        IAsyncEnumerable<Pessoa> GetAllAdmGroup(int id);
        Task<bool> RemoveAdmGroup(int id);
    }
}
