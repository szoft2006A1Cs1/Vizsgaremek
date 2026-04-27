using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace JogsifoglaloApi.Tests.UnitTests
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private Mock<IAppointmentRepository> _mockRepo = null!;
        private AppointmentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IAppointmentRepository>();
            _controller = new AppointmentsController(_mockRepo.Object);
            SetUserContext("10", "tanulo");
        }

        private void SetUserContext(string id, string role)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        private Appointment CreateValidApp(int id = 1, Status st = Status.szabad, int userId = 10)
        {
            return new Appointment
            {
                Idopont_Id = id,
                Allapot = st,
                Ar = 15000,
                Oktat_Id = 1,
                Kezdes_Dt = DateTime.Now.AddDays(1),
                Oktat = new Instruct { Oktato_Id = 1, Kategoria_Id = 1, Oktato = new Instructor { Felhasznalo_Id = userId, Oktato_Id = 1 } }
            };
        }

        [TestMethod]
        public async Task GetAvailable_WhenExist_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetAvailableAsync()).ReturnsAsync(new List<Appointment> { CreateValidApp() });
            var res = await _controller.GetAvailable();
            Assert.IsInstanceOfType(res.Result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetAvailable_Empty_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetAvailableAsync()).ReturnsAsync(new List<Appointment>());
            var res = await _controller.GetAvailable();
            Assert.IsInstanceOfType(res.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetAvailableById_WhenExist_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetAvailableByIdAsync(1)).ReturnsAsync(new List<Appointment> { CreateValidApp() });
            var res = await _controller.GetAvailableById(1);
            Assert.IsInstanceOfType(res.Result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetAvailableById_Empty_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetAvailableByIdAsync(1)).ReturnsAsync(new List<Appointment>());
            var res = await _controller.GetAvailableById(1);
            Assert.IsInstanceOfType(res.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetMyBookings_Valid_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetByStudentIdAsync(10)).ReturnsAsync(new List<Appointment> { CreateValidApp() });
            var res = await _controller.GetMyBookings();
            Assert.IsInstanceOfType(res.Result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetMyBookings_NoUser_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            var res = await _controller.GetMyBookings();
            Assert.IsInstanceOfType(res.Result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task GetMySchedule_AsInstructor_ReturnsOk()
        {
            SetUserContext("5", "oktato");
            _mockRepo.Setup(r => r.GetByInstructorUserIdAsync(5)).ReturnsAsync(new List<Appointment> { CreateValidApp() });
            var res = await _controller.GetMySchedule();
            Assert.IsInstanceOfType(res.Result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetMySchedule_AsStudent_ReturnsForbidden()
        {
            var res = await _controller.GetMySchedule();
            Assert.AreEqual(403, (res.Result as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task GetAll_AsAdmin_ReturnsOk()
        {
            SetUserContext("1", "admin");
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Appointment> { CreateValidApp() });
            var res = await _controller.GetAll();
            Assert.IsInstanceOfType(res.Result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetAll_AsNonAdmin_ReturnsForbidden()
        {
            var res = await _controller.GetAll();
            Assert.AreEqual(403, (res.Result as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task CreateAppointment_Valid_ReturnsOk()
        {
            SetUserContext("5", "oktato");
            _mockRepo.Setup(r => r.GetInstructByIdAsync(1)).ReturnsAsync(new Instruct {Oktato_Id = 1, Kategoria_Id = 1, Oktato = new Instructor { Felhasznalo_Id = 5, Oktato_Id = 1 } });
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            var res = await _controller.CreateAppointment(new AppointmentCreateDto { Oktat_Id = 1, Kezdes_Dt = DateTime.Now.AddDays(1) });
            Assert.AreEqual(200, (res as ObjectResult)?.StatusCode);
        }
        [TestMethod]
        public async Task CreateAppointment_PastDate_ReturnsBadRequest()
        {
            SetUserContext("5", "oktato");
            _mockRepo.Setup(r => r.GetInstructByIdAsync(1)).ReturnsAsync(new Instruct {Oktato_Id = 1, Kategoria_Id = 1, Oktato = new Instructor { Felhasznalo_Id = 5, Oktato_Id = 1 } });
            var res = await _controller.CreateAppointment(new AppointmentCreateDto { Oktat_Id = 1, Kezdes_Dt = DateTime.Now.AddDays(-1) });
            Assert.AreEqual(400, (res as ObjectResult)?.StatusCode);
        }
        [TestMethod]
        public async Task CreateAppointment_WrongInstructor_ReturnsForbidden()
        {
            SetUserContext("5", "oktato");
            _mockRepo.Setup(r => r.GetInstructByIdAsync(1)).ReturnsAsync(new Instruct {Oktato_Id = 1, Kategoria_Id = 1, Oktato = new Instructor { Felhasznalo_Id = 99, Oktato_Id = 1 } });
            var res = await _controller.CreateAppointment(new AppointmentCreateDto { Oktat_Id = 1, Kezdes_Dt = DateTime.Now.AddDays(1) });
            Assert.AreEqual(403, (res as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task BookAppointment_Free_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CreateValidApp(1, Status.szabad));
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            var res = await _controller.BookAppointment(1);
            Assert.AreEqual(200, (res as ObjectResult)?.StatusCode);
        }
        [TestMethod]
        public async Task BookAppointment_Taken_ReturnsBadRequest()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CreateValidApp(1, Status.foglalt));
            var res = await _controller.BookAppointment(1);
            Assert.AreEqual(400, (res as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task CancelBooking_Owner_ReturnsOk()
        {
            var app = CreateValidApp(1, Status.foglalt); app.Tanulo_Id = 10;
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            var res = await _controller.CancelBooking(1);
            Assert.AreEqual(200, (res as ObjectResult)?.StatusCode);
        }
        [TestMethod]
        public async Task CancelBooking_NotOwner_ReturnsForbidden()
        {
            var app = CreateValidApp(1, Status.foglalt); app.Tanulo_Id = 99;
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            var res = await _controller.CancelBooking(1);
            Assert.AreEqual(403, (res as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAppointment_Valid_ReturnsOk()
        {
            SetUserContext("5", "oktato");
            var app = CreateValidApp(1, Status.szabad, 5);
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            var res = await _controller.UpdateAppointment(1, new AppointmentUpdateDto { Ar = 20000 });
            Assert.AreEqual(200, (res as ObjectResult)?.StatusCode);
        }
        [TestMethod]
        public async Task UpdateAppointment_Occupied_ReturnsBadRequest()
        {
            SetUserContext("5", "oktato");
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CreateValidApp(1, Status.foglalt, 5));
            var res = await _controller.UpdateAppointment(1, new AppointmentUpdateDto { Ar = 20000 });
            Assert.AreEqual(400, (res as ObjectResult)?.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAppointment_Admin_ReturnsOk()
        {
            SetUserContext("1", "admin");
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CreateValidApp(1, Status.foglalt));
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            var res = await _controller.DeleteAppointment(1);
            Assert.AreEqual(200, (res as ObjectResult)?.StatusCode);
        }
    }
}