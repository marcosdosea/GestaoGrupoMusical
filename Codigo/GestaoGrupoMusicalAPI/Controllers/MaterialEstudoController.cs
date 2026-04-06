using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialEstudoController : Controller
    {

        private readonly IMaterialEstudoService material;
        private readonly IMapper mapper;

        public MaterialEstudoController(IMaterialEstudoService material, IMapper mapper)
        {
            this.material = material;
            this.mapper = mapper;
        }

        // GET: MaterialEstudoController
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {

            var idGrupoClaim = User.Claims.FirstOrDefault(c => c.Type == "IdGrupoMusical")?.Value;

            if (idGrupoClaim == null) return Unauthorized("Grupo não indentificado");

            int idGrupo = int.Parse(idGrupoClaim);

            var listaMaterialEstudo = await material.GetAllMaterialEstudoPerIdGrupo(idGrupo);

            if(listaMaterialEstudo == null) return NotFound("Lista de Materias de Estudo vázia");
  
            return Ok(listaMaterialEstudo);
        }

    }
}
