using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminrepo;

        public AdminController(IAdminRepository adminrepo   )
        {
            _adminrepo = adminrepo;
        }

        [HttpGet("stats")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetStats()
        {
            var stats = await _adminrepo.GetDashboardStatsAsync();

            return Ok(stats);
        }

        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _adminrepo.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpPut("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserData(int id, [FromBody] UpdateUserAdminDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _adminrepo.UpdateUserAsync(id, dto);

            if (!success)
            {
                return NotFound(new { message = $"A(z) {id} azonosítójú felhasználó nem található!" });
            }

            return Ok(new { message = "Felhasználó adatai sikeresen frissítve!" });
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var delete = await _adminrepo.DeleteUserAsync(id);

            if (!delete)
            {
                return NotFound(new { message = $"A(z) {id} azonosítójú felhasználó nem található!" });
            }

            return Ok(new { message = "Felhasználó sikeresen törölve!" });
        }

        [HttpPatch("users/{id}/reset-password")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return BadRequest("Hibás a jelszó formátuma!");

            var hashedPw = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var success = await _adminrepo.ResetPasswordAsync(id, hashedPw);

            if (!success) return NotFound("Nem található a felhasználó!");

            return Ok(new { message = "Jelszó sikeresen frissítve!" });
        }

        [HttpGet("appointments")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetAppointment()
        {
            var appointment = await _adminrepo.GetAllAppointmentsAsync();

            return Ok(appointment);
        }

        [HttpPut("appointments/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentAdminDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _adminrepo.UpdateAppointmentAsync(id, dto);

            if (!success)
            {
                return NotFound(new { message = $"A(z) {id} azonosítójú időpont nem található!" });
            }

            return Ok(new { message = "Időpont adatai sikeresen frissítve!" });
        }

        [HttpDelete("appointments/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var delete = await _adminrepo.DeleteAppointmentAsync(id);

            if (!delete)
            {
                return NotFound(new { message = $"A(z) {id} azonosítójú időpont nem található!" });
            }

            return Ok(new { message = "Időpont sikeresen törölve!" });
        }

    }
}
