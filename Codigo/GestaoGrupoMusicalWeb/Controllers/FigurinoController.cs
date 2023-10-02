using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Utilities;
using System.Net;
using System.Security.Principal;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class FigurinoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IManequimService _manequimService;
        private readonly IFigurinoService _figurinoService;
        private readonly IPessoaService _pessoaService;
        private readonly IMovimentacaoFigurinoService _movimentacaoService;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public FigurinoController(IMapper mapper, IGrupoMusicalService grupoMusical,
            IManequimService manequim, IFigurinoService figurino, UserManager<UsuarioIdentity> userManager, IPessoaService pessoa, IMovimentacaoFigurinoService movimentacaoService)
        {
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _manequimService = manequim;
            _figurinoService = figurino;
            _userManager = userManager;
            _pessoaService = pessoa;
            _movimentacaoService = movimentacaoService;
        }
        [HttpPost]
        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var response = await _figurinoService.GetDataPage(request, Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));
            return Json(response);
        }
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        // GET: FigurinoController
        public async Task<ActionResult> Index()
        {
            var listFigurinos = await _figurinoService.GetAll(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));

            var listFigurinosViewModdel = _mapper.Map<IEnumerable<FigurinoViewModel>>(listFigurinos);

            return View(listFigurinosViewModdel);
        }

        // GET: FigurinoController/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FigurinoController/Create
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: FigurinoController/Create
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FigurinoViewModel figurinoViewModel)
        {
            if (ModelState.IsValid)
            {
                figurinoViewModel.IdGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

                var figurino = _mapper.Map<Figurino>(figurinoViewModel);
                HttpStatusCode resul = await _figurinoService.Create(figurino);

                switch (resul)
                {
                    case HttpStatusCode.Created:
                        Notificar("<b>Sucesso</b>! Figurino cadastrado!", Notifica.Sucesso);
                        break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("<b>Erro</b>! Algo deu errado ao cadastrar novo figurino", Notifica.Erro);
                        return View(figurinoViewModel);
                    default:
                        Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                        break;
                }

                return RedirectToAction("Index");
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                return View();
            }
        }

        // GET: FigurinoController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Edit(int id)
        {
            var figurino = await _figurinoService.Get(id);
            var figurinoViewModel = _mapper.Map<FigurinoViewModel>(figurino);

            return View(figurinoViewModel);
        }

        // POST: FigurinoController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, FigurinoViewModel figurinoViewModel)
        {
            if (ModelState.IsValid)
            {
                var figurino = _mapper.Map<Figurino>(figurinoViewModel);

                figurino.IdGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

                HttpStatusCode resul = await _figurinoService.Edit(figurino);

                switch (resul)
                {
                    case HttpStatusCode.OK:
                        Notificar("<b>Sucesso</b>! Figurino alterado!", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));

                    case HttpStatusCode.InternalServerError:
                        Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                        return View(figurinoViewModel);

                    default:
                        Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                        return View(figurinoViewModel);
                }
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                return View();
            }
        }

        // GET: FigurinoController/Delete/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Delete(int id)
        {
            var figurino = await _figurinoService.Get(id);
            var figurinoViewModel = _mapper.Map<FigurinoViewModel>(figurino);

            return View(figurinoViewModel);
        }

        // POST: FigurinoController/Delete/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, FigurinoViewModel figurinoViewModel)
        {
            HttpStatusCode resul = await _figurinoService.Delete(id);

            switch (resul)
            {
                case HttpStatusCode.OK:
                    Notificar("<b>Sucesso</b>! Figurino removido!", Notifica.Sucesso);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro</b>! Algo deu errado ao remover figurino. Verifique se há estoque emprestado.", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                    return View(figurinoViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: FigurinoController/DeleteEstoque/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteEstoque(int idFigurino, int idManequim)
        {
            HttpStatusCode result = await _figurinoService.DeleteEstoque(idFigurino, idManequim);
            switch (result)
            {
                case HttpStatusCode.OK:
                    Notificar("<b>Sucesso</b>! Estoque removido!", Notifica.Sucesso);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("<b>Alerta</b>! Não é permitido <b>Excluir Estoque</b> com peças <b>Entregues</b>! Quantidade <b>Disponível</b> foi <b>zerada</b>.", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro</b>! Algo deu errado ao tentar remover estoque.", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                    break;
            }

            return RedirectToAction(nameof(Estoque), new { id = idFigurino });
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Estoque(int id)
        {
            EstoqueViewModel estoqueViewModel = new();
            var figurino = await _figurinoService.Get(id);

            var estoque = await _figurinoService.GetAllEstoqueDTO(id);

            estoqueViewModel.TabelaEstoques = estoque;

            estoqueViewModel.Id = figurino.Id;
            estoqueViewModel.Nome = figurino.Nome;
            estoqueViewModel.Data = figurino.Data;

            return View(estoqueViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> CreateEstoque(int idFigurino)
        {
            var figurino = await _figurinoService.Get(idFigurino);
            var manequins = _manequimService.GetAll();

            SelectList listManequins = new SelectList(manequins, "Id", "Tamanho");

            CreateEstoqueViewModel estoqueViewModel = new()
            {
                IdFigurino = figurino.Id,
                Nome = figurino.Nome,
                Data = figurino.Data.Value.ToString("dd/MM/yyyy"),
                listManequim = listManequins
            };

            return View(estoqueViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEstoque(CreateEstoqueViewModel estoqueViewModel)
        {
            Figurinomanequim estoque = new()
            {
                IdFigurino = estoqueViewModel.IdFigurino,
                IdManequim = estoqueViewModel.IdManequim,
                QuantidadeDisponivel = estoqueViewModel.QuantidadeDisponivel,
                QuantidadeEntregue = 0,
                QuantidadeDescartada = 0
            };

            HttpStatusCode resul = await _figurinoService.CreateEstoque(estoque);

            switch (resul)
            {
                case HttpStatusCode.Created:
                    Notificar("<b>Sucesso</b>! Estoque cadastrado.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.Accepted:
                    Notificar("<b>Sucesso</b>! Adicionado peças ao estoque.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("<b>Alerta</b>! Dados insuficientes.", Notifica.Alerta);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("<b>Alerta</b>! Sem quantidade disponível para estoque.", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro</b>! Algum problema ao registrar estoque.", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro</b>! Algum problema ao tentar registrar estoque.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Estoque), new { id = estoqueViewModel.IdFigurino });
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Movimentar(int id, int? page, string sortOrder, string currentFilter)
        {
            var figurino = await _figurinoService.Get(id);
            var manequins = await _movimentacaoService.GetEstoque(id);
            if (manequins == null)
            {
                Notificar("<b>Alerta</b>! Figurino não possue estoque.", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }

            int idGrupo = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
            var associados = _pessoaService.GetAllPessoasOrder(idGrupo);

            var movimentacoes = await _movimentacaoService.GetAllByIdFigurino(id);        
            SelectList listAssociados = new SelectList(associados, "Id", "Nome");
            SelectList listEstoque = new SelectList(manequins, "IdManequim", "TamanhoEstoque");

            var movimentarFigurinoViewModel = new MovimentacaoFigurinoViewModel
            {
                IdFigurino = figurino.Id,
                NomeFigurino = figurino.Nome,
                DataFigurinoString = figurino.Data.Value.ToString("dd/MM/yyyy"),
                ListaAssociado = listAssociados,
                ListaManequim = listEstoque,
                Movimentacoes = movimentacoes
            };

            return View(movimentarFigurinoViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Movimentar(MovimentacaoFigurinoViewModel movimentacaoViewModel)
        {
            var colaborador = await _pessoaService.GetByCpf(User.Identity.Name);

            string status = movimentacaoViewModel.Movimentacao;

                Movimentacaofigurino movimentacao = new Movimentacaofigurino
                {
                    Data = movimentacaoViewModel.Data,
                    IdFigurino = movimentacaoViewModel.IdFigurino,
                    IdManequim = movimentacaoViewModel.IdManequim,
                    IdAssociado = (movimentacaoViewModel.Movimentacao.Equals("DANIFICADO")) ? colaborador.Id : movimentacaoViewModel.IdAssociado,
                    IdColaborador = colaborador.Id,
                    Status = status,
                    ConfirmacaoRecebimento = (movimentacaoViewModel.Movimentacao.Equals("DANIFICADO")) ? (sbyte)1 : (sbyte)0,
                    Quantidade = movimentacaoViewModel.Quantidade
                };

            HttpStatusCode resul = await _movimentacaoService.CreateAsync(movimentacao);

            string tipoMov = string.Empty;

            if (movimentacaoViewModel.Movimentacao.Equals("ENTREGUE"))
            {
                tipoMov = "Entregue";
            }
            else
            {
                tipoMov = "Devolvido";
            }

            switch (resul)
            {
                case HttpStatusCode.OK:
                    Notificar($"<b>Sucesso!</b> Figurino foi <b>{tipoMov}</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("<b>Alerta!</b> Não há estoque desse tamanho", Notifica.Alerta);
                    break;
                case HttpStatusCode.NoContent:
                    Notificar("<b>Alerta!</b> Não há peças disponíveis para movimentação", Notifica.Alerta);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("<b>Alerta!</b> Não há nada para devolver", Notifica.Alerta);
                    break;
                case HttpStatusCode.FailedDependency:
                    Notificar("<b>Erro!</b> O <b>Associado</b> ainda não confirmou o recebimento do <b>Figurino</b>", Notifica.Alerta);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("<b>Erro!</b> Problema com a quantidade a devolver.", Notifica.Erro);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro!</b> Algo deu errado", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro!</b> Algo deu errado na operação", Notifica.Erro);
                    break;
            }

            return RedirectToAction(nameof(Movimentar), new { id = movimentacaoViewModel.IdFigurino });
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMovimento(int Id, int idFigurino)
        {
            if (Id != null && Id > 0)
            {
                HttpStatusCode resul = await _movimentacaoService.DeleteAsync(Id);

            switch (resul)
            {
                    case HttpStatusCode.OK:
                        Notificar($"<b>Sucesso!</b> Movimentação foi <b>removida</b>", Notifica.Sucesso);
                        break;
                    case HttpStatusCode.NotFound:
                        Notificar("<b>Alerta!</b> Não há movimentação com essas informações", Notifica.Alerta);
                        break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("<b>Erro!</b> Algo deu errado", Notifica.Erro);
                        break;
                    default:
                        Notificar("<b>Erro!</b> Algo deu errado na operação", Notifica.Erro);
                        break;
                }
            }
            else
            {
                Notificar("<b>Erro!</b> Código inválido!", Notifica.Erro);
            }

            return RedirectToAction(nameof(Movimentar), new { id = idFigurino });
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR, ASSOCIADO")]
        public async Task<ActionResult> Movimentacoes()
        {
            var associado = await _pessoaService.GetByCpf(User.Identity?.Name);
            if (associado == null)
            {
                return RedirectToAction("Sair", "Identity");
            }

            var MovimentacoesFigurino = await _movimentacaoService.MovimentacoesByIdAssociadoAsync(associado.Id);

            return View(MovimentacoesFigurino);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR, ASSOCIADO")]
        public async Task<ActionResult> ConfirmarMovimentacao(int idMovimentacao)
        {
            var associado = await _pessoaService.GetByCpf(User.Identity?.Name);
            if (associado == null)
            {
                return RedirectToAction("Sair", "Identity");
            }
            string mensagem = string.Empty;
            switch (await _movimentacaoService.ConfirmarMovimentacao(idMovimentacao, associado.Id))
            {
                case HttpStatusCode.Created:
                    mensagem = "Empréstimo <b>Confirmado</b> com <b>Sucesso</b>";
                    Notificar(mensagem, Notifica.Sucesso);
                    break;
                case HttpStatusCode.OK:
                    mensagem = "Devolução <b>Confirmada</b> com <b>Sucesso</b>";
                    Notificar(mensagem, Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    mensagem = "<b>Erro</b>, O <b>Associado</b> não corresponde ao mesmo do <b>Empréstimo</b>";
                    Notificar(mensagem, Notifica.Erro);
                    break;
                case HttpStatusCode.FailedDependency:
                    mensagem = "<b>Erro</b>, O <b>Associado</b> não corresponde ao mesmo da <b>Devolução</b>";
                    Notificar(mensagem, Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    mensagem = "<b>Erro</b>, A <b>Movimentação</b> não existe !";
                    Notificar(mensagem, Notifica.Erro);
                    break;
                case HttpStatusCode.InternalServerError:
                    mensagem = "Erro ! Aconteceu um problema durante a confirmação.";
                    Notificar(mensagem, Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Movimentacoes));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> EditEstoque(int idFigurino, int idManequim)
        {
            var figurino = await _figurinoService.Get(idFigurino);
            var estoque = await _figurinoService.GetEstoque(idFigurino, idManequim);
            var manequins = _manequimService.GetAll();
            SelectList listManequins = new SelectList(manequins, "Id", "Tamanho");

            var estoqueviewmodel = new CreateEstoqueViewModel()
            {
                IdFigurino = figurino.Id,
                Nome = figurino.Nome,
                Data = figurino.Data.Value.ToString("dd/MM/yyyy"),
                listManequim = listManequins,
                QuantidadeDisponivel = estoque.Disponivel
            };
            return View(estoqueviewmodel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEstoque(CreateEstoqueViewModel estoque, int idFigurino, int idManequim)
        {
            var estoqueviewModel = new Figurinomanequim()
            {
                IdFigurino = idFigurino,
                IdManequim = idManequim,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel
            };

            HttpStatusCode resul = await _figurinoService.EditEstoque(estoqueviewModel);

            switch (resul)
            {
                case HttpStatusCode.OK:
                    Notificar($"<b>Sucesso!</b> Estoque foi <b>Editado</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("<b>Alerta!</b> Não há estoque", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro!</b> Algo deu errado", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro!</b> Algo deu errado na operação", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Estoque), new { id = estoque.IdFigurino });
        }

    }
}
