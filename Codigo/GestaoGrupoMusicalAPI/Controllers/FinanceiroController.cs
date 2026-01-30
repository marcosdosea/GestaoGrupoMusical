using AutoMapper;
using Core.Service;
using Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var listaFinanceiro = financeiroService.GetAllFinanceiroPorIdGrupo(5);
            return Ok();
        }

        // GET api/<FinanceiroController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var pagamento = financeiroService.GetAssociadosPagamento(id);

            if(pagamento == null)
            {
                return BadRequest();
            }
            return Ok(pagamento);
        }

        // POST api/<FinanceiroController>
        [HttpPost]
        public ActionResult Post([FromBody] FinanceiroCreateDTO financeiro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            return Ok();
        }

    }
}
