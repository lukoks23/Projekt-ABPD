using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")] // <-- adnotacja authorize dodająca wymóg bycia zautentykowanym w aplikacji
                             // oraz spełnienia reguł, które ewentualnie mogą być w niej zawarte.
public class AdminController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Jestem adminem!");
    }
}