using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryrepo;

        public CategoriesController(ICategoryRepository categoryrepo)
        {
            _categoryrepo = categoryrepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryrepo.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CategoryDto>> Create(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Kategoria_Kod = categoryDto.Kategoria_Kod,
                Kategoria_Nev = categoryDto.Kategoria_Nev,
                Oradij = categoryDto.Oradij,
                Leiras = categoryDto.Leiras
            };

            await _categoryrepo.AddAsync(category);
            await _categoryrepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = category.Kategoria_Id }, categoryDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _categoryrepo.DeleteAsync(id);
                if (!success) return NotFound("Kategória nem található.");

                return Ok(new { message = "Kategória sikeresen törölve." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Váratlan hiba történt a törlés során.");
            }
        }
    }
}
