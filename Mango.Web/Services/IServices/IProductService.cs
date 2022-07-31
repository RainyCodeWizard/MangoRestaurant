using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface IProductService : IBaseService
{
    Task<T> GetAllProductsAsync<T>();
    Task<T> GetProductByIdAsync<T>(int id);
    Task<T> CreateProductAsync<T>(ProductDto producDto);
    Task<T> UpdateProductAsync<T>(ProductDto producDto);
    Task<T> DeleteProductAsync<T>(int id);
}
