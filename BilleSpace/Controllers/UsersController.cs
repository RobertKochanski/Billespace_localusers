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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            return await _mediator.Send(command).Process();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            return await _mediator.Send(command).Process();
        }
    }
}
