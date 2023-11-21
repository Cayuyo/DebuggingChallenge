using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebuggingChallenge.Models;

namespace DebuggingChallenge.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Hint on a few errors: pay close attention to how things are named

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("create")]
    public IActionResult MakeUser(User newUser) // CreateUser fue cambiado a MakeUser para coincidir con el envio del formulario.
    {
        if(ModelState.IsValid)
        {
            HttpContext.Session.SetString("Username", newUser.Name);
            if(newUser.Location != null)
            {
                HttpContext.Session.SetString("Location", $"{newUser.Location}");
            } else {
                HttpContext.Session.SetString("Location", "Undisclosed");
            }
            return Redirect("Generate"); // reedirige a Generate para visualizar la pagina de generacion de codigo
        } else {
            return View("Index");
        }
    }

    [HttpGet("generate")]
    public IActionResult Generate()
    {
        if(HttpContext.Session.GetString("Username") == null) // HttpContext.Session.GetString("name") fue reemplazado por Username para validar la session del usuario 
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("Passcode") == null)
        {
            GeneratePasscode();
        }
        return View();
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // Hint: Something isn't right here...
    [HttpPost("generate/new")]
    public IActionResult GenerateNew()
    {
        GeneratePasscode(); // Genera un codigo automaticamente al acceder a la pagina
        return RedirectToAction("Generate");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void GeneratePasscode()
    {
        string passcode = "";
        string CharOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string NumOptions = "0123456789";
        Random rand = new Random();
        for(int i = 1; i < 15; i++)
        {
            int odds = rand.Next(2);
            if(odds == 0)
            {
                passcode += CharOptions[rand.Next(CharOptions.Length)];
            } else {
                passcode += NumOptions[rand.Next(NumOptions.Length)];
            }
        }
        HttpContext.Session.SetString("Passcode", passcode);
    }
}
