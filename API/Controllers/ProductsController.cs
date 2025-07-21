using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        // Simulate fetching products from a database
        return Ok(await repo.GetProductsAsync());
    }

    [HttpGet("{id:int}")] // api//products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // Fetch a single product by its ID
        var product = await repo.GetProductByIdAsync(id);

        if (product == null)
            return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // Add a new product to the database
        repo.AddProduct(product);

        if (await repo.SaveChangesAsync())
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);

        return BadRequest("Failed to create product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        // Check if the product exists
        if (id != product.Id || !ProductExists(id))
            return BadRequest("Cannot update this product");

        // Update the product in the database
        repo.UpdateProduct(product);

        if (await repo.SaveChangesAsync())
            return NoContent(); // Return 204 No Content if the update was successful

        return BadRequest("Failed to update product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        // Find the product to delete
        var product = await repo.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        // Remove the product from the database
        repo.DeleteProduct(product);
        if (await repo.SaveChangesAsync())
            return NoContent(); // Return 204 No Content if the deletion was successful

        return BadRequest("Failed to delete product");
    }

    private bool ProductExists(int id)
    {
        // Check if a product with the given ID exists
        return repo.ProductExists(id);
    }
}
