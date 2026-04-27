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
    public class AdminMaintenanceFlowTests
    {
        private Mock<IAdminRepository> _mockAdminRepo = null!;
        private AdminController _adminController = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAdminRepo = new Mock<IAdminRepository>();
            _adminController = new AdminController(_mockAdminRepo.Object);
        }

        private void SetAdminContext()
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [TestMethod]
        public async Task Scenario_Admin_CheckStats_AndDeleteUser_Flow()
        {
            SetAdminContext();

            var fakeStats = new AdminDashboardDto { OsszesFelhasznalo = 10, MaiIdopontokSzama = 50 };
            _mockAdminRepo.Setup(r => r.GetDashboardStatsAsync()).ReturnsAsync(fakeStats);

            _mockAdminRepo.Setup(r => r.DeleteUserAsync(66)).ReturnsAsync(true);

            var statsResult = await _adminController.GetStats();
            var okStats = statsResult as OkObjectResult;
            Assert.IsNotNull(okStats);
            Assert.AreEqual(fakeStats, okStats.Value);

            var deleteResult = await _adminController.DeleteUser(66);

            Assert.IsInstanceOfType(deleteResult, typeof(OkObjectResult));

            _mockAdminRepo.Verify(r => r.DeleteUserAsync(66), Times.Once);
        }

        [TestMethod]
        public async Task Scenario_Admin_DeleteNonExistentUser_ReturnsNotFound()
        {
            SetAdminContext();
            _mockAdminRepo.Setup(r => r.DeleteUserAsync(999)).ReturnsAsync(false);

            var result = await _adminController.DeleteUser(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
