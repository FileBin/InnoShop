using System.Net;

namespace InnoShop.Tests.TestProductAPI.Tests.Access;

class AccessTests : SetupProductApi {
    [Test]
    public async Task CreateUnauthorizedAsync() {
        var result = await CreateProduct(CRUDTestsBase.NormalProduct);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetUnauthorizedAsync() {
        var result = await GetProduct(new Guid().ToString());
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task UpdateUnauthorizedAsync() {
        var result = await UpdateProduct(new Guid().ToString(), CRUDTestsBase.UpdateAll);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteUnauthorizedAsync() {
        var result = await DeleteProduct(new Guid().ToString());
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}