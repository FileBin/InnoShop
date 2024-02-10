using System.Net.Http.Headers;
using System.Net.Http.Json;
using InnoShop.Application;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Shared.Models.Product;
using InnoShop.Infrastructure.ProductManagerAPI;
using InnoShop.Infrastructure.ProductManagerAPI.Data;
using InnoShop.Tests.TestUserAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace InnoShop.Tests.TestProductAPI;

public class SetupProductApi : SetupUserApi {

    WebApplicationFactory<Program> webAppProductApi;
    protected HttpClient clientProductApi;

    protected void SetJwtToken(string value) {
        clientProductApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, value);
    }


    public override async Task SetupAsync() {
        var initUserTask = base.SetupAsync();

        webAppProductApi =
        new TestWebApplicationFactory<Program, ApplicationDbContext>() {
            DbName = "ProductDb"
        };
        clientProductApi = webAppProductApi.CreateClient();

        await initUserTask;
    }

    public override void Cleanup() {
        clientProductApi.Dispose();
        webAppProductApi.Dispose();

        base.Cleanup();
    }

    protected async Task<HttpResponseMessage> CreateProduct(CreateProductDto dto) {
        return await clientProductApi.PostAsJsonAsync("/api/products/create", dto);
    }

    protected async Task<HttpResponseMessage> GetProduct(string id) {
        return await clientProductApi.GetAsync($"/api/products/{id}");
    }

    protected async Task<HttpResponseMessage> UpdateProduct(string id, UpdateProductDto dto) {
        return await clientProductApi.PutAsJsonAsync($"/api/products/{id}", dto);
    }

    protected async Task<HttpResponseMessage> DeleteProduct(string id) {
        return await clientProductApi.DeleteAsync($"/api/products/{id}");
    }

    protected async Task<HttpResponseMessage> SearchProducts(SearchQueryDto dto) {
        var linkGenerator = new RouteBasedLinkGenerator() { Route = "/api/products/search" };
        return await clientProductApi.GetAsync(linkGenerator.GenerateLink(dto));
    }
}