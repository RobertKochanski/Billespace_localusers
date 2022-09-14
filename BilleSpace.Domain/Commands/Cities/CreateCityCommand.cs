using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using BilleSpace.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleSpace.Domain.Commands.Cities
{
    public class CreateCityCommand : IRequest<Result<CityModel>>
    {
        public string Name { get; set; }
        public CountryModel Country { get; set; }
    }

    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityModel>>
    {
        private readonly BilleSpaceDbContext _context;
        private readonly ILogger<CreateCityCommandHandler> _logger;

        public CreateCityCommandHandler(BilleSpaceDbContext context, ILogger<CreateCityCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation($"[{DateTime.UtcNow}] Object '{nameof(CreateCityCommandHandler)}' has been created.");
        }

        public async Task<Result<CityModel>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            City? city = null;
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == request.Country.Name);
            var cityFromDb = await _context.Cities.FirstOrDefaultAsync(x => x.Name == request.Name && x.Country.Name == request.Country.Name);

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Country.Name) || string.IsNullOrEmpty(request.Country.Symbol))
            {
                _logger.LogError($"[{DateTime.UtcNow}] Fill missing fields!");
                return Result.BadRequest<CityModel>(new List<string>() { $"[{DateTime.UtcNow}] Fill missing fields!" });
            }

            if (cityFromDb != null)
            {
                _logger.LogError($"[{DateTime.UtcNow}] City already exists!");
                return Result.BadRequest<CityModel>(new List<string>() { $"[{DateTime.UtcNow}] City already exists!" });
            }

            if (country != null)
            {
                city = new City()
                {
                    Name = request.Name,
                    Country = country
                };
            }
            else
            {
                city = new City()
                {
                    Name = request.Name,
                    Country = new Country()
                    {
                        Name = request.Country.Name,
                        Symbol = request.Country.Symbol,
                    }
                };
            }
            
            try
            {
                await _context.Cities.AddAsync(city);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"[{DateTime.UtcNow}] City was created.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Something goes wrong during saving changes in database.");
                _logger.LogError($"[{DateTime.UtcNow}] {ex.Message}");
                return Result.BadRequest<CityModel>(new List<string>() { $"Error occurred while saving changes to database." });
            }

            var response = new CityModel()
            {
                Name = request.Name,
                Country = request.Country
            };

            return Result.Ok(response);
        }
    }
}
