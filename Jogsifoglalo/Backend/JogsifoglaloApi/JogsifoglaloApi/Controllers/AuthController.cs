using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {
            try
            {
                var user = new User
                {
                    Nev = request.Nev,
                    Email = request.Email,
                    Telefonszam = request.Telefonszam,
                    Cim = request.Cim,
                    Jelszo = string.Empty,
                    Szerepkor_Nev = Role.tanulo,
                };

                var registeredUser = await _authService.RegisterAsync(user, request.Jelszo);
                return Created($"{Request.GetDisplayUrl()}/{user.Felhasznalo_Id}",registeredUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            { 
                return StatusCode(500, "Váratlan hiba történt a regisztráció során.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Jelszo);

            if (token == null)
            {
                return BadRequest("Hibás email vagy jelszó!");
            }

            return Ok(token);
        }
    }
}
