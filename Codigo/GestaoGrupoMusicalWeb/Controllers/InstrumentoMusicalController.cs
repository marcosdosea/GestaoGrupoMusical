using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InstrumentoMusicalController : BaseController
    {
        private readonly IInstrumentoMusicalService _instrumentoMusical;
        private readonly IPessoaService _pessoa;
        private readonly IMovimentacaoInstrumentoService _movimentacaoInstrumento;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IMapper _mapper;

        public InstrumentoMusicalController(
            IInstrumentoMusicalService instrumentoMusical,
            IPessoaService pessoa,
            IMovimentacaoInstrumentoService movimentacaoInstrumento,
            IGrupoMusicalService grupoMusical,
            IMapper mapper)
        {
            _instrumentoMusical = instrumentoMusical;
            _pessoa = pessoa;
            _movimentacaoInstrumento = movimentacaoInstrumento;
            _grupoMusical = grupoMusical;
            _mapper = mapper;

        }

        // GET: InstrumentoMusicalController
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Index()
        {
            
            var listaInstrumentoMusical = await _instrumentoMusical.GetAllDTO(_grupoMusical.GetIdGrupo(User.Identity.Name));
            return View(listaInstrumentoMusical);
        }


        // GET: InstrumentoMusicalController/Details/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Details(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // GET: InstrumentoMusicalController/Create
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Create()
        {
            InstrumentoMusicalViewModel instrumentoMusicalViewModel = new InstrumentoMusicalViewModel();

            IEnumerable<Tipoinstrumento> listaInstrumentos = await _instrumentoMusical.GetAllTipoInstrumento();

            instrumentoMusicalViewModel.ListaInstrumentos = new SelectList(listaInstrumentos, "Id", "Nome", null);

            return View(instrumentoMusicalViewModel);
        }

        // POST: InstrumentoMusicalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Create(InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {

            int idGrupo = _grupoMusical.GetIdGrupo(User.Identity.Name);
            instrumentoMusicalViewModel.IdGrupoMusical = idGrupo;
            if (ModelState.IsValid)
            {
                var instrumentoMusicalModel = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                switch (await _instrumentoMusical.Create(instrumentoMusicalModel))
                {
                    case 100:
                        Notificar("A data de aquisição <b>" + instrumentoMusicalModel.DataAquisicao.ToShortDateString() + "</b> é maior que a data de Hoje <b>" + DateTime.Now.ToShortDateString() + "</b>", Notifica.Alerta);
                        break;
                    case 500:
                        Notificar("Falha ao <b>cadastrar<b> instrumento.", Notifica.Erro);
                        break;
                    case 200:
                        Notificar("Instrumento <b>Cadastrado<b> com <b>Sucesso<b>.", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                }

            }
            IEnumerable<Tipoinstrumento> listaInstrumentos = await _instrumentoMusical.GetAllTipoInstrumento();
            instrumentoMusicalViewModel.ListaInstrumentos = new SelectList(listaInstrumentos, "Id", "Nome", null);
            return View(nameof(Create), instrumentoMusicalViewModel);
        }



        // GET: InstrumentoMusicalController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Edit(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            if (instrumentoMusicalModel.Status != "EMPRESTADO")
            {
                IEnumerable<Tipoinstrumento> listaInstrumentos = await _instrumentoMusical.GetAllTipoInstrumento();

                instrumentoMusicalModel.ListaInstrumentos = new SelectList(listaInstrumentos, "Id", "Nome", null);

                return View(instrumentoMusicalModel);
            }
            else
            {
                Notificar("Não é permitido <b>Editar</b> os dados de um instrumento <b>Emprestado</b>.", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InstrumentoMusicalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Edit(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {

            if (instrumentoMusicalViewModel.IsDanificado != null)
            {
                if (instrumentoMusicalViewModel.IsDanificado == true)
                {
                    instrumentoMusicalViewModel.Status = "DANIFICADO";
                }
                else
                {
                    instrumentoMusicalViewModel.Status = "DISPONIVEL";
                }
            }

            if (ModelState.IsValid)
            {
                if (await _instrumentoMusical.Edit(_mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel)) == 500)
                {
                    Notificar("Falha ao <b>Editar</b> instrumento.", Notifica.Erro);
                    return RedirectToAction(nameof(Edit));
                }
                if (instrumentoMusicalViewModel.DataAquisicao > DateTime.Now)
                {
                    Notificar("A data de aquisição <b>" + instrumentoMusicalViewModel.DataAquisicao.ToShortDateString() + "</b> é maior que a data de Hoje <b>" + DateTime.Now.ToShortDateString() + "</b>", Notifica.Alerta);
                    return RedirectToAction(nameof(Edit));
                }
                var instrumentoMusical = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                await _instrumentoMusical.Edit(instrumentoMusical);
                Notificar("Instrumento Musical <b>Editado</b> com <b>Sucesso</b>.", Notifica.Sucesso);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Delete/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Delete(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);

            if (instrumentoMusicalModel.Status != "EMPRESTADO")
            {
                return View(instrumentoMusicalModel);
            }
            else
            {
                Notificar("Não é permitido <b>Deletar</b> os dados de um instrumento <b>Emprestado</b>.", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: InstrumentoMusicalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Delete(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            await _instrumentoMusical.Delete(id);
            Notificar("Instrumento Musical <b>Deletado</b> com <b>Sucesso</b>.", Notifica.Sucesso);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Movimentar(int id)
        {
            MovimentacaoInstrumentoViewModel movimentacaoModel = new();
            var instrumento = await _instrumentoMusical.Get(id);
            var movimentacao = await _movimentacaoInstrumento.GetEmprestimoByIdInstrumento(id);
            if (instrumento.Status.Equals("DANIFICADO"))
            {
                Notificar("Não é permitido fazer uma <b>movimentação</b> de um instrumento <b>Danificado</b>", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }

            if (instrumento == null)
            {
                Notificar($"O Id {id} não <b>Corresponde</b> a nenhuma <b>Movimentação</b>", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }

            if (movimentacao != null && instrumento.Status == "EMPRESTADO")
            {
                movimentacaoModel.IdAssociado = movimentacao.IdAssociado;
                movimentacaoModel.Movimentacao = "DEVOLUCAO";
            }

            movimentacaoModel.Movimentacoes = await _movimentacaoInstrumento.GetAllByIdInstrumento(id);
            movimentacaoModel.Patrimonio = instrumento.Patrimonio;
            movimentacaoModel.IdInstrumentoMusical = instrumento.Id;
            movimentacaoModel.NomeInstrumento = await _instrumentoMusical.GetNomeInstrumento(id);

            int idGrupo = _grupoMusical.GetIdGrupo(User.Identity.Name);
            var listaPessoas = _pessoa.GetAllPessoasOrder(idGrupo).ToList();
           // listaPessoas.Remove(listaPessoas.Single(p => p.Cpf == User.Identity?.Name));

            movimentacaoModel.ListaAssociado = new SelectList(listaPessoas, "Id", "Nome");
            return View(movimentacaoModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Movimentar(MovimentacaoInstrumentoViewModel movimentacaoPost)
        {
            movimentacaoPost.ListaAssociado = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            movimentacaoPost.Movimentacoes = await _movimentacaoInstrumento.GetAllByIdInstrumento(movimentacaoPost.IdInstrumentoMusical);

            if (ModelState.IsValid)
            {
                var colaborador = await _pessoa.GetByCpf(User.Identity?.Name);

                if (colaborador != null && (colaborador.IdPapelGrupo == 2 || colaborador.IdPapelGrupo == 3))
                {

                    var movimentacao = new Movimentacaoinstrumento
                    {
                        Data = movimentacaoPost.Data,
                        IdInstrumentoMusical = movimentacaoPost.IdInstrumentoMusical,
                        IdAssociado = movimentacaoPost.IdAssociado,
                        IdColaborador = colaborador.Id,
                        TipoMovimento = movimentacaoPost.Movimentacao
                    };

                    switch (await _movimentacaoInstrumento.CreateAsync(movimentacao))
                    {
                        case 200:
                            if (movimentacao.TipoMovimento == "EMPRESTIMO")
                            {
                                Notificar("Instrumento <b>Emprestado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                            }
                            else
                            {
                                Notificar("Instrumento <b>Devolvido</b> com <b>Sucesso</b>", Notifica.Sucesso);
                            }
                            return RedirectToAction(nameof(Movimentar), new { id = movimentacaoPost.IdInstrumentoMusical });
                        case 400:
                            Notificar("Não é possível <b>Emprestar</b> um instrumento <b>Danificado</b>", Notifica.Alerta);
                            return RedirectToAction(nameof(Movimentar), new { id = movimentacaoPost.IdInstrumentoMusical });
                        case 401:
                            if (movimentacao.TipoMovimento == "EMPRESTIMO")
                            {
                                Notificar("Não é possível <b>Emprestar</b> um instrumento que não está <b>Disponível</b>", Notifica.Alerta);
                            }
                            else
                            {
                                Notificar("Não é possível <b>Devolver</b> um instrumento que não está <b>Emprestado</b>", Notifica.Alerta);
                            }
                            break;
                        case 402:
                            Notificar("Esse <b>Associado</b> não corresponde ao <b>Empréstimo</b> desse <b>Instrumento</b>", Notifica.Erro);
                            break;
                        case 500:
                            Notificar("Desculpe, ocorreu um <b>Erro</b> durante a <b>Movimentação</b> do instrumento, se isso persistir entre em contato com o suporte", Notifica.Erro);
                            break;
                    }
                }
                else
                {
                    Notificar("Desculpe, você não tem <b>permissão</b> para realizar essa <b>operação</b>", Notifica.Erro);
                }
            }

            return View(movimentacaoPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> DeleteMovimentacao(int id, int IdInstrumento)
        {
            switch (await _movimentacaoInstrumento.DeleteAsync(id))
            {
                case 200:
                    Notificar("Movimentação <b>Excluida</b> com <b>Sucesso</b>", Notifica.Sucesso);
                    break;
                case 400:
                    Notificar("Não é possível <b>Excluir</b> essa <b>Movimentação</b> de <b>Empréstimo</b> pois o instrumento não foi <b>Devolvido</b>", Notifica.Alerta);
                    break;
                case 404:
                    Notificar($"O Id {id} não <b>Corresponde</b> a nenhuma <b>Movimentação</b>", Notifica.Erro);
                    break;
                case 500:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante a <b>Exclusão</b> da movimentação, se isso persistir entre em contato com o suporte", Notifica.Erro);
                    break;
            }

            return RedirectToAction(nameof(Movimentar), new { id = IdInstrumento });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> NotificarViaEmail(int id, int IdInstrumento)
        {
            switch (await _movimentacaoInstrumento.NotificarViaEmailAsync(id))
            {
                case 200:
                    Notificar("Notificação <b>Enviada</b> com <b>Sucesso</b>", Notifica.Sucesso);
                    break;
                case 401:
                    Notificar("O instrumento <b>Não</b> está <b>Cadastrado</b> no sistema, por favor entre em contato com o suporte", Notifica.Erro);
                    break;
                case 402:
                    Notificar("O correspondente <b>Não</b> está <b>Cadastrado</b> no sistema, por favor entre em contato com o suporte", Notifica.Erro);
                    break;
                case 404:
                    Notificar($"O Id {id} não <b>Corresponde</b> a nenhuma <b>Movimentação</b>", Notifica.Erro);
                    break;
                case 500:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Envio</b> da notificação, se isso persistir entre em contato com o suporte", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Movimentar), new { id = IdInstrumento });
        }

        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> Movimentacoes()
        {
            var associado = await _pessoa.GetByCpf(User.Identity?.Name);
            if (associado == null)
            {
                return RedirectToAction("Sair", "Identity");
            }

            return View();
        }
    }
}
