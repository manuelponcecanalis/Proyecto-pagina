// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Pagina_proyecto.Areas.Data;

namespace Pagina_proyecto.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Nombre")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Apellido")]
            public string SecondName { get; set; }

            [Required]
            [RegularExpression(@"^\d+$", ErrorMessage = "El número de registro solo puede contener dígitos.")]
            [Display(Name = "Numero de registro")]
            public string NumeroRegistro { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    FirstName = Input.FirstName,
                    SecondName = Input.SecondName,
                    NumeroRegistro = int.TryParse(Input.NumeroRegistro, out var nro) ? nro : 0,
                    UserName = Input.Email,
                    Email = Input.Email
                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme,
                        host: new Uri(_configuration["AppUrl"] ?? $"{Request.Scheme}://{Request.Host}").Host);

                    var emailBody = $@"<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'/></head>
<body style='margin:0;padding:0;background-color:#f4f6f9;font-family:Segoe UI,Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f9;padding:40px 0;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
        <tr><td style='background-color:#003366;padding:32px 40px;text-align:center;'>
          <img src='https://proyectoeconomicas.org/Recurso_4-100-removebg-preview.png' alt='PROYECTO Económicas' style='max-width:260px;height:auto;'/>
        </td></tr>
        <tr><td style='background-color:#E91E63;height:4px;'></td></tr>
        <tr><td style='padding:48px 40px 32px 40px;'>
          <h1 style='color:#003366;font-size:26px;font-weight:700;margin:0 0 16px 0;'>¡Bienvenido/a a Proyecto Económicas!</h1>
          <p style='color:#333333;font-size:16px;line-height:1.7;margin:0 0 24px 0;'>Gracias por registrarte. Para completar tu registro y acceder a todos los contenidos de la plataforma, necesitamos verificar tu dirección de correo electrónico.</p>
          <p style='color:#333333;font-size:16px;line-height:1.7;margin:0 0 36px 0;'>Hacé clic en el botón para confirmar tu cuenta:</p>
          <table cellpadding='0' cellspacing='0' style='margin:0 auto 36px auto;'>
            <tr><td align='center' style='background-color:#E91E63;border-radius:8px;'>
              <a href='{callbackUrl}' target='_blank' style='display:inline-block;padding:16px 40px;color:#ffffff;font-size:16px;font-weight:700;text-decoration:none;'>Confirmar mi cuenta</a>
            </td></tr>
          </table>
          <p style='color:#666666;font-size:14px;margin:0 0 8px 0;'>Si el botón no funciona, copiá y pegá este enlace en tu navegador:</p>
          <p style='margin:0 0 32px 0;'><a href='{callbackUrl}' style='color:#003366;font-size:13px;word-break:break-all;'>{callbackUrl}</a></p>
          <hr style='border:none;border-top:1px solid #eeeeee;margin:0 0 28px 0;'/>
          <p style='color:#999999;font-size:13px;line-height:1.6;margin:0;'>Si no creaste una cuenta en Proyecto Económicas, podés ignorar este mensaje. Este enlace expira en 24 horas.</p>
        </td></tr>
        <tr><td style='background-color:#f4f6f9;padding:24px 40px;text-align:center;border-top:1px solid #eeeeee;'>
          <p style='color:#999999;font-size:12px;margin:0 0 8px 0;'>© 2024 PROYECTO Económicas. Todos los derechos reservados.</p>
          <p style='margin:0;'>
            <a href='https://www.instagram.com/proyectoeconomicas/' style='color:#003366;font-size:12px;text-decoration:none;margin:0 8px;'>Instagram</a>
            <a href='https://x.com/proyectofce' style='color:#003366;font-size:12px;text-decoration:none;margin:0 8px;'>X</a>
            <a href='https://www.youtube.com/proyectoeconomicas' style='color:#003366;font-size:12px;text-decoration:none;margin:0 8px;'>YouTube</a>
          </p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";

                    await _emailSender.SendEmailAsync(Input.Email, "Confirmá tu cuenta - Proyecto Económicas", emailBody);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}