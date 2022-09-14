using BilleSpace.Domain.Commands.Reservations;
using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleSpace.UnitTests.Reservations
{
    public class DeleteReservationTests
    {
        private async Task<BilleSpaceDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<BilleSpaceDbContext>()
                .UseInMemoryDatabase("InMemoryDbToTest")
                .Options;
            var databaseContext = new BilleSpaceDbContext(options, Options.Create(new OperationalStoreOptions()));
            databaseContext.Database.EnsureCreated();

            return databaseContext;
        }

        [Test]
        public async Task DeleteReservationTest_WithCorrectData_ReturnResultOk()
        {
            //Arrange
            var user = new User()
            {
                Id = "7177744e-025e-4bb0-993b-2b97737a7491",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var reservation = new Reservation()
            {
                Id = Guid.Parse("b7f4bc32-fb72-4e5a-9059-6fcd578ebcef"),
                OfficeId = Guid.Empty,
                Date = DateTime.UtcNow,
                UserId = user.Id,
                ParkingZoneId = Guid.Empty,
                OfficeZoneId = Guid.Empty,
                Office = new Office()
                {
                    Address = String.Empty,
                    Creator = user,
                    City = new City()
                    {
                        CountryId = Guid.Empty,
                        Country = new Country()
                        {
                            Id = Guid.Empty,
                            Name = "asd",
                            Symbol = "asd"
                        },
                        Id = Guid.Empty,
                        Name = "yes"
                    },
                    CityId = Guid.Empty,
                    Id = Guid.Empty,
                    OfficeMapUrl = String.Empty,
                    PostCode = String.Empty,
                    OfficeZones = new List<OfficeZone>()
                    {
                        new OfficeZone()
                        {
                            Desks = 12,
                            Id = Guid.Empty,
                            Name = "as"
                        }
                    },
                    ParkingZones = new List<ParkingZone>()
                    {
                        new ParkingZone()
                        {
                            Id = Guid.Empty,
                            Name = String.Empty,
                            Spaces = 12
                        }
                    }
                },
                OfficeDesk = String.Empty,
                OfficeZone = new OfficeZone()
                {
                    Desks = 12,
                    Id = Guid.Empty,
                    Name = "as"
                },
                ParkingSpace = String.Empty,
                ParkingZone = new ParkingZone()
                {
                    Id = Guid.Empty,
                    Name = "as",
                    Spaces = 12
                }
            };

            var request = new DeleteReservationCommand(reservation.Id, user.Id)
            {
            };

            var cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Reservations.RemoveRange(context.Reservations);
            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            Mock<ILogger<DeleteReservationCommandHandler>> mockLogger = new Mock<ILogger<DeleteReservationCommandHandler>>();

            var handler = new DeleteReservationCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Errors, Is.Null);
        }

        [Test]
        public async Task DeleteReservationTest_WhenReservationNotFoundInDb_ReturnResultNotFound()
        {
            //Arrange
            var user = new User()
            {
                Id = "df04bf8b-2031-4b49-9507-25bfb9c95aa5",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var request = new DeleteReservationCommand(Guid.Parse("b7f4bc32-fb72-4e5a-9059-6fcd578ebcef"), user.Id)
            {

            };

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Reservations.RemoveRange(context.Reservations);
            await context.SaveChangesAsync();

            Mock<ILogger<DeleteReservationCommandHandler>> mockLogger = new Mock<ILogger<DeleteReservationCommandHandler>>();

            var handler = new DeleteReservationCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(result.Code, Is.EqualTo(404));
            Assert.That(result.Errors[0], Is.EqualTo("There is no object with id: b7f4bc32-fb72-4e5a-9059-6fcd578ebcef"));
        }

    }
}
