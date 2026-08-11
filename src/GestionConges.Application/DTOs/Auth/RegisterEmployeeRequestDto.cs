using System.ComponentModel.DataAnnotations;

namespace GestionConges.Application.DTOs.Auth
{
    public class RegisterEmployeeRequestDto
    {
        [Required(ErrorMessage = "Le token d'invitation est requis")]
        public string InvitationToken { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string LastName { get; set; }

        public string Phone { get; set; }
    }
}