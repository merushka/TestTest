using Microsoft.AspNetCore.Mvc;
using WebApplication.DTOs;
using WebApplication.Data;
using WebApplication.Models;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication.Controllers
{
    [Route("statistics/sales")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public StatisticsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("products/{id}")]
        public IActionResult GetProductStatistics(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("Продукт не найден.");
            }

            var orders = _context.OrderItems
                .Where(oi => oi.ProductId == id)
                .Select(oi => new
                {
                    OrderId = oi.OrderId,
                    Count = oi.Quantity,
                    Summ = oi.Quantity * oi.Price,
                    UserName = _context.Orders
                        .Where(o => o.Id == oi.OrderId)
                        .Select(o => o.Customer == null ? "[No name]" : o.Customer.Name)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(new
            {
                LeftCount = product.Quantity,
                Orders = orders
            });
        }
    }
}
