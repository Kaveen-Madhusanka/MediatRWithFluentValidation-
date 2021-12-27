using MediatR;
using MediatRWithValidation.Models;
using MediatRWithValidation.Services.Orders.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediatRWithValidation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<IActionResult> InsertOrder(Order order)=>  Ok(await _mediator.Send(new InsertOrderCommand(order)));

        //[HttpGet]
        //public async Task<IActionResult> GetOrder() => Ok(await _mediator.Send(new GetOrdersQuery()));

        [HttpPost]
        public async Task<IActionResult> Post( AddOrderCommand command)
            => Ok(await _mediator.Send(command));
    }
}
