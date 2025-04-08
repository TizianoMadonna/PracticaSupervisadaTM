using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.Auth;

public class AccountController : Controller
{
    // Endpoint para el botón de Google
    [HttpPost]
    public async Task<IActionResult> HandleGoogleToken([FromBody] GoogleTokenModel model)
    {
        try
        {
            // 1. Validar el token JWT de Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(model.Credential);

            // 2. Crear identidad del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.Subject),
                new Claim(ClaimTypes.Email, payload.Email),
                new Claim(ClaimTypes.Name, payload.Name),
                new Claim("picture", payload.Picture)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Crear cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) // Cookie válida por 7 días
                });

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // Endpoint para logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

// Modelo para recibir el token
public class GoogleTokenModel
{
    public string Credential { get; set; }
}
