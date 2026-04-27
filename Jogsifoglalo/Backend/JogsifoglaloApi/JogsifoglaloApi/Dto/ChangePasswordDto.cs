using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class ChangePasswordDto
    {
        [Required]
        public string RegiJelszo { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Az új jelszónak legalább 6 karakternek kell lennie!")]
        public string UjJelszo { get; set; } = string.Empty;
    }
}
