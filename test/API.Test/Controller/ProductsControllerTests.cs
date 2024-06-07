using Moq;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Data;
using API.Controllers;
using Microsoft.EntityFrameworkCore;

namespace API.Test.Controller;

public class ProductsControllerTests
{
    private readonly AppDbContext _context;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new AppDbContext(options);
        _controller = new ProductsController(_context);
    }

    [Fact]
    public async Task GetProducts_Returns_All_Products()
    {
        // Arrange
        _context.Products.AddRange(new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 10 },
                new Product { Id = Guid.NewGuid(), Name = "Product2", Price = 20 },
            });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var returnValue = Assert.IsType<List<Product>>(actionResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProduct_Returns_Product_By_Id()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 10 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetProduct(product.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var returnValue = Assert.IsType<Product>(actionResult.Value);
        Assert.Equal(product.Id, returnValue.Id);
    }

    [Fact]
    public async Task PostProduct_Creates_Product()
    {
        // Arrange
        var product = new Product { Name = "Product1", Price = 10 };

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnValue = Assert.IsType<Product>(createdAtActionResult.Value);
        Assert.Equal(product.Name, returnValue.Name);
    }

    [Fact]
    public async Task PutProduct_Updates_Product()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 10 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.Name = "UpdatedProduct";

        // Act
        var result = await _controller.PutProduct(product.Id, product);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_Deletes_Product()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 10 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteProduct(product.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedProduct = await _context.Products.FindAsync(product.Id);
        Assert.Null(deletedProduct);
    }
}
