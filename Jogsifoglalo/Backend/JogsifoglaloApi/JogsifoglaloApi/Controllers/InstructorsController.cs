using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly IInstructorRepository _instructorrepo;

        public InstructorsController(IInstructorRepository instructorrepo)
        {
            _instructorrepo = instructorrepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorDisplayDto>>> GetAll()
        {
            var instructors = await _instructorrepo.GetAllAsync();

            var response = instructors.Select(i => new InstructorDisplayDto
            {
                Oktato_Id = i.Oktato_Id,
                Nev = i.Felhasznalo?.Nev ?? "Névtelen oktató",
                Email = i.Felhasznalo?.Email ?? "Nincs email",
                Telefonszam = i.Felhasznalo?.Telefonszam ?? "Nincs telefonszám",
                Cim = i.Felhasznalo?.Cim ?? "Nincsen cím",
                Kategoriak = i.Instructs
                    .Select(ins => ins.Kategoria != null ? ins.Kategoria.Kategoria_Kod : "Ismeretlen")
                    .ToList()
            });

            return Ok(response);
        }

        [HttpGet("by-category/{categoryCode}")]
        public async Task<ActionResult<IEnumerable<InstructorDisplayDto>>> GetByCategory(string categoryCode)
        {
            var instructors = await _instructorrepo.GetInstructorsByCategoryAsync(categoryCode);

            if (instructors == null || !instructors.Any())
            {
                return NotFound($"Nem találtunk oktatót a(z) '{categoryCode}' kategóriához.");
            }

            var response = instructors.Select(i => new InstructorDisplayDto
            {
                Oktato_Id = i!.Oktato_Id,
                Nev = i.Felhasznalo?.Nev ?? "Névtelen oktató",
                Email = i.Felhasznalo?.Email ?? "Nincs email",
                Telefonszam = i.Felhasznalo?.Telefonszam ?? "Nincs telefonszám",
                Cim = i.Felhasznalo?.Cim ?? "Nincsen cím",
                Kategoriak = i.Instructs
                    .Select(ins => ins.Kategoria?.Kategoria_Kod ?? "Ismeretlen")
                    .ToList()
            });

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,oktato")]
        public async Task<IActionResult> UpdateInstructor(int id, [FromBody] InstructorUpdateDto updateDto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var instructor = await _instructorrepo.GetByIdAsync(id);
            if (instructor == null) return NotFound("Oktató nem található.");

            if (userRole == "oktato" && instructor.Felhasznalo_Id.ToString() != userIdFromToken)
            {
                return StatusCode(403, "Nincs jogosultságod más oktató adatait módosítani!");
            }

            if (instructor.Felhasznalo != null)
            {
                if (!string.IsNullOrEmpty(updateDto.Nev)) instructor.Felhasznalo.Nev = updateDto.Nev;
                if (!string.IsNullOrEmpty(updateDto.Email)) instructor.Felhasznalo.Email = updateDto.Email;
                if (!string.IsNullOrEmpty(updateDto.Telefonszam)) instructor.Felhasznalo.Telefonszam = updateDto.Telefonszam;
                if (!string.IsNullOrEmpty(updateDto.Cim)) instructor.Felhasznalo.Cim = updateDto.Cim;
            }

            if (updateDto.UjKategoriak != null)
            {
                var toRemove = instructor.Instructs
                        .Where(ins => ins.Kategoria != null &&
                                     !updateDto.UjKategoriak.Contains(ins.Kategoria.Kategoria_Kod ?? ""))
                        .ToList();

                foreach (var item in toRemove)
                {
                    instructor.Instructs.Remove(item);
                }

                foreach (var katKod in updateDto.UjKategoriak)
                {
                    var kategoria = await _instructorrepo.GetCategoryByNameOrCodeAsync(katKod);
                    if (kategoria != null && !instructor.Instructs.Any(i => i.Kategoria_Id == kategoria.Kategoria_Id))
                    {
                        var ujKapcsolat = new Instruct
                        {
                            Oktato_Id = instructor.Oktato_Id,
                            Kategoria_Id = kategoria.Kategoria_Id
                        };

                        instructor.Instructs.Add(ujKapcsolat);
                    }
                }
            }

            _instructorrepo.Update(instructor);
            await _instructorrepo.SaveChangesAsync();

            return Ok(new { message = "Oktató adatai és kategóriái sikeresen frissítve!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {

            var activeAppointment = await _instructorrepo.HasActiveAppointmentsAsync(id);
            if (activeAppointment) return BadRequest("Nem törölhető oktató, akinek vannak teljesítetlen órái.");
            
            var success = await _instructorrepo.DeleteInstructorAsync(id);

            if (!success) return NotFound("Az oktató nem található vagy a törlés sikertelen.");

            return Ok(new { message = "Oktató és a hozzá tartozó felhasználó sikeresen törölve." });
        }
    }
}
