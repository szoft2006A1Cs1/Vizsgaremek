using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JogsifoglaloApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPayRepository _payrepo;
        private readonly IAppointmentRepository _appointmentrepo;

        public PaymentsController(IPayRepository payrepo, IAppointmentRepository appointmentrepo)
        {
            _payrepo = payrepo;
            _appointmentrepo = appointmentrepo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PayDisplayDto>> GetById(int id)
        {
            var pay = await _payrepo.GetByIdAsync(id);
            if (pay == null) return NotFound();

            var payDisplay = new PayDisplayDto
            {
                Fizetes_Id = pay.Fizetes_Id,
                Osszeg = pay.Osszeg,
                Datum = pay.Datum,
            };

            return Ok(payDisplay);
        }

        [HttpPost]
        [Authorize(Roles = "tanulo,admin")]
        public async Task<ActionResult<PayDisplayDto>> CreatePayment([FromBody] PayCreateDto createDto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentrepo.GetByIdAsync(createDto.Idopont_Id);
            if (appointment == null) return NotFound("Az időpont nem található.");

            if (appointment.Allapot == Status.foglalt) 
                return BadRequest("Ez az időpont már ki van fizetve.");

            var newPay = new Pay
            {
                Felhasznalo_Id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Idopont_Id = createDto.Idopont_Id,
                Osszeg = createDto.Osszeg,
                Datum = DateTime.Now,
            };

            await _payrepo.AddAsync(newPay);

            appointment.Allapot = Status.foglalt;
            appointment.Tanulo_Id = currentUserId;
            _appointmentrepo.Update(appointment);

            await _payrepo.SaveChangesAsync();
            await _appointmentrepo.SaveChangesAsync();

            var response = new PayDisplayDto
            {
                Fizetes_Id = newPay.Fizetes_Id,
                Osszeg = newPay.Osszeg,
                Datum = newPay.Datum,
            };

            return CreatedAtAction(nameof(GetById), new { id = newPay.Fizetes_Id }, response);
        }
    }
}
