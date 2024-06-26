﻿using BilleSpace.Domain.Commands.Cities;
using BilleSpace.Domain.Queries;
using BilleSpace.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BilleSpace.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] LoadCitiesQuery query)
        {
            var result = _mediator.Send(query);
            return await result.Process();
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateCityCommand command)
        {
            var result = _mediator.Send(command);
            return await result.Process();
        }
    }
}
