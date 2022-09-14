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
    public class CreateReservationTests
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
                Id = "9ccc3e52-cd9f-49a7-8ded-a39815b30d18",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var office = new Office()
            {
                Id = Guid.Parse("d2dae82c-aeed-4135-a547-d8154b24b43e"),
                Address = "address",
                Creator = user,
                City = new City()
                {
                    Id = Guid.Parse("897a77ae-661b-437d-9e68-c04764e3a026"),
                    Country = new Country()
                    {
                        Id = Guid.Parse("b1c40f73-bf83-46fb-858c-089b13ce6b5a"),
                        Name = "asd",
                        Symbol = "asd"
                    },
                    Name = "yes"
                },
                OfficeMapUrl = string.Empty,
                PostCode = "address",
                OfficeZones = new List<OfficeZone>()
                {
                    new OfficeZone()
                    {
                        Desks = 12,
                        Id = Guid.Parse("a764d968-863a-46e8-96c9-aec8363cb9e4"),
                        Name = "as"
                    }
                },
                ParkingZones = new List<ParkingZone>()
                {
                    new ParkingZone()
                    {
                        Id = Guid.Parse("5ced5abc-5ffd-4b6a-9fdb-395cbbce5772"),
                        Name = string.Empty,
                        Spaces = 12
                    }
                }
            };

            var reservation = new Reservation()
            {
                Id = Guid.Parse("b7f4bc32-fb72-4e5a-9059-6fcd578ebcef"),
                Date = DateTime.UtcNow,
                UserId = user.Id,
                Office = office,
                OfficeDesk = "2",
                ParkingSpace = "3",
                OfficeZone = office.OfficeZones[0],
                ParkingZone = office.ParkingZones[0],
            };

            var request = new ManageReservationCommand()
            {
                Id = Guid.Empty,
                UserId = reservation.UserId,
                Date = reservation.Date,
                OfficeDesk = reservation.OfficeDesk,
                ParkingSpace = reservation.ParkingSpace,
                OfficeId = reservation.Office.Id,
                OfficeZoneId = reservation.OfficeZone.Id,
                ParkingZoneId = reservation.ParkingZone.Id,
            };

            var cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Reservations.RemoveRange(context.Reservations);
            context.Offices.Add(reservation.Office);
            context.OfficeZones.Add(reservation.OfficeZone);
            context.ParkingZones.Add(reservation.ParkingZone);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageReservationCommandHandler>> mockLogger = new Mock<ILogger<ManageReservationCommandHandler>>();

            var handler = new ManageReservationCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Data.OfficeDesk, Is.EqualTo("2"));
            Assert.That(result.Data.ParkingSpace, Is.EqualTo("3"));
            Assert.That(result.Errors, Is.Null);
        }

        [Test]
        public async Task DeleteReservationTest_WithUncorrectData_ReturnResultBadRequest()
        {
            //Arrange
            var user = new User()
            {
                Id = "9ccc3e52-cd9f-49a7-8ded-a39815b30d18",
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
                Date = DateTime.UtcNow,
                UserId = user.Id,
                OfficeDesk = "2",
                ParkingSpace = "3",
            };

            var request = new ManageReservationCommand()
            {
                Id = Guid.Empty,
                UserId = reservation.UserId,
                Date = reservation.Date,
                OfficeDesk = reservation.OfficeDesk,
                ParkingSpace = reservation.ParkingSpace,
                OfficeId = Guid.Empty,
                OfficeZoneId = Guid.Empty,
                ParkingZoneId = Guid.Empty,
            };

            var cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Reservations.RemoveRange(context.Reservations);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageReservationCommandHandler>> mockLogger = new Mock<ILogger<ManageReservationCommandHandler>>();

            var handler = new ManageReservationCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Errors, Is.Not.Null);
            Assert.That(result.Errors.Count, Is.EqualTo(3));
        }
    }
}
