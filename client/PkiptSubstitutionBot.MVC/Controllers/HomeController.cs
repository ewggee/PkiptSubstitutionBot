using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PkiptSubstitutionBot.client.Application.DTOs;
using PkiptSubstitutionBot.client.Application.Services;
using PkiptSubstitutionBot.client.MVC.Models;
using System.Security.Claims;
using PkiptSubstitutionBot.client.Application.Consts;

namespace PkiptSubstitutionBot.client.MVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AdminService _adminService;
    private readonly RabbitMqService _rabbitMqService;

    public HomeController(
        ILogger<HomeController> logger, 
        AdminService adminService, 
        RabbitMqService rabbitMqService)
    {
        _logger = logger;
        _adminService = adminService;
        _rabbitMqService = rabbitMqService;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var result = await _adminService.Login(loginRequest);

        if (result == true)
        {
            var claims = new List<Claim>
            {
                new("username", loginRequest.Username)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginRequest.RememberMe,
                ExpiresUtc = loginRequest.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction(nameof(UploadImage));
        }

        ViewBag.Message = "Неверный логин или пароль";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}

    public IActionResult UploadImage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IEnumerable<IFormFile> images, DateOnly date, string? messageText)
    {
        var sendSubRequest = new SendSubRequest
        {
            Images = [],
            Date = date,
            MessageText = messageText
        };

        foreach (var image in images)
        {
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);

            sendSubRequest.Images.Add(new ImageData
            {
                FileName = image.FileName,
                ContentType = image.ContentType,
                Data = ms.ToArray()
            });
        }

        await _rabbitMqService.SendMessageAsync(sendSubRequest, RabbitMqQueueNameConsts.Subst);

        ViewBag.Message = $"Замены отправлены!";
        return View();
    }

    public IActionResult SendMessage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string message)
    {
        await _rabbitMqService.SendMessageAsync(message, RabbitMqQueueNameConsts.Messages);

        ViewBag.Message = "Сообщение отправлено!";
        return View();
    }

    public async Task<IActionResult> Administrators()
    {
        var currentUsername = User.FindFirst("username")?.Value!;
        var admins = await _adminService.GetAdminsAsync(currentUsername);

        ViewBag.Message = TempData["Message"]?.ToString();
        return View(admins.Select(a => new AdminViewModel { Username = a.Username }));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAdmin(string username)
    {
        await _adminService.DeleteAdminByUsername(username);

        TempData["Message"] = "Администратор успешно удалён!";
        return RedirectToAction(nameof(Administrators));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin([FromForm] RegisterRequest registerRequest)
    {
        if (await _adminService.IsAdminExists(registerRequest.Username))
        {
            TempData["Message"] = "Администратор с таким логином уже зарегистрирован";
            return RedirectToAction(nameof(Administrators));
        }

        await _adminService.Register(registerRequest);

        TempData["Message"] = $"Администратор успешно добавлен!";
        return RedirectToAction(nameof(Administrators));
    }
}
