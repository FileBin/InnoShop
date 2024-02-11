using InnoShop.Application;
using InnoShop.Application.Shared.Models.Product;
using InnoShop.Domain.Entities;
using InnoShop.Domain.Enums;

namespace InnoShop.Tests.TestProductAPI.Tests.Search;

class SearchTestsBase : SetupProductAuth {

    protected SearchQueryDto EmptyQuery
    => new SearchQueryDto() {
        From = 0,
        To = 20,
    };

    protected List<Product> testProducts;

    public static List<CreateProductDto> testProductInfosList = [
        CRUDTestsBase.FreeProduct,
        CRUDTestsBase.NormalProduct,

        new CreateProductDto(title: "Mogus", price: 6.00m),
        new CreateProductDto(title: "A Free Mogus", price: 0.00m),
        new CreateProductDto(title: "A Mogus", price: 8.00m),

        new CreateProductDto(title: "Sus", price: 6.00m),
        new CreateProductDto(title: "A Free SUS", price: 0.00m),
        new CreateProductDto(title: "Sussy Mogus", price: 11.00m),

        new CreateProductDto(title: "Amogus", price: 21.00m),
    ];

    public override async Task SetupAsync() {
        await base.SetupAsync();

        var infos = testProductInfosList;


        testProducts = infos.Select(x => (Product?)null).ToList()!;

        var ids = infos.Select(x => (string)null!).ToList();

        var n = testProducts.Count;

        for (int i = 0; i < n; i++) {
            var createInfo = infos[i];
            var result = await CreateProduct(createInfo);

            Assert.That(result.IsSuccessStatusCode, Is.True);
            var id = await result.Content.ReadAsStringAsync();

            Assert.That(id, Is.Not.Empty);
            ids[i] = id;
        }

        Assert.That(ids.Any(x => x is null), Is.False);


        await Parallel.ForAsync(0, n, async (i, token) => {
            var id = ids[i];

            var result = await UpdateProduct(id, new UpdateProductDto(status: AvailabilityStatus.Published));
            Assert.That(result.IsSuccessStatusCode, Is.True);

            result = await GetProduct(id);
            Assert.That(result.IsSuccessStatusCode, Is.True);

            var product = await GetJsonContent<Product>(result);

            testProducts[i] = product;
        });

        Assert.That(testProducts.Any(x => x is null), Is.False);
    }

    protected async Task SearchTestPositive(SearchQueryDto query) {
        var result = await SearchTest(query);

        var actualNames = result.actual.Select(p => p.Title).ToArray();
        var etalonNames = result.etalon.Select(p => p.Title).ToArray();

        Assert.That(actualNames, Is.EqualTo(etalonNames).AsCollection);
    }

    protected async Task SearchTestNegative(SearchQueryDto query) {
        var result = await SearchProducts(query);
        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    private record SearchTestResult(Product[] actual, Product[] etalon);

    private async Task<SearchTestResult> SearchTest(SearchQueryDto query) {
        var searchDbTask = SearchProducts(query);

        var etalon = SearchEtalon(testProducts, query).ToArray();

        var result = await searchDbTask;
        Assert.That(result.IsSuccessStatusCode, Is.True);

        var dbList = await GetJsonContent<string[]>(result);

        var actualProducts = dbList
            .Select(id => testProducts.First(p => p.Id.ToString() == id))
            .ToArray();

        return new SearchTestResult(actualProducts, etalon);

    }

    public static IEnumerable<Product> SearchEtalon(IEnumerable<Product> products, SearchQueryDto query) {
        var contains = query.Contains?.Trim().ToLower();
        var priceFrom = query.PriceFrom;
        var priceUpTo = query.PriceUpTo;

        products = products.Where(p => p.Availability == AvailabilityStatus.Published);

        if (priceFrom is not null) {
            products = products.Where(product
                => product.Price >= priceFrom.Value);
        }

        if (priceUpTo is not null) {
            products = products.Where(product
                => product.Price <= priceUpTo.Value);
        }

        if (contains is not null) {
            products = products.Where(product
                => product.Title.ToLower().Contains(contains));
        }

        switch (query.SortingOrder) {
            case SortingOrder.AtoZ:
                products = products.OrderBy(product => product.Title.ToLower());
                break;
            case SortingOrder.ByDate:
                products = products.OrderBy(product => product.CreationTimestamp);
                break;
            case SortingOrder.ByPrice:
                products = products.OrderBy(product => product.Price);
                break;
        }

        if (query.SortingType == SortingType.Descending) {
            products = products.Reverse();
        }

        var skip = query.From;
        var take = query.To - query.From + 1;

        products = products.Skip(skip).Take(take);

        return products.ToArray();
    }
}
