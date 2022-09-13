using BilleSpace.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using BilleSpace.Infrastructure.Models;
using BilleSpace.Domain.Results;

namespace BilleSpace.Domain.Commands.Offices
{
    public class DeleteOfficeCommand : IRequest<Result>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CreatorId { get; set; }

        public DeleteOfficeCommand(Guid id, string creatorId)
        {
            Id = id;
            CreatorId = creatorId;
        }
    }
    public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand, Result>
    {
        private readonly BilleSpaceDbContext _dbContext;
        private readonly ILogger<DeleteOfficeCommandHandler> _logger;

        public DeleteOfficeCommandHandler(BilleSpaceDbContext dbContext, ILogger<DeleteOfficeCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogInformation($"[{DateTime.UtcNow}] Object {nameof(DeleteOfficeCommandHandler)} has been created.");
        }

        public async Task<Result> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
        {
            var office = await _dbContext.Offices
                .Include(x => x.OfficeZones)
                .Include(x => x.ParkingZones)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            // Validation
            if (office == null)
            {
                _logger.LogInformation($"[{DateTime.UtcNow}] Office given id doesn't exist.");
                return Result.NotFound(request.Id);
            }

            if (office.Creator.Id == request.CreatorId)
            {
                try
                {
                    _dbContext.OfficeZones.RemoveRange(office.OfficeZones);
                    _dbContext.ParkingZones.RemoveRange(office.ParkingZones);
                    _dbContext.Offices.Remove(office);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[{DateTime.UtcNow}] Something goes wrong during saving changes in database.");
                    _logger.LogError($"[{DateTime.UtcNow}] {ex.Message}");
                    return Result.BadRequest<OfficeModel>(new List<string>() { $"Error occurred while saving changes to database." });
                }
            }
            else
            {
                _logger.LogInformation($"[{DateTime.UtcNow}] You're not authorized to delete offices");
                return Result.BadRequest<OfficeModel>(new List<string> { "You're not authorized to delete offices" });
            }
        }
    }
}