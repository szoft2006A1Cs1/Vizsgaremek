using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JogsifoglaloApi.Tests.Scenarios
{
    [TestClass]
    public class InstructorFlowTests
    {
        private Mock<IInstructorRepository> _mockInstructorRepo = null!;
        private InstructorsController _instructorController = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockInstructorRepo = new Mock<IInstructorRepository>();
            _instructorController = new InstructorsController(_mockInstructorRepo.Object);
        }

        private void SetUserContext(string userId, string role)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _instructorController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [TestMethod]
        public async Task Scenario_Instructor_UpdateOwnProfile_And_VerifyChanges()
        {
            SetUserContext("50", "oktato");

            var fakeInstructor = new Instructor
            {
                Oktato_Id = 5,
                Felhasznalo_Id = 50,
                Felhasznalo = new User
                {
                    Felhasznalo_Id = 50,
                    Nev = "Eredeti Oktato",
                    Email = "oktato@auto.hu",
                    Jelszo = "jelszo",
                    Telefonszam = "06701112233"
                },
                Instructs = new List<Instruct>()
            };

            _mockInstructorRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(fakeInstructor);
            _mockInstructorRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var updateDto = new InstructorUpdateDto
            {
                Nev = "Frissitett Oktato Nev",
                Telefonszam = "06209998877"
            };

            var result = await _instructorController.UpdateInstructor(5, updateDto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            Assert.AreEqual("Frissitett Oktato Nev", fakeInstructor.Felhasznalo.Nev);
            Assert.AreEqual("06209998877", fakeInstructor.Felhasznalo.Telefonszam);

            _mockInstructorRepo.Verify(r => r.Update(fakeInstructor), Times.Once);
            _mockInstructorRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Scenario_Instructor_TryToUpdateOtherProfile_ReturnsForbidden()
        {
            SetUserContext("50", "oktato");

            var otherInstructor = new Instructor
            {
                Oktato_Id = 10,
                Felhasznalo_Id = 99, 
                Felhasznalo = new User { Nev = "Masik Oktato", Email = "masik@oktato.hu", Jelszo = "kamujelszo", Telefonszam = "060000000" },
                Instructs = new List<Instruct>()
            };

            _mockInstructorRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(otherInstructor);

            var updateDto = new InstructorUpdateDto { Nev = "Valami Nev" };
            var result = await _instructorController.UpdateInstructor(10, updateDto);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(403, objectResult.StatusCode);

            Assert.AreEqual("Masik Oktato", otherInstructor.Felhasznalo.Nev);
        }
    }
}
