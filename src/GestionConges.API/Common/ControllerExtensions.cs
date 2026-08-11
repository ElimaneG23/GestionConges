using GestionConges.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace GestionConges.API.Common;

/// <summary>Convertit un Result&lt;T&gt; applicatif en IActionResult HTTP homogène.</summary>
public static class ControllerExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.Success) return controller.Ok(result.Data);
        return controller.BadRequest(new { error = result.Error });
    }
}
