using API.DTOs.BusinessDTOs;
using API.Services.BusinessServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")] // <-- adnotacja authorize dodająca wymóg bycia zautentykowanym w aplikacji
                             // oraz spełnienia reguł, które ewentualnie mogą być w niej zawarte.
public class AdminController(ICustomerService customerService) : ControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> DeleteCustomerAsync(string pesel, CancellationToken token)
    {
        await customerService.DeleteCustomerAsync(pesel, token);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomerAsync([FromBody] InUpdateCustomerDto customer, CancellationToken token)
    {
        return Ok(await customerService.UpdateCustomerAsync(customer,token));
    }
}