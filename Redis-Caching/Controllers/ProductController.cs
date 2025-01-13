using Microsoft.AspNetCore.Mvc;
using Redis_Caching.Models;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly RedisCacheService _cacheService;
    private const string ProductCacheKey = "ProductList";

    private static List<Product> products = new List<Product>
    {
        new Product(1, "Product 1", "Description 1", 100.0m, 10),
        new Product(2, "Product 2", "Description 2", 200.0m, 20),
        new Product(2, "Product 2", "Description 2", 200.0m, 20)
    };

    public ProductsController(RedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cachedProducts = await _cacheService.GetCacheValueAsync<List<Product>>(ProductCacheKey);
        if (cachedProducts == null)
        {
            cachedProducts = products;
            await _cacheService.SetCacheValueAsync(ProductCacheKey, cachedProducts, TimeSpan.FromMinutes(10));
        }

        return Ok(cachedProducts);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product newProduct)
    {
        if (newProduct == null)
        {
            return BadRequest("Invalid product data.");
        }

        products.Add(newProduct);
        var cachedProducts = await _cacheService.GetCacheValueAsync<List<Product>>(ProductCacheKey) ?? products;
        cachedProducts.Add(newProduct);

        await _cacheService.SetCacheValueAsync(ProductCacheKey, cachedProducts, TimeSpan.FromMinutes(10));

        return CreatedAtAction(nameof(GetAll), newProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        if (updatedProduct == null || updatedProduct.Id != id)
        {
            return BadRequest("Invalid product data.");
        }

        var cachedProducts = await _cacheService.GetCacheValueAsync<List<Product>>(ProductCacheKey) ?? products;
        var existingProduct = cachedProducts.FirstOrDefault(p => p.Id == id);

        if (existingProduct == null)
        {
            return NotFound("Product not found.");
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Stock = updatedProduct.Stock;

        var productInList = products.FirstOrDefault(p => p.Id == id);
        if (productInList != null)
        {
            productInList.Name = updatedProduct.Name;
            productInList.Description = updatedProduct.Description;
            productInList.Price = updatedProduct.Price;
            productInList.Stock = updatedProduct.Stock;
        }
        await _cacheService.SetCacheValueAsync(ProductCacheKey, cachedProducts, TimeSpan.FromMinutes(10));

        return Ok(existingProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var cachedProducts = await _cacheService.GetCacheValueAsync<List<Product>>(ProductCacheKey) ?? products;
        var product = cachedProducts.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        cachedProducts.Remove(product);


        await _cacheService.SetCacheValueAsync(ProductCacheKey, cachedProducts, TimeSpan.FromMinutes(10));

        return NoContent();
    }
    [HttpDelete("clear-cache")]
    public async Task<IActionResult> ClearCache()
    {

        await _cacheService.RemoveCacheValueAsync(ProductCacheKey);

        return NoContent();
    }
}
