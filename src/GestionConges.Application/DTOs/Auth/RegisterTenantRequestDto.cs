using System.ComponentModel.DataAnnotations;

namespace GestionConges.Application.DTOs.Auth
{
    public class RegisterTenantRequestDto
    {
        [Required(ErrorMessage = "Le nom de l'entreprise est requis")]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le sous-domaine est requis")]
        [RegularExpression(@"^[a-z0-9]+$", ErrorMessage = "Le sous-domaine ne peut contenir que des lettres minuscules et des chiffres")]
        [MaxLength(50)]
        public string Subdomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email de l'admin est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string AdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe de l'admin est requis")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
        public string AdminPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom de l'admin est requis")]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom de l'admin est requis")]
        public string AdminLastName { get; set; } = string.Empty;

        public string AdminPhone { get; set; } = string.Empty;
    }
}