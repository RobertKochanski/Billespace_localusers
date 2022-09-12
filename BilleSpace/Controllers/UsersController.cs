using BilleSpace.Domain.Commands;
using BilleSpace.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BilleSpace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ManageUserCommand command)
        {
            return await _mediator.Send(command).Process();
        }
    }
}
