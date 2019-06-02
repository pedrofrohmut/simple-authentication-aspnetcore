using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace Mvc.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => 
      View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [HttpGet]
    public IActionResult ErrorForbidden() => View();

    [HttpGet]
    public IActionResult ErrorNotLoggedIn() => View();

    [HttpPost] // autobind to form name submitted
    public async Task<IActionResult> Login(string name) 
    {
      // Validate if the form is empty
      if (string.IsNullOrEmpty(name)) 
      {
        return RedirectToAction(nameof(Index));
      }

      // Identity: an array of Claims that represents the information abou a User
      // Claims: key/value pairs
      // In a production App you would pass more information that just the name
      ClaimsIdentity identity = new ClaimsIdentity(
        new Claim[] 
        {
          new Claim(ClaimTypes.Name, name),
          new Claim(ClaimTypes.Role, "admin")
        }, 
        CookieAuthenticationDefaults.AuthenticationScheme
      );

      ClaimsPrincipal principal = new ClaimsPrincipal(identity);  

      await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal
      );

      return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      await HttpContext
        .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      
      return RedirectToAction(nameof(Index));
    }

    // Action Level Authorization
    [Authorize(Policy = "MustBeAdmin")] 
    [HttpGet]
    public IActionResult Manage() => View();
  }
}
