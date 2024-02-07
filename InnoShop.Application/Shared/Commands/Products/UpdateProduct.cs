using InnoShop.Application.Shared.Exceptions;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Shared.Models.Product;
using InnoShop.Application.Validation;
using InnoShop.Domain.Abstraction;

namespace InnoShop.Application.Shared.Commands.Products;

public class UpdateProductCommand : UpdateProductDto, IUpdateProductDescriptor, ICommand {
    public UpdateProductCommand(UpdateProductDto other) : base(other) { }

    public UpdateProductCommand(string title, string description, decimal price) : base(title, description, price) { }

    public required IUserDescriptor UserDesc { get; init; }

    public required string ProductId { get; init; }
}

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand> {
    public UpdateProductValidator() {
        RuleFor(x => x.UserDesc.UserId).NotEmpty();
        When(x => x.Price is not null, () => {
            RuleFor(x => x.Price!.Value).PriceValidation();
        });
        When(x => x.Title is not null, () => {
            RuleFor(x => x.Title!).TitleValidation();
        });
        When(x => x.Description is not null, () => {
            RuleFor(x => x.Description!).DescriptionValidation();
        });
    }
}

public class UpdateProductCommandHandler(IProductDbContext context)
 : IProductCommandHandler<UpdateProductCommand> {
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken) {
        var productId = Guid.Parse(request.ProductId);

        var product = await context.Products.FindAsync([productId], cancellationToken);

        // make product invisible for others
        if (product is null || !product.IsEditableByUser(request.UserDesc))
            throw NotFoundException.NotFoundInDatabase("Product");

        if (request.Title is not null) {
            product.Title = request.Title;
        }

        if (request.Description is not null) {
            product.Description = request.Description;
        }

        if (request.Price is not null) {
            product.Price = request.Price.Value;
        }

        context.Products.Update(product);
        context.TriggerSave();
    }
}
