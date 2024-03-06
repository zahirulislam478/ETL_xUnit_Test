using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETL_API.Controllers;
using ETL_API.Repositories.Interfaces;
using ETL_Shared.InputModels;
using ETL_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace ETL_Test
{
    // Test class for testing the PatientsController
    public class Patient_APT_Test
    {
        // Mocks for dependencies
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Patient>> _mockRepository;
        private readonly PatientsController _controller;

        // Constructor to set up mocks and initialize controller
        public Patient_APT_Test()
        {
            // Initialize mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IGenericRepository<Patient>>();

            // Setup GetRepository method of IUnitOfWork to return mock repository
            _mockUnitOfWork.Setup(uow => uow.GetRepository<Patient>()).Returns(_mockRepository.Object);

            // Initialize controller with mock dependencies
            _controller = new PatientsController(_mockUnitOfWork.Object);
        }

        // Test method to verify the behavior of GetPatents action method
        [Fact]
        public async Task GetPatents_ReturnsListOfPatients()
        {
            // Arrange
            var patients = new List<Patient>
            {
                // Sample patient data
                new Patient { PatientId = 1, PatientName = "Patient 1", Epilepsy = EpilepsyType.Yes, DiseaseInformationId = 100, DiseaseInformation = new DiseaseInformation() },
                new Patient { PatientId = 2, PatientName = "Patient 2", Epilepsy = EpilepsyType.No, DiseaseInformationId = 200, DiseaseInformation = new DiseaseInformation() }
            };

            // Setup mock repository to return sample patient data
            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>()))
                .ReturnsAsync(patients);

            // Act
            var result = await _controller.GetPatents();

            // Assert
            var returnValue = Assert.IsType<List<Patient>>(result);

            // Ensure the returned value is not null and count matches expected count
            Assert.NotNull(returnValue);
            Assert.Equal(patients.Count, returnValue.Count);
        }

        // Test method to verify the behavior of PostPatient action method with valid input
        [Fact]
        public async Task PostPatient_WithValidInput_ReturnsNoContent()
        {
            // Arrange
            var model = new PatientInputModel
            {
                // Sample patient input model
                PatientName = "Test Patient",
                Epilepsy = ETL_Shared.Models.EpilepsyType.No,
                DiseaseInformationId = 1,
                AllergyIds = new List<int> { 1, 2 }.ToArray(),
                NCDIds = new List<int> { 3, 4 }.ToArray()
            };

            // Act
            var result = await _controller.PostPatient(model);

            // Assert
            Assert.IsType<ActionResult<Patient>>(result);
        }
    }
}
