namespace InnoShop.Tests.TestProductAPI.Tests.CRUD;

class CreateTests : CRUDTestsBase {

    [Test]
    public async Task CreateNormally() {
        await CreatePositiveTest(NormalProduct);
    }

    [Test]
    public async Task CreateFree() {
        await CreatePositiveTest(FreeProduct);
    }


    [Test]
    public async Task CreateEmptyDescription() {
        var product = FreeProduct;
        product.Description = "";
        await CreatePositiveTest(product);
    }

    [Test]
    public async Task CreateEmptyTitle() {
        var product = FreeProduct;
        product.Title = "";
        await CreateNegativeTest(product);
    }

    [Test]
    public async Task CreateNegativePrice() {
        var product = FreeProduct;
        product.Price = -12m;
        await CreateNegativeTest(product);
    }

    [Test]
    public async Task CreateLongDesc() {
        var product = FreeProduct;
        product.Description = TestGenerator.GenerateRandomString(2000);
        await CreateNegativeTest(product);
    }
}