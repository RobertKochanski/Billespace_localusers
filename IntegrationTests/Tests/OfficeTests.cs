using BilleSpace.Domain.Commands.Offices;
using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure.Entities;
using BilleSpace.Infrastructure.Models;
using IntegrationTests;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BilleSpace.IntegrationTests.Tests
{
    public class OfficeTests : Setup
    {
        [Test]
        public async Task OfficeController_Successfull()
        {
            // Arrange
            await AuthenticateAsync();

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

            User user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2aca",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var addModel = new ManageOfficeCommand
            {
                Id = Guid.NewGuid(),
                City = city.Name,
                Address = "test",
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


            var updateModel = new ManageOfficeCommand
            {
                Id = addModel.Id,
                City = city.Name,
                Address = "testChanged",
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

            //act
            //add
            var response = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModel), Encoding.UTF8, "application/json"));
            var addResponse = await response.Content.ReadAsAsync<Result<OfficeModel>>();

            //getAllOffices
            var getOffices = await _httpClient.GetAsync($"api/Offices/");
            var getOfficesResponse = await getOffices.Content.ReadAsAsync<Result<List<OfficeModel>>>();

            //getOfficeById
            var getOfficebyId = await _httpClient.GetAsync($"api/Offices/{addResponse.Data.Id}");
            var getOfficebyIdResponse = await getOfficebyId.Content.ReadAsAsync<Result<OfficeModel>>();

            //update
            var upresponse = await _httpClient.PutAsync($"api/Offices/{addResponse.Data.Id}", new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json"));
            var updateResponse = await upresponse.Content.ReadAsAsync<Result<OfficeModel>>();

            //getOfficeByIdAfterUpdate
            var getOfficebyIdAfterUpdate = await _httpClient.GetAsync($"api/Offices/{addResponse.Data.Id}");
            var getOfficebyIdResponseAfterUpdate = await getOfficebyIdAfterUpdate.Content.ReadAsAsync<Result<OfficeModel>>();

            //del 
            var delresult = await _httpClient.DeleteAsync($"api/Offices/{updateResponse.Data.Id}");


            //assert

            //add
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(addResponse.Code, Is.EqualTo(200));
            Assert.That(addResponse.Data.Address, Is.EqualTo("test"));
            Assert.That(addResponse.Errors, Is.Null);

            //getAllOffices
            Assert.That(getOffices.IsSuccessStatusCode, Is.True);
            Assert.That(getOfficesResponse.Code, Is.EqualTo(200));
            Assert.That(getOfficesResponse.Errors, Is.Null);

            //getOfficeById
            Assert.That(getOfficebyId.IsSuccessStatusCode, Is.True);
            Assert.That(getOfficebyIdResponse.Code, Is.EqualTo(200));
            Assert.That(getOfficebyIdResponse.Data.Address, Is.EqualTo("test"));
            Assert.That(getOfficebyIdResponse.Errors, Is.Null);

            //update
            Assert.That(upresponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(updateResponse.Code, Is.EqualTo(200));
            Assert.That(updateResponse.Data.Address, Is.EqualTo("testChanged"));
            Assert.That(updateResponse.Errors, Is.Null);

            //getOfficeByIdgetOfficebyIdAfterUpdate
            Assert.That(getOfficebyIdAfterUpdate.IsSuccessStatusCode, Is.True);
            Assert.That(getOfficebyIdResponseAfterUpdate.Code, Is.EqualTo(200));
            Assert.That(getOfficebyIdResponseAfterUpdate.Data.Address, Is.EqualTo("testChanged"));
            Assert.That(getOfficebyIdResponseAfterUpdate.Errors, Is.Null);

            //del
            Assert.That(delresult.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public async Task OfficeController_InvalidScenarios()
        {
            // Arrange
            AuthenticateAsync();

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

            User user = new User()
            {
                Id = "469f41f5-0b77-4b25-a158-ddec103f2aca",
                Email = "integration@test.com",
                PasswordHash = "zaq1@WSX",
                FullName = "integration test",
                UserName = "integ",
                PhoneNumber = "123123123",
                IsReceptionist = true
            };

            var addModel = new ManageOfficeCommand
            {
                Id = Guid.NewGuid(),
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

            var addModelSameOfficeZone = new ManageOfficeCommand
            {
                Id = addModel.Id,
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

            var addModelWithNullOfficeZone = new ManageOfficeCommand
            {
                Id = addModel.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel>
                {

                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };

            var addModelWithSameOfficeAdress = new ManageOfficeCommand
            {
                Id = addModel.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel> {
                new OfficeZoneModel
                {
                    Name = "OfficeZone8",
                    Desks = 5,
                },
                new OfficeZoneModel
                {
                    Name = "OfficeZone9",
                    Desks = 15,
                }
            },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                CreatorId = user.Id
            };

            var addModelWithoutCity = new ManageOfficeCommand
            {
                Id = addModel.Id,
                City = string.Empty,
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

            //act
            //addCorrect
            var response = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModel), Encoding.UTF8, "application/json"));
            var addResponse = await response.Content.ReadAsAsync<Result<OfficeModel>>();

            //addWithSameOficeZoneName
            var responseSameOficeZoneName = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModelSameOfficeZone), Encoding.UTF8, "application/json"));
            var addResponseSameOficeZoneName = await responseSameOficeZoneName.Content.ReadAsAsync<Result<OfficeModel>>();

            //addWithSameOficeZoneName
            var responseNullOfficeZone = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModelWithNullOfficeZone), Encoding.UTF8, "application/json"));
            var addResponseNullOfficeZone = await responseNullOfficeZone.Content.ReadAsAsync<Result<OfficeModel>>();

            //addWithSameOficeAdress
            var responseAdress = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModelWithSameOfficeAdress), Encoding.UTF8, "application/json"));
            var addResponseAdress = await responseAdress.Content.ReadAsAsync<Result<OfficeModel>>();

            //addWithoutCity
            var responseWithoutCity = await _httpClient.PostAsync("api/Offices", new StringContent(JsonConvert.SerializeObject(addModelWithoutCity), Encoding.UTF8, "application/json"));
            var addResponseWithoutCity = await responseWithoutCity.Content.ReadAsAsync<Result<OfficeModel>>();

            //del 
            var delresultWithoutAuthorization = await _httpClient.DeleteAsync($"api/Offices/{addResponse.Data.Id.Equals(null)}");

            //assert

            //addWithSameOficeZoneName
            Assert.That(responseSameOficeZoneName.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(addResponseSameOficeZoneName.Code, Is.EqualTo(400));
            Assert.That(addResponseSameOficeZoneName.Errors.Count, Is.EqualTo(1));

            //addWithNullOfficeZone
            Assert.That(responseNullOfficeZone.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(addResponseNullOfficeZone.Code, Is.EqualTo(400));
            Assert.That(addResponseNullOfficeZone.Errors.Count, Is.EqualTo(1));

            //addWithSameOficeAdress
            Assert.That(responseAdress.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(addResponseAdress.Code, Is.EqualTo(400));
            Assert.That(addResponseAdress.Errors.Count, Is.EqualTo(1));

            //addWithoutCity
            Assert.That(responseWithoutCity.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(addResponseWithoutCity.Code, Is.EqualTo(400));
            Assert.That(addResponseWithoutCity.Errors.Count, Is.EqualTo(1));

            //del
            Assert.That(delresultWithoutAuthorization.IsSuccessStatusCode, Is.False);
        }
    }
}
