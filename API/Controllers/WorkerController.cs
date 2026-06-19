using API.DTOs.BusinessDTOs;
using API.Services.BusinessServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize]
public class WorkerController(ICustomerService customerService, IContractService contractService, IAccountantService accountantService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCustomerAsync([FromBody] InCustomerDto customer, CancellationToken token)
    {
        return Created("",await customerService.AddCustomerAsync(customer,token));
    }

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

    [HttpPost]
    [Route("/contract/")]
    public async Task<IActionResult> AddContractAsync([FromBody] InContractDto inContractDto, CancellationToken token)
    {
        return Created("",await contractService.AddContractAsync(inContractDto, token));
    }

    [HttpPost]
    [Route("/payment/")]
    public async Task<IActionResult> AddPaymentAsync([FromBody] InPaymentDto paymentDto, CancellationToken token)
    {
        return Created("",await contractService.AddPaymentAsync(paymentDto, token));
    }

    [HttpGet]
    public async Task<IActionResult> GetRealIncome([FromQuery] int? softwareId,[FromQuery] string? currencyCode,[FromQuery] bool? expected, CancellationToken token)
    {
        return Ok(await accountantService.GetIncomeAsync(softwareId,currencyCode,expected, token));
    }
}