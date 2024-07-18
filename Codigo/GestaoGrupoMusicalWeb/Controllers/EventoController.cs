﻿using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Core.Datatables;
using NuGet.Protocol;
using Core.DTO;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : BaseController
    {
        private readonly IEventoService _evento;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IPessoaService _pessoa;
        private readonly IFigurinoService _figurino;


        public EventoController(IEventoService evento, IMapper mapper, IGrupoMusicalService grupoMusical, IPessoaService pessoa, IFigurinoService figurino)
        {
            _evento = evento;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _pessoa = pessoa;
            _figurino = figurino;
        }

        // GET: EventoController
        public ActionResult Index()
        {
            var listaEvento = _evento.GetAllIndexDTO();
            return View(listaEvento);
        }

        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var response = _evento.GetDataPage(request, await _grupoMusical.GetIdGrupo(User.Identity.Name));
            return Json(response);
        }

        // GET: EventoController/Details/5
        public ActionResult Details(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            return View(eventoModel);
        }

        // GET: EventoController/Create
        public async Task<ActionResult> Create()
        {

            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = await _pessoa.GetRegentesForAutoCompleteAsync(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var figurinosDropdown = await _figurino.GetAllFigurinoDropdown(idGrupoMusical);

            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("######## Lista de pessoas ########");
            foreach (var f in listaPessoasAutoComplete)
            {
                Console.WriteLine(f.Id + " | " + f.Nome);
            }

            Console.WriteLine("######## Lista de Figurinos ########");
            foreach (var f in figurinosDropdown)
            {
                Console.WriteLine(f.Id + " | " + f.Nome);
            }
            EventoCreateViewlModel eventoModelCreate = new()
            {
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome")
            };

            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            eventoModelCreate.JsonLista = listaPessoasAutoComplete.ToJson();
            return View(eventoModelCreate);
        }

        // POST: EventoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventoCreateViewlModel eventoModel)
        {
            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);
            if (ModelState.IsValid && eventoModel.IdRegentes != null)
            {
                eventoModel.IdGrupoMusical = idGrupoMusical;
                Console.WriteLine("#########################################");
                Console.WriteLine("SEGUNDO");
                //EventoCreateDTO eventoCreateDTO = _mapper.Map<EventoCreateDTO>(eventoModel);
                Evento evento = _mapper.Map<Evento>(eventoModel);
                int auxId = (await _pessoa.GetByCpf(User.Identity.Name))?.Id ?? 0;
                if (auxId != 0 && eventoModel.IdFigurinoSelecionado != 0)
                {
                    evento.Id = auxId;
                    foreach (int i in eventoModel.IdRegentes!)
                    {
                        Console.WriteLine("Regentes: " + i);
                    }

                    Console.WriteLine("Figurino: " + eventoModel.IdFigurinoSelecionado);
                    Console.WriteLine("Local: " + eventoModel.Local);
                    Console.WriteLine("Repositorio: " + eventoModel.Repertorio);
                    string mensagem = "";
                    switch (await _evento.Create(evento,eventoModel.IdRegentes,eventoModel.IdFigurinoSelecionado))
                    {
                        case HttpStatusCode.OK:
                            Notificar("Ensaio <b>Cadastrado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                            return RedirectToAction(nameof(Index));
                        case HttpStatusCode.BadRequest:
                            mensagem = "Alerta! A <b>data de início</b> deve ser maior que a data de <b>hoje</b>";
                            Notificar(mensagem, Notifica.Alerta);
                            break;
                        case HttpStatusCode.PreconditionFailed:
                            mensagem = "Alerta! A data de <b>início</b> deve ser maior que a data <b>fim</b> " + DateTime.Now;
                            Notificar(mensagem, Notifica.Alerta);
                            break;
                        default:
                            mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro durante o <b>Cadastro</b> de ensaio.";
                            Notificar(mensagem, Notifica.Erro);
                            break;
                    }
                }
                else
                {
                    Notificar("<b>Erro</b>! Há algo errado ao cadastrar um novo Evento", Notifica.Erro);
                }
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado ao cadastrar um novo Evento", Notifica.Erro);
            }
           

            var listaPessoasAutoComplete = await _pessoa.GetRegentesForAutoCompleteAsync(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var figurinosDropdown = await _figurino.GetAllFigurinoDropdown(idGrupoMusical);

            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("######## Lista de pessoas ########");
            foreach (var f in listaPessoasAutoComplete)
            {
                Console.WriteLine(f.Id + " | " + f.Nome);
            }

            Console.WriteLine("######## Lista de Figurinos ########");
            foreach (var f in figurinosDropdown)
            {
                Console.WriteLine(f.Id + " | " + f.Nome);
            }
            EventoCreateViewlModel eventoModelCreate = new()
            {
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome")
            };

            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            eventoModelCreate.JsonLista = listaPessoasAutoComplete.ToJson();
            return View(eventoModel);
        }

        // GET: EventoController/Edit/5
        public ActionResult Edit(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");

            return View(eventoModel);
        }

        // POST: EventoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventoViewModel eventoModel)
        {
            if (ModelState.IsValid)
            {
                var evento = _mapper.Map<Evento>(eventoModel);
                _evento.Edit(evento);
                return RedirectToAction(nameof(Index));
            }
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            return RedirectToAction("Edit", eventoModel);
        }

        // GET: EventoController/Delete/5
        public ActionResult Delete(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            return View(eventoModel);
        }

        // POST: EventoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, EventoViewModel eventoViewModel)
        {
            _evento.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> NotificarEventoViaEmail(int id)
        {
            var pessoas = await _grupoMusical.GetAllPeopleFromGrupoMusical(await _grupoMusical.GetIdGrupo(User.Identity.Name));
            switch (_evento.NotificarEventoViaEmail(pessoas, id))
            {
                case HttpStatusCode.OK:
                    Notificar("Notificação de Evento foi <b>Enviada</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("O Evento <b>Não</b> está <b>Cadastrado</b> no sistema.", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar($"O evento {id} <b>não foi encontrado</b>.", Notifica.Erro);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("Houve um erro. Tente novamente mais tarde.", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Envio</b> da notificação.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
