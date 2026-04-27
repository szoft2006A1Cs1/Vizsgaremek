using JogsifoglaloApi.Controllers;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Repositories;
using JogsifoglaloApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JogsifoglaloApi.Tests.UnitTests
{
    [TestClass]
    public class CategoryControllerTests
    {
        private Mock<ICategoryRepository> _mockRepo = null!;
        private CategoriesController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _controller = new CategoriesController(_mockRepo.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkWithList()
        {
            var categories = new List<Category>
            {
                new Category { Kategoria_Kod = "B", Kategoria_Nev = "Személyautó", Oradij = 10000, Leiras = "Autó" },
                new Category{ Kategoria_Kod = "A", Kategoria_Nev = "Motor", Oradij = 8000, Leiras = "Motor" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _controller.GetAll();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }


        [TestMethod]
        public async Task Create_ValidData_ReturnsCreatedAtAction()
        {
            var dto = new CategoryDto
            {
                Kategoria_Kod = "C",
                Kategoria_Nev = "Teherautó",
                Oradij = 15000,
                Leiras = "Nehézgépjármû"
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _controller.Create(dto);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(nameof(_controller.GetAll), createdResult.ActionName);
        }


        [TestMethod]
        public async Task Delete_ExistingId_ReturnsOk()
        {
            int testId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(testId)).ReturnsAsync(true);

            var result = await _controller.Delete(testId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Delete_NonExistingId_ReturnsNotFound()
        {
            int testId = 99;
            _mockRepo.Setup(r => r.DeleteAsync(testId)).ReturnsAsync(false);

            var result = await _controller.Delete(testId);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Kategória nem található.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task Delete_WhenCategoryHasDependencies_ReturnsBadRequest()
        {
            int testId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(testId))
                     .ThrowsAsync(new InvalidOperationException("A kategória nem törölhetõ, mert aktív órák tartoznak hozzá."));

            var result = await _controller.Delete(testId);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
            Assert.AreEqual("A kategória nem törölhetõ, mert aktív órák tartoznak hozzá.", badRequest.Value);
        }

        [TestMethod]
        public async Task Delete_UnexpectedError_Returns500InternalServerError()
        {
            int testId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(testId)).ThrowsAsync(new Exception("Adatbázis hiba"));

            var result = await _controller.Delete(testId);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Váratlan hiba történt a törlés során.", statusCodeResult.Value);
        }
    }
}