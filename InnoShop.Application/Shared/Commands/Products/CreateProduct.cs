using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Models.Product;
using InnoShop.Application.Validation;
using InnoShop.Domain.Abstraction;
using InnoShop.Domain.Services;

namespace InnoShop.Application.Shared.Commands.Products;

public class CreateProductCommand : CreateProductDto, ICreateProductDescriptor, ICommand<string> {
    public CreateProductCommand(CreateProductDto other) : base(other) { }

    public CreateProductCommand(string title, decimal price, string description = "") : base(title, price, description) { }

    public required IUserDescriptor UserDesc { get; init; }
}

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand> {
    public CreateProductValidator() {
        RuleFor(x => x.UserDesc.UserId).NotEmpty();
        RuleFor(x => x.Price).PriceValidation();
        RuleFor(x => x.Title).TitleValidation();
        RuleFor(x => x.Description).DescriptionValidation();
    }
}

public class CreateProductCommandHandler(IProductDbContext context, IProductFactory productFactory)
 : IProductCommandHandler<CreateProductCommand, string> {
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken) {
        var product = productFactory.Create(request);
        await context.Products.AddAsync(product, cancellationToken);

        context.TriggerSave();

        return product.Id.ToString();
    }
}
