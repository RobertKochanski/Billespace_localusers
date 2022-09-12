using BilleSpace.Infrastructure.Entities;
using System.Net;
using BilleSpace.Infrastructure.Models;
using Newtonsoft.Json;
using System.Text;
using BilleSpace.Domain.Results;
using BilleSpace.Domain.Commands;

// office ogólnie, po id, delete, edycja
namespace BilleSpace.IntegrationTests.Offices
{
    [Category("E2E_Office_Test")]
    public class E2E_Office_Test_With_UnCorrect_Data : Setup
    {
        [Test]
        public async Task E2E_Office_With_UnCorrect_Data_Test()
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

            var modelOffice = new Office()
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
                AuthorNameIdentifier = "Author"
            };

            var addModel = new ManageOfficeCommand
            {
                Id = modelOffice.Id,
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
                AuthorNameIdentifier = "Zdzisiek"
            };

            var addModelSameOfficeZone = new ManageOfficeCommand
            {
                Id = modelOffice.Id,
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
                AuthorNameIdentifier = "Zdzisiek"
            };

            var addModelWithNullOfficeZone = new ManageOfficeCommand
            {
                Id = modelOffice.Id,
                City = city.Name,
                Address = "Warszawa",
                PostCode = "Olsztyn",
                OfficeZones = new List<OfficeZoneModel>
                {

                },
                ParkingZones = new List<ParkingZoneModel>(),
                OfficeMap = null,
                AuthorNameIdentifier = "Zdzisiek"
            };

            var addModelWithSameOfficeAdress = new ManageOfficeCommand
            {
                Id = modelOffice.Id,
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
                AuthorNameIdentifier = "Zdzisiek"
            };

            var addModelWithoutCity = new ManageOfficeCommand
            {
                Id = modelOffice.Id,
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
                AuthorNameIdentifier = "Zdzisiek"
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