using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ASSOCIADO, ADMINISTRADOR GRUPO")] 
    public class FinanceiroController : ControllerBase
    {
        private readonly IFinanceiroService financeiroService;
        private readonly IMapper mapper;

        public FinanceiroController(IFinanceiroService financeiroService, IMapper mapper)
        {
            this.financeiroService = financeiroService;
            this.mapper = mapper;
        }

        // Rota: GET api/Financeiro

        [HttpGet]
        public ActionResult Get()
        {
            var idGrupoClaim = User.Claims.FirstOrDefault(c => c.Type == "IdGrupoMusical")?.Value;

            if (idGrupoClaim == null) return Unauthorized("Grupo musical não identificado.");

            int id = int.Parse(idGrupoClaim);

            // Esse método do seu service já traz Pagos, Isentos, Atrasos e Recebido!
            var listafinanceiro = financeiroService.GetAllFinanceiroPorIdGrupo(id);

            return Ok(listafinanceiro);
        }

        // Rota: GET api/Financeiro/5/associados

        [HttpGet("{idReceita}/associados")]
        public async Task<ActionResult<IEnumerable<AssociadoPagamentoDTO>>> GetAssociadosDoPagamento(int idReceita)
        {
            try
            {
                // Chama o método que você já criou no FinanceiroService
                var associados = await financeiroService.GetAssociadosPagamento(idReceita);

                if (associados == null || !associados.Any())
                {
                    return Ok(new List<AssociadoPagamentoDTO>());
                }
                return Ok(associados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // GET api/Financeiro/associado
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

        [HttpPost]
        public ActionResult Post([FromBody] FinanceiroCreateDTO financeiro)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var status = financeiroService.Create(financeiro);

            if (status == FinanceiroStatus.Success) return Ok();

            return BadRequest(status.ToString());
        }
    }
}