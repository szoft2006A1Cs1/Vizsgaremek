using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
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
    public class StudentFlowTests
    {
        private Mock<IAuthService> _mockAuth = null!;
        private Mock<IPayRepository> _mockPayRepo = null!;
        private Mock<IAppointmentRepository> _mockAppRepo = null!;

        private AuthController _authController = null!;
        private PaymentsController _paymentsController = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAuth = new Mock<IAuthService>();
            _mockPayRepo = new Mock<IPayRepository>();
            _mockAppRepo = new Mock<IAppointmentRepository>();

            _authController = new AuthController(_mockAuth.Object);
            _paymentsController = new PaymentsController(_mockPayRepo.Object, _mockAppRepo.Object);
        }

        private void SetUserContext(string id, string role)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _paymentsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [TestMethod]
        public async Task Scenario_Student_FromRegistrationToSuccessfulBooking()
        {
            var regDto = new UserRegisterDto
            {
                Nev = "Teszt Elek",
                Email = "elek@citromail.hu",
                Jelszo = "Titkos123",
                Telefonszam = "06301112233"
            };

            var registeredUser = new User { Felhasznalo_Id = 10, Nev = regDto.Nev, Email = regDto.Email, Jelszo = "hash", Telefonszam = regDto.Telefonszam };

            _mockAuth.Setup(s => s.RegisterAsync(It.IsAny<User>(), regDto.Jelszo))
                     .ReturnsAsync(registeredUser);

            _authController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var regResult = await _authController.Register(regDto);

            if (regResult.Result is ObjectResult obj)
            {
                Assert.IsTrue(obj.StatusCode == 201 || obj.StatusCode == 200, "A regisztrációnak sikeresnek kell lennie!");
            }
            else if (regResult.Result is CreatedResult cr)
            {
                Assert.AreEqual(201, cr.StatusCode);
            }
            else
            {
                Assert.IsNotNull(regResult.Result, "A regisztráció eredménye null lett!");
            }

            SetUserContext("10", "tanulo");

            var szabadIdopont = new Appointment
            {
                Idopont_Id = 50,
                Allapot = Status.szabad,
                Oktat_Id = 1,
                Kezdes_Dt = DateTime.Now,
                Ar = 15000
            };

            _mockAppRepo.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(szabadIdopont);
            _mockPayRepo.Setup(r => r.AddAsync(It.IsAny<Pay>())).Returns(Task.CompletedTask);
            _mockPayRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockAppRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var payDto = new PayCreateDto { Idopont_Id = 50, Osszeg = 15000 };

            var payResult = await _paymentsController.CreatePayment(payDto);

            Assert.AreEqual(Status.foglalt, szabadIdopont.Allapot);
            Assert.AreEqual(10, szabadIdopont.Tanulo_Id);
            Assert.IsInstanceOfType(payResult.Result, typeof(CreatedAtActionResult));
        }
    }
}
