using BilleSpace.Domain.Commands;
using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;


namespace BilleSpace.IntegrationTests.Reservations
{
    public class ReservationTests : Setup
    {
        [Test]
        public async Task ReservationsController_Successfull()
        {
            // Arrange
            AuthenticateAsync();

            // Act
            var responsePost = await _httpClient.PostAsync("api/reservations", new StringContent(
                JsonConvert.SerializeObject(new ManageReservationCommand()
                {
                    Id = Guid.Parse("e15ff775-3a89-4f0e-a037-78bf1c7b0d8c"),
                    Date = DateTime.Now,
                    OfficeDesk = "2",
                    ParkingSpace = "3",
                    UserNameIdentifier = "Zdzisiek",
                    OfficeId = Guid.Parse("e15ff775-3a89-4f0e-a037-78bf1c7b0d8c"),
                    OfficeZoneId = Guid.Parse("b2a24a75-5b26-456a-aa1d-f02821be6d4a"),
                    ParkingZoneId = Guid.Parse("c0988a2e-24f5-4a82-b4c2-3619f338a2eb")
                }),
                Encoding.UTF8,
                "application/json")
            );
            var createContent = await responsePost.Content.ReadAsAsync<Result<ReservationModel>>();

            var responseGet = await _httpClient.GetAsync("api/reservations");
            var getContent = await responseGet.Content.ReadAsAsync<Result<List<ReservationModel>>>();

            var responseGetById = await _httpClient.GetAsync($"api/reservations/{createContent.Data.Id}");
            var getByIdContent = await responseGetById.Content.ReadAsAsync<Result<ReservationModel>>();

            var responseDelete = await _httpClient.DeleteAsync($"api/reservations/{createContent.Data.Id}");

            var responseGetAfterDelete = await _httpClient.GetAsync("api/reservations");
            var getContentAfterDelete = await responseGetAfterDelete.Content.ReadAsAsync<Result<List<ReservationModel>>>();

            // Assert
            // POST
            Assert.That(responsePost.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(createContent.Code, Is.EqualTo(200));
            Assert.That(createContent.Data.OfficeDesk, Is.EqualTo("2"));
            Assert.That(createContent.Data.ParkingSpace, Is.EqualTo("3"));
            Assert.That(createContent.Data.OfficeZone.Name, Is.EqualTo("OfficeZone"));
            Assert.That(createContent.Data.ParkingZone.Name, Is.EqualTo("ParkingZone"));

            // GET
            Assert.That(responseGet.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getContent.Code, Is.EqualTo(200));
            Assert.That(getContent.Data.Count, Is.EqualTo(1));

            // GET By ID
            Assert.That(responseGetById.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getByIdContent.Code, Is.EqualTo(200));
            Assert.That(getByIdContent.Data.ParkingSpace, Is.EqualTo("3"));
            Assert.That(getByIdContent.Data.OfficeDesk, Is.EqualTo("2"));
            Assert.That(getByIdContent.Data.OfficeZone.Name, Is.EqualTo("OfficeZone"));
            Assert.That(getByIdContent.Data.ParkingZone.Name, Is.EqualTo("ParkingZone"));

            // DELETE
            Assert.That(responseDelete.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // GET after DELETE
            Assert.That(responseGetAfterDelete.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getContentAfterDelete.Code, Is.EqualTo(200));
            Assert.That(getContentAfterDelete.Data.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task POSTReservations_OfficeAndOfficeZoneNotExists_ReturnBadRequestResultWithErrorsList()
        {
            // Arrange
            AuthenticateAsync();

            // Act
            var responsePost = await _httpClient.PostAsync("api/reservations", new StringContent(
                JsonConvert.SerializeObject(new ManageReservationCommand()
                {
                    Id = Guid.Parse("e15ff775-3a89-4f0e-a037-78bf1c7b0d8c"),
                    Date = DateTime.Now,
                    OfficeDesk = "2",
                    ParkingSpace = "3",
                    UserNameIdentifier = "Zdzisiek",
                    OfficeId = Guid.Parse("de551601-f699-49ee-9f8a-073be249ddca"), // Not Exist
                    OfficeZoneId = Guid.Parse("a2558bdb-8f9e-42f8-a3ba-1bdf767608e1"), // Not Exist
                    ParkingZoneId = Guid.Parse("c0988a2e-24f5-4a82-b4c2-3619f338a2eb")
                }),
                Encoding.UTF8,
                "application/json")
            );

            var createContent = await responsePost.Content.ReadAsAsync<Result<ReservationModel>>();

            // Assert 
            Assert.That(responsePost.IsSuccessStatusCode, Is.EqualTo(false));
            Assert.That(responsePost.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(createContent.Code, Is.EqualTo(400));
            Assert.That(createContent.Errors.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task DELETEReservations_ReservationNotExist_ReturnBadRequestResultWithErrorsList()
        {
            // Arrange
            AuthenticateAsync();

            // Act
            var responsePost = await _httpClient.DeleteAsync("api/reservations/a2558bdb-8f9e-42f8-a3ba-1bdf767608e1");
            var createContent = await responsePost.Content.ReadAsAsync<Result>();

            // Assert 
            Assert.That(responsePost.IsSuccessStatusCode, Is.EqualTo(false));
            Assert.That(createContent.Code, Is.EqualTo(404));
            Assert.That(createContent.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdReservation_ReservationNotExist_ReturnBadRequestResultWithErrorsList()
        {
            // Arrange
            AuthenticateAsync();

            // Act
            var responsePost = await _httpClient.GetAsync("api/reservations/a2558bdb-8f9e-42f8-a3ba-1bdf767608e1");
            var createContent = await responsePost.Content.ReadAsAsync<Result>();

            // Assert 
            Assert.That(responsePost.IsSuccessStatusCode, Is.EqualTo(false));
            Assert.That(createContent.Code, Is.EqualTo(404));
            Assert.That(createContent.Errors.Count, Is.EqualTo(1));
        }
    }
}
