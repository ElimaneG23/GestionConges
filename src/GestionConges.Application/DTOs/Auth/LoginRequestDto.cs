using GestionConges.Application.DTOs.Tenant;
using GestionConges.Application.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace GestionConges.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string Password { get; set; } = string.Empty;

        // Sous-domaine non requis pour login via email
    }

   
}