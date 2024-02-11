using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Entities;

namespace InnoShop.Application.Shared.Commands.Products;

public class GetProductCommand : ICommand<Product> {

    public required IUserDescriptor UserDesc { get; set; }

    public required string ProductId { get; set; }
}

public sealed class GetProductValidator : AbstractValidator<GetProductCommand> {
    public GetProductValidator() {
        RuleFor(x => x.UserDesc.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public class GetProductCommandHandler(IProductDbContext context)
 : IProductCommandHandler<GetProductCommand, Product> {
    public async Task<Product> Handle(GetProductCommand request, CancellationToken cancellationToken) {
        var product = await context.Products.GetProductById(request.ProductId, cancellationToken);

        product.ValidateVisibility(request.UserDesc);

        return product;
    }
}
