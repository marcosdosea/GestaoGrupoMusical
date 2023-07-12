using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GestaoGrupoMusicalWeb.Controllers.BaseController;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class PessoaController : BaseController
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IManequimService _manequim;

        public PessoaController (IPessoaService pessoaService, IMapper mapper, IGrupoMusicalService grupoMusical, IManequimService manequim)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _manequim = manequim;
        }

        // GET: PessoaController
        public ActionResult Index()
        {
            var listaPessoasDTO = _pessoaService.GetAllAssociadoDTO();

            return View(listaPessoasDTO);
        }

        // GET: PessoaController/Details/5
        public ActionResult Details(int id)
        {
            var pessoa = _pessoaService.Get(id);
            PessoaViewModel pessoaModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaModel);
        }

        // GET: PessoaController/Create
        public ActionResult Create()
        {
            PessoaViewModel pessoaViewModel = new PessoaViewModel();

            IEnumerable<Papelgrupo> listaPapelGrupo = _pessoaService.GetAllPapelGrupo();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", null);
            pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", null);
            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PessoaViewModel pessoaViewModel)
        {
            pessoaViewModel.Cpf = pessoaViewModel.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
            pessoaViewModel.Cep = pessoaViewModel.Cep.Replace("-", string.Empty);

            IEnumerable<Papelgrupo> listaPapelGrupo = _pessoaService.GetAllPapelGrupo();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            if (ModelState.IsValid)
            {
                var pessoaModel = _mapper.Map<Pessoa>(pessoaViewModel);
                String mensagem = String.Empty;
              

                switch (await _pessoaService.Create(pessoaModel))
                {
                    case 200:
                        Notificar("Associado <b>Cadastrado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 500:
                        Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        return RedirectToAction(nameof(Index));
                    case 400:
                        mensagem = "<b>Alerta<b>,não foi possível cadastrar, a data de entrada deve ser menor que " + DateTime.Now;
                        Notificar(mensagem, Notifica.Alerta);
                       
                        pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", pessoaViewModel.IdGrupoMusical);
                        pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", pessoaViewModel.IdPapelGrupo);
                        pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);
                        return View("Create", pessoaViewModel);
                    case 401:
                        mensagem = "<b>Alerta<b>, não foi possível cadastrar, a data de nascimento deve ser menor que " + DateTime.Now + " e menor que 120 anos ";
                        Notificar(mensagem, Notifica.Alerta);
                        pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", pessoaViewModel.IdGrupoMusical);
                        pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", pessoaViewModel.IdPapelGrupo);
                        pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);
                        return View("Create", pessoaViewModel);

                }

            }
            else
            {          
                pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", null);
                pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", null);
                pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);
                return View("Create", pessoaViewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Edit/5
        public ActionResult Edit(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            IEnumerable<Papelgrupo> listaPapelGrupo = _pessoaService.GetAllPapelGrupo();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", pessoaViewModel.IdGrupoMusical);
            pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", pessoaViewModel.IdPapelGrupo);
            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);



            return View(pessoaViewModel);
        }

        // POST: PessoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PessoaViewModel pessoaViewModel)
        {
            pessoaViewModel.Cpf = pessoaViewModel.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
            pessoaViewModel.Cep = pessoaViewModel.Cep.Replace("-", string.Empty);
            var cpf = _pessoaService.GetCPFExistente(id,pessoaViewModel.Cpf);
            
            if(cpf)
            {
                ModelState.Remove("Cpf");
            }

            if (ModelState.IsValid)
            {
                var pessoa = _mapper.Map<Pessoa>(pessoaViewModel);
                _pessoaService.Edit(pessoa);
            }
            else
            {
                IEnumerable<Papelgrupo> listaPapelGrupo = _pessoaService.GetAllPapelGrupo();
                IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
                IEnumerable<Manequim> listaManequim = _manequim.GetAll();

                pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", null);
                pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", null);
                pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);
                return View("Edit", pessoaViewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Delete/5
        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PessoaViewModel pessoaViewModel)
        {
            return RedirectToAction(nameof(Index));
        }

        public ActionResult RemoveAssociado(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);
            return View(pessoaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAssociado(int id, PessoaViewModel pessoaViewModel)
        {
            var pessoassociada = _pessoaService.Get(id);
            _pessoaService.RemoverAssociado(pessoassociada, pessoaViewModel.MotivoSaida);
            return RedirectToAction(nameof(Index));
        }

    }
}
