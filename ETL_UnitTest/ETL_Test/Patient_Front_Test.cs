using ETL_Front.Controllers;
using ETL_Front.Services;
using ETL_Shared.InputModels;
using ETL_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;
using Xunit;

namespace ETL_Test
{
    public class Patient_Front_Test 
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var expectedPatients = new List<Patient>(); // Populate with sample patient data
            var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            mockHttpClientWrapper.Setup(wrapper => wrapper.GetStringAsync("api/Patients/All/Include"))
                                 .ReturnsAsync(JsonConvert.SerializeObject(expectedPatients));
            var controller = new PatientsController(mockHttpClientWrapper.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IPagedList<Patient>>(viewResult.Model);
            Assert.Equal(expectedPatients, model);
        }

        [Fact]
        public async Task Create_GET_ReturnsViewResult_WithRequiredData()
        {
            // Arrange
            var allergies = new List<Allergy> { new Allergy { AllergyId = 1, AllergyName = "Allergy 1" } };
            var diseaseInformations = new List<DiseaseInformation> { new DiseaseInformation { DiseaseInformationId = 1, DiseaseName = "Disease 1" } };
            var ncds = new List<NCD> { new NCD { NCDId = 1, NCDName = "NCD 1" } };

            var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            mockHttpClientWrapper.SetupSequence(wrapper => wrapper.GetStringAsync(It.IsAny<string>()))
                                 .ReturnsAsync(JsonConvert.SerializeObject(allergies))
                                 .ReturnsAsync(JsonConvert.SerializeObject(diseaseInformations))
                                 .ReturnsAsync(JsonConvert.SerializeObject(ncds));

            var controller = new PatientsController(mockHttpClientWrapper.Object);

            // Act
            var result = await controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["Allergies"]);
            Assert.NotNull(viewResult.ViewData["DiseaseInformations"]);
            Assert.NotNull(viewResult.ViewData["NCDs"]);
        }

        [Fact]
        public async Task Create_POST_WithValidModel_ReturnsRedirectToActionResult_WithMessage()
        {
            // Arrange
            var model = new PatientInputModel { PatientName = "John Doe" };

            var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            mockHttpClientWrapper.Setup(wrapper => wrapper.PostJsonAsync("api/Patients", model))
                                 .Returns(Task.CompletedTask);

            var controller = new PatientsController(mockHttpClientWrapper.Object);

            // Act
            var result = await controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.Equal("done", redirectResult.RouteValues?["msg"]);

        }
    }
}
