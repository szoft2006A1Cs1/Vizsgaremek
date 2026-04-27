using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace JogsifoglaloApi.Tests.UnitTests;

[TestClass]
public class InstructorControllerTests
{
    private Mock<IInstructorRepository> _mockRepo = null!;
    private InstructorsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IInstructorRepository>();
        _controller = new InstructorsController(_mockRepo.Object);

        SetUserContext("1", "admin");
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

    private Instructor CreateFakeInstructor(int id, int userId, string name)
    {
        return new Instructor
        {
            Oktato_Id = id,
            Felhasznalo_Id = userId,
            Felhasznalo = new User
            {
                Felhasznalo_Id = userId,
                Nev = name,
                Email = "teszt@oktato.hu",
                Jelszo = "hashed",
                Telefonszam = "0630"
            },
            Instructs = new List<Instruct>()
        };
    }

    [TestMethod]
    public async Task GetAllInstructors_ReturnsList()
    {
        var instructors = new List<Instructor> { CreateFakeInstructor(1, 10, "Gipsz Jakab") };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(instructors);

        var result = await _controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as IEnumerable<InstructorDisplayDto>;
        Assert.AreEqual(1, response?.Count());
        Assert.AreEqual("Gipsz Jakab", response?.First().Nev);
    }

    [TestMethod]
    public async Task GetInstructorById_ValidId_ReturnsOk()
    { 
        var instructors = new List<Instructor> { CreateFakeInstructor(1, 10, "Gipsz Jakab") };
        _mockRepo.Setup(r => r.GetInstructorsByCategoryAsync("B")).ReturnsAsync(instructors);

        var result = await _controller.GetByCategory("B");

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public async Task GetInstructorById_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetInstructorsByCategoryAsync("Z")).ReturnsAsync(new List<Instructor>());

        var result = await _controller.GetByCategory("Z");

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task UpdateInstructor_OwnProfile_ReturnsOk()
    {
        SetUserContext("10", "oktato"); 
        var instructor = CreateFakeInstructor(1, 10, "Eredeti Név");
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(instructor);
        _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var updateDto = new InstructorUpdateDto { Nev = "Új Név" };

        var result = await _controller.UpdateInstructor(1, updateDto);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual("Új Név", instructor.Felhasznalo!.Nev);
    }

    [TestMethod]
    public async Task UpdateInstructor_OtherInstructorProfile_ReturnsForbidden()
    {
        SetUserContext("10", "oktato");
        var otherInstructor = CreateFakeInstructor(2, 99, "Másik Oktató");
        _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(otherInstructor);

        var updateDto = new InstructorUpdateDto { Nev = "Trollkodás" };

        var result = await _controller.UpdateInstructor(2, updateDto);

        var objectResult = result as ObjectResult;
        Assert.AreEqual(403, objectResult?.StatusCode);
    }

    [TestMethod]
    public async Task DeleteInstructor_WithActiveAppointments_ReturnsBadRequest()
    {
        _mockRepo.Setup(r => r.HasActiveAppointmentsAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteInstructor(1);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual("Nem törölhetõ oktató, akinek vannak teljesítetlen órái.", badRequest.Value);
    }

    [TestMethod]
    public async Task DeleteInstructor_AdminAction_ReturnsOk()
    {
        _mockRepo.Setup(r => r.HasActiveAppointmentsAsync(1)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.DeleteInstructorAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteInstructor(1);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        _mockRepo.Verify(r => r.DeleteInstructorAsync(1), Times.Once);
    }
}