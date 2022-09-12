using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using BilleSpace.Domain.Commands;

namespace BilleSpace.UnitTests.OfficeTests
{
    [Category("DeleteOfficeServiceTest")]
    public class DeleteOfficeServiceTest
    {
        private async Task<BilleSpaceDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<BilleSpaceDbContext>()
            .UseInMemoryDatabase("InMemoryDatabase")
            .Options;
            var databaseContext = new BilleSpaceDbContext(options, null);
            databaseContext.Database.EnsureCreated();

            return databaseContext;
        }

        [Test]
        public async Task DeleteOfficeServiceTest_ShouldDeleteOffice_WithCorrectData()
        {
            var myCurrentUserIdentifier = "test";
            //var id = "ae8c494f-8875-4a1c-9624-828015ce2b3e";
            //act
            City city = new City()
            {
                Name = "Test",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "test",
                    Symbol = "TE"
                }
            };

            var office = new Office
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "test",
                PostCode = "test",
                OfficeMapUrl = "test",
                OfficeZones = new List<OfficeZone>
                {
                   new OfficeZone
                   {
                       Name = "OfficeZone",
                       Desks = 5,
                   },
                },
                ParkingZones = new List<ParkingZone>(),
                AuthorNameIdentifier = "test"
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var request = new DeleteOfficeCommand(office.Id, myCurrentUserIdentifier);

            Mock<ILogger<DeleteOfficeCommandHandler>> mockLogger = new Mock<ILogger<DeleteOfficeCommandHandler>>();

            var handler = new DeleteOfficeCommandHandler(context, mockLogger.Object) { };

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            //act

            var result = await handler.Handle(request, cancellationTokenSource.Token);

            //assert

            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Errors, Is.Null);
        }

        [Test]
        public async Task DeleteOfficeServiceTest_ShouldDeleteOffice_WithNotExistingOffice()
        {
            var myCurrentUserIdentifier = "test";

            //act
            City city = new City()
            {
                Name = "Test",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "test",
                    Symbol = "TE"
                }
            };

            var office = new Office
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "test",
                PostCode = "test",
                OfficeMapUrl = "test",
                OfficeZones = new List<OfficeZone>
                {
                   new OfficeZone
                   {
                       Name = "OfficeZone",
                       Desks = 5,
                   },
                },
                ParkingZones = new List<ParkingZone>(),
                AuthorNameIdentifier = "test"
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var wrongId = Guid.NewGuid();

            var request = new DeleteOfficeCommand(wrongId, myCurrentUserIdentifier);

            Mock<ILogger<DeleteOfficeCommandHandler>> mockLogger = new Mock<ILogger<DeleteOfficeCommandHandler>>();

            var handler = new DeleteOfficeCommandHandler(context, mockLogger.Object) { };

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            //act

            var result = await handler.Handle(request, cancellationTokenSource.Token);

            //assert

            Assert.That(result.Code, Is.EqualTo(404));
            Assert.That(result.Errors[0], Is.EqualTo($"There is no object with id: {wrongId}"));
        }

        [Test]
        public async Task DeleteOfficeServiceTest_ShouldDeleteOffice_WithUncorrectUserNameIdentifier()
        {
            var myCurrentUserIdentifier = "wrong";
            //var id = "ae8c494f-8875-4a1c-9624-828015ce2b3e";
            //act
            City city = new City()
            {
                Name = "Test",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "test",
                    Symbol = "TE"
                }
            };

            var office = new Office
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "test",
                PostCode = "test",
                OfficeMapUrl = "test",
                OfficeZones = new List<OfficeZone>
                {
                   new OfficeZone
                   {
                       Name = "OfficeZone",
                       Desks = 5,
                   },
                },
                ParkingZones = new List<ParkingZone>(),
                AuthorNameIdentifier = "test"
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var request = new DeleteOfficeCommand(office.Id, myCurrentUserIdentifier);

            Mock<ILogger<DeleteOfficeCommandHandler>> mockLogger = new Mock<ILogger<DeleteOfficeCommandHandler>>();

            var handler = new DeleteOfficeCommandHandler(context, mockLogger.Object) { };

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            //act

            var result = await handler.Handle(request, cancellationTokenSource.Token);

            //assert

            Assert.That(result.Code, Is.EqualTo(400));
        }
    }
}