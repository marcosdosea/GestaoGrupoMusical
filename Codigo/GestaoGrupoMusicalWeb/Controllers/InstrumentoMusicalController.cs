using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR GRUPO")]
    public class InstrumentoMusicalController : BaseController
    {
        private readonly IInstrumentoMusicalService _instrumentoMusical;
        private readonly IPessoaService _pessoa;
        private readonly IMovimentacaoInstrumentoService _movimentacaoInstrumento;
        private readonly IMapper _mapper;

        public InstrumentoMusicalController(
            IInstrumentoMusicalService instrumentoMusical, 
            IPessoaService pessoa, 
            IMovimentacaoInstrumentoService movimentacaoInstrumento, 
            IMapper mapper)
        {
            _instrumentoMusical = instrumentoMusical;
            _pessoa = pessoa;
            _movimentacaoInstrumento = movimentacaoInstrumento;
            _mapper = mapper;

        }

        // GET: InstrumentoMusicalController
        public async Task<ActionResult> Index()
        {
            var listaInstrumentoMusical = await _instrumentoMusical.GetAllDTO();
            return View(listaInstrumentoMusical);
        }


        // GET: InstrumentoMusicalController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // GET: InstrumentoMusicalController/Create
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
        public async Task<ActionResult> Create(InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            if (ModelState.IsValid)
            {
                var instrumentoMusicalModel = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                await _instrumentoMusical.Create(instrumentoMusicalModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);

            IEnumerable<Tipoinstrumento> listaInstrumentos = await _instrumentoMusical.GetAllTipoInstrumento();

            instrumentoMusicalModel.ListaInstrumentos = new SelectList(listaInstrumentos, "Id", "Nome", null);

            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {

            if(instrumentoMusicalViewModel.IsDanificado != null)
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
                var instrumentoMusical = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                await _instrumentoMusical.Edit(instrumentoMusical);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            await _instrumentoMusical.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Movimentar(int id)
        {
            MovimentacaoInstrumentoViewModel movimentacaoModel = new();
            var instrumento = await _instrumentoMusical.Get(id);
            var movimentacao = await _movimentacaoInstrumento.GetEmprestimoByIdInstrumento(id);

            if(instrumento == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if(movimentacao != null && instrumento.Status == "EMPRESTADO")
            {
                movimentacaoModel.IdAssociado = movimentacao.IdAssociado;
                movimentacaoModel.Movimentacao = "DEVOLUCAO";
            }

            movimentacaoModel.Movimentacoes = await _movimentacaoInstrumento.GetAllByIdInstrumento(id);
            movimentacaoModel.Patrimonio = instrumento.Patrimonio;
            movimentacaoModel.IdInstrumentoMusical = instrumento.Id;
            movimentacaoModel.NomeInstrumento = await _instrumentoMusical.GetNomeInstrumento(id);

            var listaPessoas = _pessoa.GetAll().ToList();
            listaPessoas.Remove(listaPessoas.Single(p => p.Cpf == User.Identity?.Name));

            movimentacaoModel.ListaAssociado = new SelectList(listaPessoas, "Id", "Nome");
            return View(movimentacaoModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                            return RedirectToAction(nameof(Movimentar));
                        case 400:
                            Notificar("Não é possível <b>Emprestar</b> um instrumento <b>Danificado</b>", Notifica.Alerta);
                            break;
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
        public async Task<ActionResult> DeleteMovimentacao(int id, int IdInstrumento)
        {
            if(await _movimentacaoInstrumento.DeleteAsync(id))
            {
                Notificar("Movimentação <b>Excluida</b> com <b>Sucesso</b>", Notifica.Sucesso);
            }
            else
            {
                Notificar("Desculpe, ocorreu um <b>Erro</b> durante a <b>Exclusão</b> da movimentação, se isso persistir entre em contato com o suporte", Notifica.Erro);
            }

            return RedirectToAction(nameof(Movimentar), new { id = IdInstrumento });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NotificarViaEmail(int id, int IdInstrumento)
        {
            if(await _movimentacaoInstrumento.NotificarViaEmailAsync(id))
            {
                Notificar("Notificação <b>Enviada</b> com <b>Sucesso</b>", Notifica.Sucesso);
            }
            else
            {
                Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Envio</b> de notificação, se isso persistir entre em contato com o suporte", Notifica.Erro);
            }
            return RedirectToAction(nameof(Movimentar), new { id = IdInstrumento });
        }
    }
}
