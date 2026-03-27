using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service;


namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ASSOCIADO, ADMINISTRADOR_DO_GRUPO_MUSICAL")]
    public class FinanceiroController : ControllerBase
    {
        private readonly IFinanceiroService financeiroService;
        private readonly IMapper mapper;


        public FinanceiroController(IFinanceiroService financeiroService, IMapper mapper)
        {
            this.financeiroService = financeiroService;
            this.mapper = mapper;
        }

        // GET: api/<FinanceiroController>
        [HttpGet]
        public ActionResult Get()
        {
            var idGrupoClaim = User.Claims.FirstOrDefault(c => c.Type == "IdGrupoMusical")?.Value;

            if (idGrupoClaim == null) return Unauthorized("Grupo musical não identificado.");

            int id = int.Parse(idGrupoClaim);

            var listafinanceiro = financeiroService.GetAllFinanceiroPorIdGrupo(id);

            return Ok(listafinanceiro);
        }

        // GET api/<FinanceiroController>/5
        [HttpGet("{id}")]
        public ActionResult GetAsync(int id)
        {
            var pagamento = financeiroService.Get(id);

            if (pagamento == null)
            {
                return NotFound();
            }
            return Ok(pagamento);
        }

        // POST api/<FinanceiroController>
        [HttpPost]
        public ActionResult Post([FromBody] FinanceiroCreateDTO financeiro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var status = financeiroService.Create(financeiro);

            if (status == FinanceiroStatus.Success) return Ok();

            return BadRequest(status.ToString());
        }

        [HttpGet("associado")]
        public async Task<ActionResult<IEnumerable<FinanceiroMobileDTO>>> GetPagamentosDoAssociado()
        {
            try
            {
                int idPessoa = Convert.ToInt32(User.FindFirst("IdPessoa")?.Value);
                var pagamentos = await financeiroService.GetPagamentosDoAssociadoAsync(idPessoa);
                if (pagamentos == null || !pagamentos.Any())
                {
                    return Ok(new List<FinanceiroMobileDTO>());

                }
                return Ok(pagamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

    }
}
