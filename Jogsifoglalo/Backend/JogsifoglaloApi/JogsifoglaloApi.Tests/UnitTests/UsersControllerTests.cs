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
public class UsersControllerTests
{
    private Mock<IUserRepository> _mockRepo = null!;
    private UsersController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IUserRepository>();
        _controller = new UsersController(_mockRepo.Object);

        SetUserContext("1", "tanulo");
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

    [TestMethod]
    public async Task GetProfile_ValidUser_ReturnsOk()
    {
        var user = new User { Felhasznalo_Id = 1, Nev = "Teszt Elek", Email = "teszt@gmail.com", Jelszo = "hash",
        Telefonszam = "0630", Szerepkor_Nev = Role.tanulo };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _controller.GetMyProfile();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var displayDto = okResult.Value as UserDisplayDto;
        Assert.AreEqual("Teszt Elek", displayDto?.Nev);
    }

    [TestMethod]
    public async Task GetProfile_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var result = await _controller.GetMyProfile();

        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual("Felhasználó nem található.", notFoundResult.Value);
    }

    [TestMethod]
    public async Task UpdateUser_OwnProfile_ReturnsOk()
    {
        var user = new User { Felhasznalo_Id = 1, Nev = "Régi Név", Email = "teszt@gmail.com", Jelszo = "hash", Telefonszam = "0630" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var updateDto = new UserUpdateDto { Nev = "Új Név" };

        var result = await _controller.UpdateMyProfile(updateDto);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual("Új Név", user.Nev);
        _mockRepo.Verify(r => r.Update(user), Times.Once);
    }

    [TestMethod]
    public async Task UpdateUser_OtherProfile_ReturnsForbidden()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        await Assert.ThrowsExceptionAsync<System.ArgumentNullException>(async () => {
            await _controller.UpdateMyProfile(new UserUpdateDto());
        });
    }

    [TestMethod]
    public async Task DeleteUser_AdminAction_ReturnsOk()
    {
        SetUserContext("1", "admin");
        var user = new User { Felhasznalo_Id = 1, Nev = "Admin", Email = "a@a.hu", Jelszo = "h", Telefonszam = "1" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _controller.DeleteUser();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        _mockRepo.Verify(r => r.Delete(user), Times.Once);
    }

    [TestMethod]
    public async Task GetPaymentHistory_Unauthorized_ReturnsUnauthorized()
    {
        SetUserContext("1", "tanulo");

        var result = await _controller.GetAllUsers();

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }
}
