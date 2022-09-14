using BilleSpace.Domain.Queries;
using BilleSpace.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BilleSpace.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]LoadCountriesQuery query)
        {
            var result = _mediator.Send(query);
            return await result.Process();
        }
    }
}
