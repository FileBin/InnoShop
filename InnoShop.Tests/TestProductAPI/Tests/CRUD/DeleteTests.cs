namespace InnoShop.Tests.TestProductAPI.Tests.CRUD;

class DeleteTests : CRUDTestsBase {

    private string normalID, freeID;

    public override async Task SetupAsync() {
        await base.SetupAsync();

        normalID = await CreatePositiveTest(NormalProduct);

        freeID = await CreatePositiveTest(FreeProduct);
    }

    [Test]
    public async Task DeleteNormal() {
        await DeletePositiveTest(normalID);
        await ReadNegativeTest(normalID);
        await ReadPositiveTest(freeID);
    }

    [Test]
    public async Task DeleteFree() {
        await DeletePositiveTest(freeID);
        await ReadNegativeTest(freeID);
        await ReadPositiveTest(normalID);
    }

    [Test]
    public async Task DeleteNotExistent() {
        var id = new Guid().ToString();
        
        await DeleteNegativeTest(id);
        await ReadPositiveTest(freeID);
        await ReadPositiveTest(normalID);
    }
}