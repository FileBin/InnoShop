using InnoShop.Application;
using InnoShop.Application.Shared.Commands.Products;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Shared.Models.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InnoShop.Infrastructure.UserManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/products", Name = nameof(ProductsController))]
public class ProductsController : ControllerBase {
    private readonly IMediator mediator;

    public ProductsController(IMediator mediator) {
        this.mediator = mediator;
    }

    [HttpPost]
    [Route("create", Name = nameof(Create))]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken) {
        var command = new CreateProductCommand(dto) {
            UserDesc = ClaimUserDescriptor.From(User),
        };

        var result = await mediator.Send(command, cancellationToken);

        return Created(Url.Link(nameof(Get), new { id = result }), result);
    }

    [HttpGet]
    [Route("{id}", Name = nameof(Get))]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken) {
        var command = new GetProductCommand {
            ProductId = id,
            UserDesc = ClaimUserDescriptor.From(User),
        };

        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("{id}", Name = nameof(Update))]
    public async Task<IActionResult> Update([FromRoute] string id,
                                            [FromBody] UpdateProductDto dto,
                                            CancellationToken cancellationToken) {
        var command = new UpdateProductCommand(dto) {
            ProductId = id,
            UserDesc = ClaimUserDescriptor.From(User),
        };

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("{id}", Name = nameof(Delete))]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken) {
        var command = new DeleteProductCommand {
            ProductId = id,
            UserDesc = ClaimUserDescriptor.From(User),
        };

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    [Route("search", Name = nameof(Search))]
    public async Task<IActionResult> Search([FromQuery, BindRequired] SearchQueryDto dto, CancellationToken cancellationToken) {
        var command = new SearchProductsCommand(dto) {
            UserDesc = ClaimUserDescriptor.From(User),
        };

        var result = await mediator.Send(command, cancellationToken);
    
        return Ok(result);
    }
}