using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class EnsaioController : BaseController
    {
        private readonly IEnsaioService _ensaio;
        private readonly IMapper _mapper;
        private readonly IPessoaService _pessoa;
        private readonly IGrupoMusicalService _grupoMusical;

        public EnsaioController(IMapper mapper, IEnsaioService ensaio, IPessoaService pessoa, IGrupoMusicalService grupoMusical)
        {
            _ensaio = ensaio;
            _mapper = mapper;
            _pessoa = pessoa;
            _grupoMusical = grupoMusical;
        }

        // GET: EnsaioController
        public async Task<ActionResult> Index()
        {
            var ensaios = await _ensaio.GetAllIndexDTO();
            return View(ensaios);
        }

        // GET: EnsaioController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        // GET: EnsaioController/Create
        public ActionResult Create()
        {
            EnsaioViewModel ensaioModel = new();

            ensaioModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            ensaioModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");

            return View(ensaioModel);
        }

        // POST: EnsaioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EnsaioViewModel ensaioViewModel)
        {
            ensaioViewModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            ensaioViewModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");
            if (ModelState.IsValid)
            {
                String mensagem = String.Empty;
                switch(await _ensaio.Create(_mapper.Map<Ensaio>(ensaioViewModel)))
                {
                    case 200:
                        mensagem = "Ensaio <b>Cadastrado</b> com <b>Sucesso</b>";
                        Notificar(mensagem, Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 400:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser menor que a data de <b>fim</b>";
                        Notificar(mensagem, Notifica.Alerta);
                        return View("Create", ensaioViewModel);
                    case 401:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser maior que a data de hoje " + DateTime.Now;
                        Notificar(mensagem, Notifica.Alerta);
                        return View("Create", ensaioViewModel);
                    case 500:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> de ensaio, se isso persistir entre em contato com o suporte";
                        Notificar(mensagem, Notifica.Erro);
                        return RedirectToAction("Create", ensaioViewModel);

                }
            }

            return View(ensaioViewModel);
        }

        // GET: EnsaioController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ensaio = await _ensaio.Get(id);
            EnsaioViewModel ensaioModel = _mapper.Map<EnsaioViewModel>(ensaio);

            ensaioModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            ensaioModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");

            return View(ensaioModel);
        }

        // POST: EnsaioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnsaioViewModel ensaioViewModel)
        {
            ensaioViewModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            ensaioViewModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");
            if (ModelState.IsValid)
            {
                String mensagem = String.Empty;
                switch (await _ensaio.Edit(_mapper.Map<Ensaio>(ensaioViewModel)))
                {
                    case 200:
                        mensagem = "Ensaio <b>Editado</b> com <b>Sucesso</b>";
                        Notificar(mensagem, Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 401:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser menor que a data de <b>a data de fim</b>, ou a <b>a data de fim</b> tem que ser maior";
                        Notificar(mensagem, Notifica.Alerta);
                        return View("Edit", ensaioViewModel);
                    case 400:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser maior que a data de hoje " + DateTime.Now;
                        Notificar(mensagem, Notifica.Alerta);
                        return View("Edit", ensaioViewModel);
                    case 500:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Editar</b> de ensaio, se isso persistir entre em contato com o suporte";
                        Notificar(mensagem, Notifica.Erro);
                        return RedirectToAction("Edit", ensaioViewModel);


                }
            }
            return View(ensaioViewModel);
        }

        // GET: EnsaioController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        // POST: EnsaioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(EnsaioViewModel ensaioModel)
        {
            String mensagem = String.Empty;
            switch (await _ensaio.Delete(ensaioModel.Id))
            {
                case 200:
                    mensagem = "Ensaio <b>Deletado</b> com <b>Sucesso</b>";
                    Notificar(mensagem, Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                case 500:
                    mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante ao <b>Excluir</b> um ensaio, se isso persistir entre em contato com o suporte";
                    Notificar(mensagem, Notifica.Erro);
                    return RedirectToAction(nameof(Index));

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
