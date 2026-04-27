using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace JogsifoglaloApi.Tests.UnitTests;

[TestClass]
public class PayControllerTests
{
    private Mock<IPayRepository> _mockPayRepo = null!;
    private Mock<IAppointmentRepository> _mockAppRepo = null!;
    private PaymentsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockPayRepo = new Mock<IPayRepository>();
        _mockAppRepo = new Mock<IAppointmentRepository>();
        _controller = new PaymentsController(_mockPayRepo.Object, _mockAppRepo.Object);

        var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "tanulo")
            };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [TestMethod]
    public async Task ProcessPayment_ValidCard_ReturnsOk()
    {
        var createDto = new PayCreateDto { Idopont_Id = 1, Osszeg = 15000 };
        var appointment = new Appointment { Idopont_Id = 1, Oktat_Id = 1, Kezdes_Dt = DateTime.Now, Ar = 15000, Allapot = Status.szabad };

        _mockAppRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
        _mockPayRepo.Setup(r => r.AddAsync(It.IsAny<Pay>())).Returns(Task.CompletedTask);
        _mockPayRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockAppRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _controller.CreatePayment(createDto);

        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(Status.foglalt, appointment.Allapot); 
    }

    [TestMethod]
    public async Task ProcessPayment_AlreadyPaid_ReturnsBadRequest()
    {
        var createDto = new PayCreateDto { Idopont_Id = 1, Osszeg = 15000 };
        var appointment = new Appointment {Idopont_Id = 1, Oktat_Id = 1, Kezdes_Dt = DateTime.Now, Ar = 15000, Allapot = Status.foglalt };

        _mockAppRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

        var result = await _controller.CreatePayment(createDto);

        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual("Ez az idõpont már ki van fizetve.", badRequest.Value);
    }

    [TestMethod]
    public async Task GetPaymentHistory_ForStudent_ReturnsOk()
    {

        var pay = new Pay { Felhasznalo_Id = 1, Idopont_Id = 1, Fizetes_Id = 1, Osszeg = 15000, Datum = DateTime.Now };
        _mockPayRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pay);

        var result = await _controller.GetById(1);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var displayDto = okResult.Value as PayDisplayDto;
        Assert.AreEqual(15000, displayDto?.Osszeg);
    }

    [TestMethod]
    public async Task GetPayment_InvalidId_ReturnsNotFound()
    {
        _mockPayRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Pay?)null);

        var result = await _controller.GetById(999);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}
