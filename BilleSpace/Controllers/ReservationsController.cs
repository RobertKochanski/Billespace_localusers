using BilleSpace.Domain.Commands.Reservations;
using BilleSpace.Domain.Queries;
using BilleSpace.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BilleSpace.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostReservation(ManageReservationCommand command)
        {
            command.Id = Guid.Empty;
            command.UserId = User.Identity.Name;
            var result = _mediator.Send(command);
            return await result.Process();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation([FromRoute]Guid id, ManageReservationCommand command)
        {
            command.Id = id;
            command.UserId = User.Identity.Name;
            var result = _mediator.Send(command);
            return await result.Process();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            var result = _mediator.Send(new DeleteReservationCommand(id, User.Identity.Name));
            return await result.Process();
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var result = _mediator.Send(new LoadReservationsQuery(User.Identity.Name));
            return await result.Process();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationDetails(Guid id)
        {
            var result = _mediator.Send(new LoadReservationDetailsQuery(id));
            return await result.Process();
        }
    }
}
