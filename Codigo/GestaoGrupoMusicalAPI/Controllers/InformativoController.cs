using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Mvc;


namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformativoController : ControllerBase
    {
        private readonly IInformativoService informativo;
        private readonly IMapper mapper;

        public InformativoController(IInformativoService informativo, IMapper mapper)
        {
            this.informativo = informativo;
            this.mapper = mapper;
        }

        // GET: api/<InformativoController>
        [HttpGet("Grupo")]
        public ActionResult Get()
        {
            var idGrupoClaim = User.Claims.FirstOrDefault(c => c.Type == "IdGrupoMusical")?.Value;

            if (idGrupoClaim == null) return Unauthorized("Grupo musical não identificado.");

            int idGrupo = int.Parse(idGrupoClaim);

            var listaInformativos = informativo.GetAllInformativoServicePorIdGrupoMusical(idGrupo);

            if (listaInformativos == null) return NotFound("Nenhum informativo no grupo");

            return Ok(listaInformativos);
        }


    }
}
