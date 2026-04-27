using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userrepo;

        public UsersController(IUserRepository userrepo)
        {
            _userrepo = userrepo;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDisplayDto>> GetMyProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();

            int userId = int.Parse(userIdString);
            var user = await _userrepo.GetByIdAsync(userId);

            if (user == null) return NotFound("Felhasználó nem található.");

            var displayDto = new UserDisplayDto
            {
                Felhasznalo_Id = user.Felhasznalo_Id,
                Nev = user.Nev,
                Email = user.Email,
                Telefonszam = user.Telefonszam,
                Cim = user.Cim,
                Szerepkor = user.Szerepkor_Nev.ToString().ToLower()
            };

            return Ok(displayDto);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserDisplayDto>>> GetAllUsers()
        {
            var users = await _userrepo.GetAllAsync();

            var userDtos = users.Select(user => new UserDisplayDto
            {
                Felhasznalo_Id = user.Felhasznalo_Id,
                Nev = user.Nev,
                Email = user.Email,
                Telefonszam = user.Telefonszam,
                Cim = user.Cim,
                Szerepkor = user.Szerepkor_Nev.ToString().ToLower()
            });

            return Ok(userDtos);
        }

        [HttpPut("update-me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserUpdateDto updateDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _userrepo.GetByIdAsync(userId);
            if (user == null) return NotFound("A felhasználó nem található.");

            if (!string.IsNullOrEmpty(updateDto.Nev)) user.Nev = updateDto.Nev;
            if (!string.IsNullOrEmpty(updateDto.Email)) user.Email = updateDto.Email;
            if (!string.IsNullOrEmpty(updateDto.Telefonszam)) user.Telefonszam = updateDto.Telefonszam;
            if (!string.IsNullOrEmpty(updateDto.Cim)) user.Cim = updateDto.Cim;

            _userrepo.Update(user);
            await _userrepo.SaveChangesAsync();

            return Ok(new { message = "Profil Sikeresen frissítve!" });
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userrepo.GetByIdAsync(userId);

            if (user == null) return NotFound("Felhasználó nem található.");

            if (!BCrypt.Net.BCrypt.Verify(request.RegiJelszo, user.Jelszo))
            {
                return BadRequest("A régi jelszó nem megfelelő!");
            }

            user.Jelszo = BCrypt.Net.BCrypt.HashPassword(request.UjJelszo);

            _userrepo.Update(user);
            await _userrepo.SaveChangesAsync();

            return Ok(new { message = "Jelszó sikeresen megváltoztatva!" });
        }

        [HttpDelete("delete-me")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userrepo.GetByIdAsync(userId);

            if (user == null) return NotFound("Felhasználó nem található.");

            _userrepo.Delete(user);
            await _userrepo.SaveChangesAsync();

            return Ok(new { message = "Felhasználó sikeresen törölve." });
        }
    }
}
