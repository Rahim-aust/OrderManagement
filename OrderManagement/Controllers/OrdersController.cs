using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;

namespace OrderManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly ProductManageDbContext context;

    public OrdersController(ProductManageDbContext context)
    {
        this.context = context;
    }
    //API 01
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder(int productId, string customerName, decimal quantity)
    {
        if (quantity <= 0) return BadRequest("Quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(customerName)) return BadRequest("Customer name cannot be empty.");

        var product = await context.TblProducts.FindAsync(productId);
        if (product == null) return NotFound("Product not found.");
        if (product.NumStock < quantity) return BadRequest("Insufficient stock.");

        var order = new TblOrder
        {
            IntProductId = productId,
            StrCustomerName = customerName,
            NumQuantity = quantity,
            DtOrderDate = DateTime.Now
        };

        product.NumStock -= quantity;
        context.TblOrders.Add(order);
        await context.SaveChangesAsync();

        return Ok("Order created successfully.");
    }

    //API 02
    [HttpPut("update-order/{orderId}")]
    public async Task<IActionResult> UpdateOrderQuantity(int orderId, decimal newQuantity)
    {
        var order = await context.TblOrders.Include(o => o.IntProduct).FirstOrDefaultAsync(o => o.IntOrderId == orderId);
        if (order == null) return NotFound("Order not found.");
        if (newQuantity <= 0) return BadRequest("Quantity must be greater than zero.");

        var stockChange = newQuantity - order.NumQuantity;
        if (order.IntProduct.NumStock < stockChange)
            return BadRequest("Insufficient stock to update the order.");

        order.IntProduct.NumStock -= stockChange;
        order.NumQuantity = newQuantity;

        await context.SaveChangesAsync();
        return Ok("Order updated successfully.");
    }

    //API 03
    [HttpDelete("delete-order/{orderId}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var order = await context.TblOrders.Include(o => o.IntProduct).FirstOrDefaultAsync(o => o.IntOrderId == orderId);
        if (order == null) return NotFound("Order not found.");

        order.IntProduct.NumStock += order.NumQuantity;
        context.TblOrders.Remove(order);

        await context.SaveChangesAsync();
        return Ok("Order deleted successfully.");
    }

    //API 04
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrdersWithDetails()
    {
        var orders = await context.TblOrders
            .Include(o => o.IntProduct)
            .Select(o => new
            {
                o.IntOrderId,
                o.StrCustomerName,
                o.NumQuantity,
                o.DtOrderDate,
                ProductName = o.IntProduct.StrProductName,
                UnitPrice = o.IntProduct.NumUnitPrice
            }).ToListAsync();

        return Ok(orders);
    }

    //API 05
    [HttpGet("product-summary")]
    public async Task<IActionResult> GetProductSummary()
    {
        var products = await context.TblProducts.ToListAsync();
        var orders = await context.TblOrders.ToListAsync();

        var summary = new List<object>();

        foreach (var product in products)
        {
            var productOrders = orders.Where(order => order.IntProductId == product.IntProductId);

            decimal totalQuantity = productOrders.Sum(order => order.NumQuantity);
            decimal totalRevenue = productOrders.Sum(order => order.NumQuantity * product.NumUnitPrice);

            summary.Add(new
            {
                ProductName = product.StrProductName,
                TotalQuantity = totalQuantity,
                TotalRevenue = totalRevenue
            });
        }

        return Ok(summary);
    }



    //API 06
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts(decimal threshold = 100)
    {
        var products = await context.TblProducts
            .Where(p => p.NumStock < threshold)
            .Select(p => new { p.StrProductName, p.NumUnitPrice, p.NumStock })
            .ToListAsync();

        return Ok(products);
    }

    //API 07
    [HttpGet("top-customers")]
    public async Task<IActionResult> GetTopCustomers()
    {
        var customers = await context.TblOrders
            .GroupBy(o => o.StrCustomerName.ToUpper())
            .Select(g => new
            {
                CustomerName = g.Key,
                TotalQuantity = g.Sum(o => o.NumQuantity)
            })
            .OrderByDescending(c => c.TotalQuantity)
            .Take(3)
            .ToListAsync();

        return Ok(customers);
    }

    //API 08
    [HttpGet("unordered-products")]
    public async Task<IActionResult> GetUnorderedProducts()
    {
        var products = await context.TblProducts
            .Where(p => !context.TblOrders.Any(o => o.IntProductId == p.IntProductId))
            .ToListAsync();

        return Ok(products);
    }
}

