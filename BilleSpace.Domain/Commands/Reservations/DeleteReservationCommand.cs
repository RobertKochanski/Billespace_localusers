using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace BilleSpace.Domain.Commands.Reservations
{
    public class DeleteReservationCommand : IRequest<Result>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }

        public DeleteReservationCommand(Guid id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class DeleteReservationCommandHandler : IRequestHandler<DeleteReservationCommand, Result>
    {
        private readonly BilleSpaceDbContext _dbContext;
        private readonly ILogger<DeleteReservationCommandHandler> _logger;

        public DeleteReservationCommandHandler(BilleSpaceDbContext dbContext, ILogger<DeleteReservationCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogInformation($"[{DateTime.UtcNow}] Object '{nameof(DeleteReservationCommandHandler)}' has been created.");
        }

        public async Task<Result> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (reservation == null)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Can't find reservation (id: {request.Id}).");
                return Result.NotFound(request.Id);
            }

            if (reservation.UserId != request.UserId)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Can not delete not your booking!");
                return Result.BadRequest($"[{DateTime.UtcNow}] Can not delete not your booking!");
            }

            try
            {
                _dbContext.Reservations.Remove(reservation);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"[{DateTime.UtcNow}] Reservation (id: {request.Id}) was deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Error while deleting reservation (id: {request.Id}):");
                _logger.LogError(e.Message);
                return Result.BadRequest("Error while deleting reservation.");
            }

            return Result.Ok();
        }
    }
}
