using System.Net;
using InnoShop.Domain.Enums;

namespace InnoShop.Tests.TestProductAPI.Tests.Search;

class SearchTests : SearchTestsBase {

    [Test]
    public async Task SearchEmptyQuery() {
        var query = EmptyQuery;
        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchBigQuery() {
        var query = EmptyQuery;
        query.To = 2000;
        await SearchTestNegative(query);
    }

    [Test]
    public async Task SearchByContainsMogus() {
        var query = EmptyQuery;
        query.Contains = "Mogus";
        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchByContainsMogusUppercase() {
        var query = EmptyQuery;
        query.Contains = "MOGUS";
        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchByContainsA() {
        var query = EmptyQuery;
        query.Contains = "A";
        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchByContainsSus() {
        var query = EmptyQuery;
        query.Contains = "SuS";
        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchSortAtoZ() {
        var query = EmptyQuery;
        query.SortingOrder = SortingOrder.AtoZ;
        query.SortingType = SortingType.Ascending;

        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchSortZtoA() {
        var query = EmptyQuery;
        query.SortingOrder = SortingOrder.AtoZ;
        query.SortingType = SortingType.Descending;

        await SearchTestPositive(query);
    }

    [Test]
    [Ignore("Not working with sqlite")]
    public async Task SearchSortByPrice() {
        var query = EmptyQuery;
        query.SortingOrder = SortingOrder.ByPrice;
        query.SortingType = SortingType.Ascending;

        await SearchTestPositive(query);
    }

    [Test]
    [Ignore("Not working with sqlite")]
    public async Task SearchSortByPriceReversed() {
        var query = EmptyQuery;
        query.SortingOrder = SortingOrder.ByPrice;
        query.SortingType = SortingType.Descending;

        await SearchTestPositive(query);
    }

    [Test]
    public async Task SearchFree() {
        var query = EmptyQuery;
        query.PriceUpTo = 0;
        await SearchTestPositive(query);
    }
}