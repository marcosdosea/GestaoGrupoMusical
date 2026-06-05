using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Core
    {
        public partial class DispositivoPessoas
        {
            public int Id { get; set; }

            public int IdPessoa { get; set; }

            public string FcmToken { get; set; } = null!;

            public DateTime DataAtualizacao { get; set; }

            public virtual Pessoa Pessoa { get; set; } = null!;
        }
    }
