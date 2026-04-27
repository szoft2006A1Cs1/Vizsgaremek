using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentrepo;

        public AppointmentsController(IAppointmentRepository appointmentrepo)
        {
            _appointmentrepo = appointmentrepo;
        } 

        [HttpGet("available")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentDisplayDto>>> GetAvailable()
        {
            var freeAppointments = await _appointmentrepo.GetAvailableAsync();

            if (freeAppointments == null || !freeAppointments.Any())
            {
                return NotFound("Jelenleg nincs szabad időpont.");
            }

            var response = freeAppointments.Select(a => new AppointmentDisplayDto
            {
                Idopont_Id = a.Idopont_Id,
                Kezdes_Dt = a.Kezdes_Dt,
                Idotartam = a.Idotartam,
                Ar = a.Ar,
                Megjegyzes = a.Megjegyzes,
                Allapot = a.Allapot.ToString(),
                Oktato_Nev = a.Oktat?.Oktato?.Felhasznalo?.Nev ?? "Nincs név",
                Kategoria_Nev = a.Oktat?.Kategoria?.Kategoria_Nev ?? "Nincs kategória"
            });

            return Ok(response);
        }

        [HttpGet("available/{oktatoId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentDisplayDto>>> GetAvailableById(int oktatoId)
        {
            var freeAppointments = await _appointmentrepo.GetAvailableByIdAsync(oktatoId);

            if (freeAppointments == null || !freeAppointments.Any())
            {
                return NotFound("Nincs szabad időpont.");
            }

            var response = freeAppointments.Select(a => new AppointmentDisplayDto
            {
                Idopont_Id = a!.Idopont_Id,
                Kezdes_Dt = a.Kezdes_Dt,
                Idotartam = a.Idotartam,
                Ar = a.Ar,
                Megjegyzes = a.Megjegyzes,
                Allapot = a.Allapot.ToString(),
                Oktato_Nev = a.Oktat?.Oktato?.Felhasznalo?.Nev,
                Kategoria_Nev = a.Oktat?.Kategoria?.Kategoria_Nev
            });

            return Ok(response);
        }

        [HttpGet("my-bookings")]
        [Authorize(Roles = "tanulo")]

        public async Task<ActionResult<IEnumerable<AppointmentDisplayDto>>> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Hiányzó azonosító");
            int bejelentkezettUserId = int.Parse(userIdClaim);

            var bookings = await _appointmentrepo.GetByStudentIdAsync(bejelentkezettUserId);

            var response = bookings.Select(a => new AppointmentDisplayDto
            {
                Idopont_Id = a.Idopont_Id,
                Kezdes_Dt = a.Kezdes_Dt,
                Idotartam = a.Idotartam,
                Megjegyzes = a.Megjegyzes,
                Oktato_Nev = a.Oktat?.Oktato?.Felhasznalo?.Nev ?? "Ismeretlen",
                Kategoria_Nev = a.Oktat?.Kategoria?.Kategoria_Nev ?? "Nincs",
                Kategoria_Kod = a.Oktat?.Kategoria?.Kategoria_Kod ?? "Nincs",
                Ar = a.Ar,
                Allapot = a.Allapot.ToString()
            });

            return Ok(response);
        }

        [HttpGet("my-schedule")]
        [Authorize(Roles = "oktato")]
        public async Task<ActionResult<IEnumerable<AppointmentDisplayDto>>> GetMySchedule()
        {
            if (!User.IsInRole("oktato")) return StatusCode(403, "Csak az oktatók láthatják az órarendet.");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Nem található azonosító.");
            int bejelentkezettUserId = int.Parse(userIdClaim);

            var appointment = await _appointmentrepo.GetByInstructorUserIdAsync(bejelentkezettUserId);

            var response = appointment.Select(a => new AppointmentDisplayDto
            {
                Idopont_Id = a.Idopont_Id,
                Kezdes_Dt = a.Kezdes_Dt,
                Idotartam = a.Idotartam,
                Ar = a.Ar,
                Allapot = a.Allapot.ToString(),
                Megjegyzes = a.Megjegyzes,
                Kategoria_Kod = a.Oktat?.Kategoria?.Kategoria_Kod ?? "N/A",
                Kategoria_Nev = a.Oktat?.Kategoria?.Kategoria_Nev ?? "Ismeretlen",
                Oktato_Nev = a.Oktat?.Oktato?.Felhasznalo?.Nev ?? "Én",
                Tanulo_Nev = a.Tanulo?.Nev ?? "SZABAD IDŐPONT"
            });

            return Ok(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<AppointmentDisplayDto>>> GetAll()
        {
            if (!User.IsInRole("admin")) return StatusCode(403, "Csak adminisztrátorok kérhetik le az összes adatot.");
            var appointment = await _appointmentrepo.GetAllAsync();

            var response = appointment.Select(a => new AppointmentDisplayDto
            {
                Idopont_Id = a.Idopont_Id,
                Kezdes_Dt = a.Kezdes_Dt,
                Idotartam = a.Idotartam,
                Ar = a.Ar,
                Allapot = a.Allapot.ToString(),
                Oktato_Nev = a.Oktat?.Oktato?.Felhasznalo?.Nev ?? "N/A",
                Tanulo_Nev = a.Tanulo?.Nev ?? "SZABAD",
                Kategoria_Nev = a.Oktat?.Kategoria?.Kategoria_Nev ?? "N/A",
                Kategoria_Kod = a.Oktat?.Kategoria?.Kategoria_Kod ?? "N/A",
                Megjegyzes = a.Megjegyzes
            });

            return Ok(response);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetStats()
        {
            var appointments = await _appointmentrepo.GetAllAsync();

            var stats = appointments
                .Where(a => a.Allapot == Status.teljesitve)
                .GroupBy(a => a.Oktat?.Oktato?.Felhasznalo?.Nev ?? "Ismeretlen oktató")
                .Select(g => new
                {
                    Oktato = g.Key,
                    OsszesBevetel = g.Sum(a => a.Ar),
                    LevezetettOrak = g.Count(),
                    AtlagosOraAr = g.Average(a => a.Ar)
                });

            return Ok(stats);
        }

        [HttpPost("create")]
        [Authorize(Roles = "oktato,admin")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Hiányzó azonosító.");
            int bejelentkezettUserId = int.Parse(userIdClaim);

            var instructRecord = await _appointmentrepo.GetInstructByIdAsync(createDto.Oktat_Id);

            if (instructRecord == null)
            {
                return NotFound("A megadott oktatási kategória nem létezik.");
            }

            if (instructRecord.Oktato?.Felhasznalo_Id != bejelentkezettUserId && !User.IsInRole("admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Nem hirdethetsz időpontot más oktató nevében!");
            }

            if (createDto.Kezdes_Dt < DateTime.Now)
            {
                return BadRequest("Nem hozhatsz létre időpontot a múltba!");
            }

            var appointment = new Appointment
            {
                Oktat_Id = createDto.Oktat_Id,
                Kezdes_Dt = createDto.Kezdes_Dt,
                Idotartam = (ushort)createDto.Idotartam,
                Ar = createDto.Ar,
                Megjegyzes = createDto.Megjegyzes,
                Allapot = Status.szabad
            };

            await _appointmentrepo.AddAsync(appointment);
            var success = await _appointmentrepo.SaveChangesAsync();

            if (success)
            {
                return Ok(new { message = "Időpont sikeresen létrehozva!", id = appointment.Idopont_Id });
            }

            return BadRequest("Hiba történt a mentés során.");
        }

        [HttpPut("book/{id}")]
        [Authorize(Roles = "tanulo")]
        public async Task<IActionResult> BookAppointment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            var bejelentkezettUserId = int.Parse(userIdClaim);

            var appointment = await _appointmentrepo.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound("Nincs ilyen időpont.");
            }

            if (appointment.Allapot != Status.szabad)
            {
                return BadRequest("Ez az időpont már foglalt!");
            }

            appointment.Allapot = Status.foglalt;
            appointment.Tanulo_Id = bejelentkezettUserId;

            var success = await _appointmentrepo.SaveChangesAsync();

            if (success) return Ok("Sikeres foglalás!");

            return BadRequest("Hiba a mentés során.");
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "tanulo")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            var bejelentkezettUserId = int.Parse(userIdClaim);

            var appointment = await _appointmentrepo.GetByIdAsync(id);
            if (appointment == null) return NotFound("Nincs ilyen időpont.");

            if (appointment.Tanulo_Id != bejelentkezettUserId)
            {
                return StatusCode(403, "Csak a saját foglalásodat mondhatod le!");
            }

            if (appointment.Allapot.ToString() == "szabad")
            {
                return BadRequest("Ezt az időpontot nem lehet lemondani.");
            }

            appointment.Tanulo_Id = null;
            appointment.Allapot = Status.szabad; 

            await _appointmentrepo.UpdateAsync(appointment);
            return Ok("Sikeresen lemondtad az időpontot.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "oktato")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentUpdateDto updateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            var bejelentkezettUserId = int.Parse(userIdClaim);

            var appointment = await _appointmentrepo.GetByIdAsync(id);
            if (appointment == null) return NotFound("Nincs ilyen időpont.");

            if (appointment.Oktat?.Oktato?.Felhasznalo_Id != bejelentkezettUserId)
                return StatusCode(403, "Csak a saját időpontodat módosíthatod!");

            if (appointment.Allapot.ToString() != "szabad")
                return BadRequest("Lefoglalt időpont adatai nem módosíthatók!");

            appointment.Kezdes_Dt = updateDto.Kezdes_Dt;
            appointment.Idotartam = (ushort)updateDto.Idotartam;
            appointment.Ar = (int)updateDto.Ar;
            appointment.Megjegyzes = updateDto.Megjegyzes;

            await _appointmentrepo.UpdateAsync(appointment);

            return Ok("Időpont sikeresen frissítve.");
        }

        [HttpPut("complete/{id}")]
        [Authorize(Roles = "oktato,admin")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var appointment = await _appointmentrepo.GetByIdAsync(id);
            if (appointment == null) return NotFound();

            if (appointment.Allapot != Status.foglalt)
                return BadRequest("Csak lefoglalt órát lehet teljesítetté nyilvánítani.");

            appointment.Allapot = Status.teljesitve;
            await _appointmentrepo.SaveChangesAsync();

            return Ok("Az óra sikeresen lezárva (teljesítve).");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "oktato,admin")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            var bejelentkezettUserId = int.Parse(userIdClaim);

            var appointment = await _appointmentrepo.GetByIdAsync(id);
            if (appointment == null) return NotFound("Nincs ilyen időpont.");

            if (!User.IsInRole("admin") && appointment.Oktat?.Oktato?.Felhasznalo_Id != bejelentkezettUserId)
            {
                return StatusCode(403, "Csak a saját időpontodat törölheted!");
            }

            if (!User.IsInRole("admin") && appointment.Allapot != Status.szabad)
            {
                return BadRequest("Lefoglalt időpontot nem törölhetsz!");
            }

            await _appointmentrepo.DeleteAsync(id);
            await _appointmentrepo.SaveChangesAsync();

            return Ok("Időpont sikeresen törölve.");
        }
    }
}
