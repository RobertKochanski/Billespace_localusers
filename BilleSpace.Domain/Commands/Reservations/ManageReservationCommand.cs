﻿using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using BilleSpace.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace BilleSpace.Domain.Commands.Reservations
{
    public class ManageReservationCommand : IRequest<Result<ReservationModel>>
    {

        [JsonIgnore]
        public Guid Id { get; set; }
        public Guid OfficeId { get; set; }
        public Guid OfficeZoneId { get; set; }
        public Guid? ParkingZoneId { get; set; }
        public DateTime Date { get; set; }
        public string OfficeDesk { get; set; }
        public string? ParkingSpace { get; set; }
        [JsonIgnore]
        public string? UserId { get; set; }
    }

    public class ManageReservationCommandHandler : IRequestHandler<ManageReservationCommand, Result<ReservationModel>>
    {
        private readonly BilleSpaceDbContext _dbContext;
        private readonly ILogger<ManageReservationCommandHandler> _logger;

        public ManageReservationCommandHandler(BilleSpaceDbContext dbContext, ILogger<ManageReservationCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogInformation($"[{DateTime.UtcNow}] Object '{nameof(ManageReservationCommandHandler)}' has been created.");
        }

        public async Task<Result<ReservationModel>> Handle(ManageReservationCommand request, CancellationToken cancellationToken)
        {
            var isAdding = request.Id == Guid.Empty;
            Reservation? reservation = null;

            var office = await _dbContext.Offices
                .Include(cit => cit.City)
                    .ThenInclude(cou => cou.Country)
                .Include(ofz => ofz.OfficeZones)
                .Include(par => par.ParkingZones)
                .FirstOrDefaultAsync(off => off.Id == request.OfficeId);

            var officeZone = await _dbContext.OfficeZones.FirstOrDefaultAsync(x => x.Id == request.OfficeZoneId);

            ParkingZone? parkingZone = null;
            if (request.ParkingZoneId != null)
            {
                parkingZone = await _dbContext.ParkingZones.FirstOrDefaultAsync(x => x.Id == request.ParkingZoneId);
            }

            var errorMessages = new List<string>();
            if (office == null)
            {
                errorMessages.Add($"[{DateTime.UtcNow}] Office with id: {request.OfficeId} does not exist.");
            }
            if (officeZone == null)
            {
                errorMessages.Add($"[{DateTime.UtcNow}] OfficeZone with id: {request.OfficeZoneId} does not exist.");
            }
            if (parkingZone == null && request.ParkingZoneId != null)
            {
                errorMessages.Add($"[{DateTime.UtcNow}] ParkingZone with id: {request.ParkingZoneId} does not exist.");
            }
            if (errorMessages.Count > 0)
            {
                _logger.LogError(string.Join(Environment.NewLine, errorMessages));
                return Result.BadRequest<ReservationModel>(errorMessages);
            }


            if (isAdding)
            {
                reservation = new Reservation()
                {
                    Date = request.Date,
                    Office = office,
                    OfficeZone = officeZone,
                    ParkingZone = parkingZone,
                    OfficeDesk = request.OfficeDesk,
                    ParkingSpace = request.ParkingSpace,
                    UserId = request.UserId,
                };

                if (request.ParkingZoneId != null)
                {
                    reservation.ParkingZoneId = request.ParkingZoneId;
                }

                var reservationFromDb = await _dbContext.Reservations
                    .FirstOrDefaultAsync(res => res.Date.Date == reservation.Date.Date &&
                                  res.OfficeId == reservation.OfficeId &&
                                  res.OfficeZoneId == reservation.OfficeZoneId &&
                                  res.ParkingZoneId == reservation.ParkingZoneId &&
                                  res.ParkingSpace == reservation.ParkingSpace ||
                                  res.OfficeDesk == reservation.OfficeDesk,
                                  cancellationToken);

                if (reservationFromDb != null)
                {
                    return Result.BadRequest<ReservationModel>(new List<string>() { "This seat is already reserved." });
                }

                try
                {
                    await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation($"[{DateTime.UtcNow}] Add new reservation.");
                }
                catch (Exception e)
                {
                    _logger.LogError($"[{DateTime.UtcNow}] Can't add new reservation:");
                    _logger.LogError(e.Message);
                    return Result.BadRequest<ReservationModel>(new List<string>() { "Can't add new reservation." });
                }
            }
            else
            {
                reservation = await _dbContext.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

                if (reservation == null)
                {
                    return Result.BadRequest<ReservationModel>(new List<string>() { $"Reservation with id: {request.Id} does not exist." });
                }

                if (reservation.UserId != request.UserId)
                {
                    _logger.LogError($"[{DateTime.UtcNow}] User with {request.UserId} Id can not edit this reservation!");
                    return Result.Forbidden<ReservationModel>(new List<string>() { $"[{DateTime.UtcNow}] User with {request.UserId} Id can not edit this reservation!" });
                }

                reservation.Date = request.Date;
                reservation.Office = office;
                reservation.OfficeZone = officeZone;
                reservation.OfficeDesk = request.OfficeDesk;
                reservation.ParkingZone = parkingZone;
                reservation.ParkingSpace = request.ParkingSpace;

                try
                {
                    _dbContext.Reservations.Update(reservation);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation($"[{DateTime.UtcNow}] Update reservation (id: {request.Id}).");
                }
                catch (Exception e)
                {
                    _logger.LogError($"[{DateTime.UtcNow}] Can't edit reservation (id: {request.Id}):");
                    _logger.LogError(e.Message);
                    return Result.BadRequest<ReservationModel>(new List<string>() { "Can't edit reservation." });
                }

            }

            var model = new ReservationModel()
            {
                Id = reservation.Id,
                Date = reservation.Date,
                Office = new OfficeModel()
                {
                    Id = reservation.OfficeId,
                    Address = reservation.Office.Address,
                    PostCode = reservation.Office.PostCode,
                    OfficeMapUrl = reservation.Office.OfficeMapUrl,
                    City = new CityModel()
                    {
                        Id = reservation.Office.CityId,
                        Name = reservation.Office.City.Name,
                        Country = new CountryModel()
                        {
                            Id = reservation.Office.City.CountryId,
                            Name = reservation.Office.City.Country.Name,
                            Symbol = reservation.Office.City.Country.Symbol
                        }
                    },
                    OfficeZones = reservation.Office.OfficeZones.Select(x => new OfficeZoneModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Desks = x.Desks
                    }),
                    ParkingZones = reservation.Office.ParkingZones.Select(x => new ParkingZoneModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Spaces = x.Spaces
                    }),
                },
                OfficeZone = new OfficeZoneModel()
                {
                    Id = reservation.OfficeZoneId,
                    Name = reservation.OfficeZone.Name,
                    Desks = reservation.OfficeZone.Desks
                },
                OfficeDesk = reservation.OfficeDesk,
                ParkingZone = reservation.ParkingZoneId != null ? new ParkingZoneModel()
                {
                    Id = reservation.ParkingZone.Id,
                    Name = reservation.ParkingZone.Name,
                    Spaces = reservation.ParkingZone.Spaces
                } : null,
                ParkingSpace = reservation.ParkingSpace
            };

            return Result.Ok(model);
        }
    }
}
