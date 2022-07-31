using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class ProductService : BaseService, IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    public ProductService(IHttpClientFactory clientFactory) : base(clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<T> CreateProductAsync<T>(ProductDto producDto)
    {
        return await this.SendAsync<T>( new ApiRequest()
        {
            ApiType = Constants.ApiType.POST,
            Data = producDto,
            Url = Constants.ProductAPIBase + "/api/products/",
            AccessToken = String.Empty
        });
    }

    public async Task<T> DeleteProductAsync<T>(int id)
    {
        return await this.SendAsync<T>( new ApiRequest()
        {
            ApiType = Constants.ApiType.DELETE,
            Url = Constants.ProductAPIBase + "/api/products/" + id.ToString(),
            AccessToken = String.Empty
        });
    }

    public async Task<T> GetAllProductsAsync<T>()
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = Constants.ApiType.GET,
            Url = Constants.ProductAPIBase + "/api/products/",
            AccessToken = String.Empty
        });
    }

    public async Task<T> GetProductByIdAsync<T>(int id)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = Constants.ApiType.GET,
            Url = Constants.ProductAPIBase + "/api/products/" + id.ToString(),
            AccessToken = String.Empty
        });
    }

    public async Task<T> UpdateProductAsync<T>(ProductDto producDto)
    {
        return await this.SendAsync<T>(new ApiRequest()
        {
            ApiType = Constants.ApiType.PUT,
            Data = producDto,
            Url = Constants.ProductAPIBase + "/api/products/",
            AccessToken = String.Empty
        });
    }
}
