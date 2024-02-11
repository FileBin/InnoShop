namespace InnoShop.Tests.TestProductAPI.Tests.CRUD;

class ReadTests : CRUDTestsBase {

    private string normalID, freeID;

    public override async Task SetupAsync() {
        await base.SetupAsync();

        normalID = await CreatePositiveTest(NormalProduct);

        freeID = await CreatePositiveTest(FreeProduct);
    }

    [Test]
    public async Task GetNormal() {
        var result = await ReadPositiveTest(normalID);
        Assert.That(result.Id.ToString(), Is.EqualTo(normalID));
        Assert.That(result.Title, Is.EqualTo(NormalProduct.Title));
    }

    [Test]
    public async Task GetFree() {
        var result = await ReadPositiveTest(freeID);
        Assert.That(result.Id.ToString(), Is.EqualTo(freeID));
        Assert.That(result.Title, Is.EqualTo(FreeProduct.Title));
    }

    [Test]
    public async Task GetNotExistent() {
        var id = new Guid().ToString();
        await ReadNegativeTest(id);
    }
}