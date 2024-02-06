using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Infrastructure.UserManagerAPI.Controllers;

[ApiController]
[Route("api/products", Name = nameof(ProductsController))]
[Authorize]
public class ProductsController : ControllerBase {
    private readonly IMediator mediator;

    public ProductsController(IMediator mediator) {
        this.mediator = mediator;
    }

    [HttpPost]
    [Route("test", Name = nameof(Test))]
    public IActionResult Test([FromBody] string name) {
        return Ok($"Hello {name}!");
    }
}