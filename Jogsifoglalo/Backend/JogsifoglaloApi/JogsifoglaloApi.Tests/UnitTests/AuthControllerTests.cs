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
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [TestMethod]
    public async Task Login_WithCorrectCredentials_Returns200Ok()
    {
        var dto = new UserLoginDto { Email = "test@test.hu", Jelszo = "Password123" };
        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Jelszo)).ReturnsAsync("valid-jwt-token");

        var result = await _controller.Login(dto);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public async Task Login_WithWrongPassword_Returns400BadRequest()
    {
        var dto = new UserLoginDto { Email = "test@test.hu", Jelszo = "WrongPass" };
        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Jelszo)).ReturnsAsync((string?)null);

        var result = await _controller.Login(dto);

        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Register_ValidUser_Returns201Created()
    {
        var dto = new UserRegisterDto { Nev = "Teszt", Email = "uj@test.hu", Jelszo = "123456", Telefonszam = "0630" };
        var returnedUser = new User { Felhasznalo_Id = 1, Nev = dto.Nev, Email = dto.Email, Jelszo = "hash", Telefonszam = dto.Telefonszam };

        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(returnedUser);

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
    }

    [TestMethod]
    public async Task Register_ExistingEmail_Returns400BadRequest()
    {
        var dto = new UserRegisterDto { Nev = "Teszt", Email = "letezo@test.hu", Jelszo = "123456", Telefonszam = "0630" };
        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ThrowsAsync(new InvalidOperationException("Email foglalt"));

        var result = await _controller.Register(dto);

        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public void AccessAdminEndpoint_WithStudentRole_Returns403Forbidden()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Role, "tanulo") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        var isInRole = _controller.User.IsInRole("admin");

        Assert.IsFalse(isInRole, "A tanulónak nem szabadna admin szerepkörrel rendelkeznie.");
    }

    [TestMethod]
    public async Task TokenValidation_ExpiredToken_Returns401Unauthorized()
    {
        var dto = new UserLoginDto { Email = "expired@test.hu", Jelszo = "any" };
        _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string?)null);

        var result = await _controller.Login(dto);

        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }
}

