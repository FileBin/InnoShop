using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Domain.Abstraction;

namespace InnoShop.Application.Shared.Commands.Products;

public class DeleteProductCommand : ICommand {

    public required IUserDescriptor UserDesc { get; set; }

    public required string ProductId { get; set; }
}

public sealed class DeleteProductValidator : AbstractValidator<DeleteProductCommand> {
    public DeleteProductValidator() {
        RuleFor(x => x.UserDesc.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public class DeleteProductCommandHandler(IProductDbContext context)
 : IProductCommandHandler<DeleteProductCommand> {
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken) {
        var product = await context.Products.GetProductById(request.ProductId, cancellationToken);

        product.ValidateEdit(request.UserDesc);

        context.Products.Remove(product);
        context.TriggerSave();
    }
}
