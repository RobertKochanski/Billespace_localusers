using BilleSpace.Domain.Commands.Offices;
using BilleSpace.Infrastructure.Entities;
using BilleSpace.Infrastructure.Models;
using BilleSpace.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;

namespace BilleSpace.UnitTests.Offices
{
    public class EditOfficeTests
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
        public async Task EditOffice_WithCorrectData_ReturnResultOKWithOfficeModel()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f4-0b77-4b25-a158-ddec103f2aca",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> { new OfficeZone
                {
                    Name = "OfficeZone",
                    Desks = 5,
                } },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel> {
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone2",
                        Desks = 5,
                    },
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone3",
                        Desks = 15,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Countries.Add(city.Country);
            context.Cities.Add(city);
            context.Offices.Add(modelToAdd);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Data.Address, Is.EqualTo("Warszawa"));
            Assert.That(result.Data.OfficeZones.ElementAt(0).Name, Is.EqualTo("OfficeZone2"));
            Assert.That(result.Data.OfficeZones.Count, Is.EqualTo(2));
            Assert.That(result.Errors, Is.Null);
        }

        [Test]
        public async Task EditOffice_WithTwoSameOfficeZoneName_ReturnResultBadRequestWithErrorMessage()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2acb",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> { new OfficeZone
                {
                    Name = "OfficeZone",
                    Desks = 5,
                } },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel> {
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    },
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone",
                        Desks = 15,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(modelToAdd);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo($"[{DateTime.UtcNow}] Office Zones must have different name."));
        }

        [Test]
        public async Task EditOffice_WithoutOfficeZoneName_ReturnResultBadRequestWithErrorMessage()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2acc",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> {
                    new OfficeZone
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Bździszewo",
                OfficeZones = new List<OfficeZoneModel>()
                {
                    new OfficeZoneModel
                    {
                        Name = "",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(modelToAdd);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo($"[{DateTime.UtcNow}] OffizeZones can not be null."));
        }

        [Test]
        public async Task EditOffice_WithExistedAddress_ReturnResultBadRequestWithErrorMessage()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2acd",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> {
                    new OfficeZone
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,                
                Creator = user
            };
            var modelToAdd2 = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn2",
                PostCode = "Olsztyn2",
                OfficeZones = new List<OfficeZone> {
                    new OfficeZone
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = city.Name,
                Address = "Olsztyn2",
                PostCode = "Bździszewo",
                OfficeZones = new List<OfficeZoneModel>()
                {
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(modelToAdd);
            context.Offices.Add(modelToAdd2);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(2));
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo($"[{DateTime.UtcNow}] Address {request.Address} already taken."));
        }

        [Test]
        public async Task EditOffice_WithWrongCity_ReturnResultBadRequestWithErrorMessage()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f3aca",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> { new OfficeZone
                {
                    Name = "OfficeZone",
                    Desks = 5,
                } },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = "Warszawa",
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel> {
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(modelToAdd);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo($"[{DateTime.UtcNow}] Can not find city {request.City}."));
        }

        [Test]
        public async Task EditOffice_NotOfficeCreator_ReturnResultForbiddenWithErrorMessage()
        {
            // Arrange
            City city = new City()
            {
                Name = "Olsztyn",
                Country = new Country()
                {
                    Id = Guid.NewGuid(),
                    Name = "Poland",
                    Symbol = "PL"
                }
            };

            var user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2bca",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var user2 = new User()
            {
                Id = "469f41f6-0b77-4b25-a158-ddec103f2acb",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var modelToAdd = new Office()
            {
                Id = Guid.NewGuid(),
                City = city,
                Address = "Olsztyn",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZone> { new OfficeZone
                {
                    Name = "OfficeZone",
                    Desks = 5,
                } },
                ParkingZones = new List<ParkingZone>(),
                OfficeMapUrl = null,
                Creator = user
            };

            var request = new ManageOfficeCommand
            {
                Id = modelToAdd.Id,
                City = "Warszawa",
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel> {
                    new OfficeZoneModel
                    {
                        Name = "OfficeZone",
                        Desks = 5,
                    }
                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user2.Id
            };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var context = await GetDatabaseContext();
            context.Offices.RemoveRange(context.Offices);
            context.Offices.Add(modelToAdd);
            await context.SaveChangesAsync();

            Mock<ILogger<ManageOfficeCommandHandler>> mockLogger = new Mock<ILogger<ManageOfficeCommandHandler>>();

            var handler = new ManageOfficeCommandHandler(context, mockLogger.Object);

            // Act
            var result = await handler.Handle(request, cancellationTokenSource.Token);

            // Assert 
            Assert.That(context.Offices.Count, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo(403));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }
    }
}
