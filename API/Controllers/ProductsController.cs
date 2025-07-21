using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context)
    {
        // Initialize any required services or dependencies here
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        // Simulate fetching products from a database
        return await context.Products.ToListAsync();
    }

    [HttpGet("{id:int}")] // api//products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // Fetch a single product by its ID
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // Add a new product to the database
        context.Products.Add(product);

        await context.SaveChangesAsync();

        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        // Check if the product exists
        if (id != product.Id || !ProductExists(id))
            return BadRequest("Cannot update this product");

        // Update the product in the database
        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        // Find the product to delete
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        // Remove the product from the database
        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        // Check if a product with the given ID exists
        return context.Products.Any(e => e.Id == id);
    }
}
