using Azure;
using Azure.Search.Documents;

namespace OrderBooking.WebAPI
{
    public static class Search
    {
        public static IServiceCollection AddSearch(this IServiceCollection services, string endpoint, string apiKey)
        {
            var searchClient = new SearchClient(new Uri(endpoint), "salesorders", new AzureKeyCredential(apiKey));

            services.AddSingleton(searchClient);

            return services;
        }

    }
}
