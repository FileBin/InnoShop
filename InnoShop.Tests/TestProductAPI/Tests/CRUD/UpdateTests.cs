using InnoShop.Application.Shared.Models.Product;
using InnoShop.Domain.Enums;

namespace InnoShop.Tests.TestProductAPI.Tests.CRUD;

class UpdateTests : CRUDTestsBase {

    private string normalID;

    public override async Task SetupAsync() {
        await base.SetupAsync();

        normalID = await CreatePositiveTest(NormalProduct);
    }

    [Test]
    public async Task UpdateAllTest() {
        var updateInfo = UpdateAll;
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateTitle() {
        var updateInfo = new UpdateProductDto(title: "A mogus");
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateTitleEmpty() {
        var updateInfo = new UpdateProductDto(title: "");
        await UpdateNegativeTest(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateTitleLong() {
        var updateInfo = new UpdateProductDto(title: TestGenerator.GenerateRandomString(2000));
        await UpdateNegativeTest(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateDescription() {
        var updateInfo = new UpdateProductDto(description: "A mogus");
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateDescriptionEmpty() {
        var updateInfo = new UpdateProductDto(description: "");
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateDescriptionLong() {
        var updateInfo = new UpdateProductDto(description: TestGenerator.GenerateRandomString(2000));
        await UpdateNegativeTest(normalID, updateInfo);
    }

    [Test]
    public async Task UpdatePrice() {
        var updateInfo = new UpdateProductDto(price: 2.00m);
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdatePriceFree() {
        var updateInfo = new UpdateProductDto(price: 0.00m);
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdatePriceNegative() {
        var updateInfo = new UpdateProductDto(price: -12.00m);
        await UpdateNegativeTest(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateAvailabilityPublished() {
        var updateInfo = new UpdateProductDto(status: AvailabilityStatus.Published);
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }

    [Test]
    public async Task UpdateAvailabilitySold() {
        var updateInfo = new UpdateProductDto(status: AvailabilityStatus.Sold);
        await UpdatePositiveTest(normalID, updateInfo);
        await VerifyUpdate(normalID, updateInfo);
    }
}