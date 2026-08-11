using GestionConges.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Common
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.Success)
            {
                return controller.Ok(result.Data);
            }

            // Gestion des erreurs selon le message
            var error = result.Error?.ToLower() ?? "";

            if (error.Contains("not found") || error.Contains("introuvable"))
                return controller.NotFound(new { message = result.Error });

            if (error.Contains("unauthorized") || error.Contains("non autorisé"))
                return controller.Unauthorized(new { message = result.Error });

            if (error.Contains("forbidden") || error.Contains("interdit"))
                return controller.Forbid();

            if (error.Contains("conflict") || error.Contains("conflit"))
                return controller.Conflict(new { message = result.Error });

            if (error.Contains("validation") || error.Contains("invalide"))
                return controller.BadRequest(new { message = result.Error });

            // Par défaut
            return controller.BadRequest(new { message = result.Error });
        }
    }
}
