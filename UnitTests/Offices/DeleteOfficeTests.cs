using BilleSpace.Domain.Commands.Offices;
using BilleSpace.Infrastructure.Entities;
using BilleSpace.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;



namespace BilleSpace.UnitTests.Offices
{
    public class DeleteOfficeTest
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
        public async Task DeleteOfficeTest_ShouldDeleteOffice_WithCorrectData()
        {
            // arrange
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

            var user = new User()
            {
                Id = "53e9b98d-b03e-4e49-9892-26a405f7bedf",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
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
                Creator = user
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var request = new DeleteOfficeCommand(office.Id, user.Id);

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
        public async Task DeleteOfficeTest_ShouldDeleteOffice_WithNotExistingOffice()
        {
            // arrenge
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

            var user = new User()
            {
                Id = "3f82e9ed-4a55-40ee-827c-61ef981f7e96",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
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
                Creator = user
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var wrongId = Guid.NewGuid();

            var request = new DeleteOfficeCommand(wrongId, user.Id);

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
        public async Task DeleteOfficeTest_ShouldDeleteOffice_WithUncorrectUser()
        {
            // arrange
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

            var user = new User()
            {
                Id = "85dea942-6291-496c-8cd4-25fdff3802b3",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
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
                Creator = user
            };

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            var request = new DeleteOfficeCommand(office.Id, "2b201b13-7f47-4672-8731-f30a9de557f6");

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
