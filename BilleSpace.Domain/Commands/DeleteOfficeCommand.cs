using BilleSpace.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using BilleSpace.Infrastructure.Models;
using BilleSpace.Domain.Results;

namespace BilleSpace.Domain.Commands
{
    public class DeleteOfficeCommand : IRequest<Result>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string? AuthorNameIdentifier { get; set; }
        public string UserNameIdentifier { get; set; }

        public DeleteOfficeCommand(Guid id, string userNameIdentifier)
        {
            Id = id;
            UserNameIdentifier = userNameIdentifier;
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
            var office = await _dbContext.Offices.FirstOrDefaultAsync(x => x.Id == request.Id);
            // Validation
            List<string> errorMessageS = new List<string>();

            if (office == null)
            {
                _logger.LogInformation($"[{DateTime.UtcNow}] Office given id doesn't exist.");
                return Result.NotFound(request.Id);
            }

            if (office.AuthorNameIdentifier == request.UserNameIdentifier)
            {
                try
                {
                    _dbContext.Offices.Remove(office);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"[{DateTime.UtcNow}] Office with {office.Id} was removed.");
                    return Result.BadRequest<OfficeModel>(new List<string>() { $"Error occurred while saving changes to database." });
                }

            }
            else
            {
                _logger.LogInformation($"[{DateTime.UtcNow}] you're not authorized to delete offices");
                return Result.BadRequest<OfficeModel>(errorMessageS);
            }


            return Result.Ok();
        }
    }

}