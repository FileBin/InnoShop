using InnoShop.Application.Shared.Models.Product;
using InnoShop.Domain.Entities;
using InnoShop.Domain.Enums;

namespace InnoShop.Tests.TestProductAPI.Tests;

class CRUDTestsBase : SetupProductAuth {
    public static CreateProductDto NormalProduct
    => new CreateProductDto(
            title: "A product",
            description: TestGenerator.GenerateRandomString(300),
            price: 12.00m
        );

    public static CreateProductDto FreeProduct
    => new CreateProductDto(
        title: "A Free product",
        description: TestGenerator.GenerateRandomString(300),
        price: 0.00m
    );

    public static UpdateProductDto UpdateAll
    => new UpdateProductDto(
        title: "Some Free product",
        description: TestGenerator.GenerateRandomString(300),
        price: 0.00m,
        status: AvailabilityStatus.Published
    );

    protected async Task<string> CreatePositiveTest(CreateProductDto createInfo) {
        var result = await CreateProduct(createInfo);

        Assert.That(result.IsSuccessStatusCode, Is.True);

        var content = await result.Content.ReadAsStringAsync();

        Assert.That(content, Is.Not.Empty);

        return content;
    }

    protected async Task CreateNegativeTest(CreateProductDto createInfo) {
        var result = await CreateProduct(createInfo);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    protected async Task<Product> ReadPositiveTest(string id) {
        var result = await GetProduct(id);
        Assert.That(result.IsSuccessStatusCode, Is.True);

        return await GetJsonContent<Product>(result);
    }

    protected async Task ReadNegativeTest(string id) {
        var result = await GetProduct(id);
        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    protected async Task UpdatePositiveTest(string id, UpdateProductDto updateInfo) {
        var result = await UpdateProduct(id, updateInfo);
        Assert.That(result.IsSuccessStatusCode, Is.True);
    }

    protected async Task UpdateNegativeTest(string id, UpdateProductDto updateInfo) {
        var result = await UpdateProduct(id, updateInfo);
        Assert.That(result.IsSuccessStatusCode, Is.False);
    }


    protected async Task DeletePositiveTest(string id) {
        var result = await DeleteProduct(id);
        Assert.That(result.IsSuccessStatusCode, Is.True);
    }

    protected async Task DeleteNegativeTest(string id) {
        var result = await DeleteProduct(id);
        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    protected async Task VerifyUpdate(string id, UpdateProductDto updateInfo) {
        var product = await ReadPositiveTest(id);

        if (updateInfo.Title is not null)
            Assert.That(product.Title, Is.EqualTo(updateInfo.Title));

        if (updateInfo.Description is not null)
            Assert.That(product.Description, Is.EqualTo(updateInfo.Description));

        if (updateInfo.Price.HasValue)
            Assert.That(product.Price, Is.EqualTo(updateInfo.Price.Value));

        if (updateInfo.Status.HasValue)
            Assert.That(product.Availability, Is.EqualTo(updateInfo.Status.Value));
    }
}
