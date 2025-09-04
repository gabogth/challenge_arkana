using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using transaction_application.Commands.CreateTransaction;
using transaction_application.Queries.GetTransaction;

namespace transaction_ms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<TransactionController> logger;
        public TransactionController(IMediator mediator, ILogger<TransactionController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <summary>
        /// Crea una nueva transacción.
        /// </summary>
        /// <remarks>
        /// Crea una transacción con estado inicial <c>pending</c> y publica el evento correspondiente.
        /// </remarks>
        /// <param name="command">Datos de la transacción.</param>
        /// <response code="201">Transacción creada.</response>
        /// <response code="400">Datos inválidos (validación).</response>
        /// <response code="500">Error interno.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Crear transacción", Description = "Crea una nueva transacción y devuelve su identificador y fecha de creación.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CreateTransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateTransactionDto>> CreateTransactionAsync([FromBody] CreateTransactionCommand command) 
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una transacción por su identificador externo.
        /// </summary>
        /// <param name="command">Parámetros de consulta (al menos el <c>transactionExternalId</c>).</param>
        /// <response code="200">Transacción encontrada.</response>
        /// <response code="404">No existe la transacción.</response>
        /// <response code="400">Parámetros inválidos.</response>
        /// <response code="500">Error interno.</response>
        [HttpGet]
        [SwaggerOperation(Summary = "Obtener transacción", Description = "Recupera una transacción por su transactionExternalId.")]
        [ProducesResponseType(typeof(GetTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetTransactionDto>> GetTransactionAsync([FromQuery]GetTransactionQuery command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
