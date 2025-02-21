using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication.Data;
using WebApplication.DTOs;

namespace WebApplication.Controllers
{
    [Route("summary")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly DatabaseContext _db;

        public SummaryController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet("sales/products/{id}")]
        public IActionResult GetProductSalesSummary(int id)
        {
            var orders = _db.Orders.Where(o => _db.OrderItems.Any(oi => oi.ProductId == id)).ToList();

            var summary = new
            {
                LeftCount = _db.Products.Where(p => p.Id == id).Select(p => p.Quantity).FirstOrDefault(),
                Orders = orders.Select(o => new
                {
                    OrderId = o.Id,
                    Count = _db.OrderItems.Where(oi => oi.OrderId == o.Id && oi.ProductId == id).Sum(oi => oi.Quantity), // Исправлено: Count()
                    Summ = _db.OrderItems.Where(oi => oi.OrderId == o.Id && oi.ProductId == id).Sum(oi => oi.Price),
                    UserName = _db.Customers.Where(c => c.Id == o.CustomerId).Select(c => c.Name).FirstOrDefault()
                }).ToList()
            };

            return Ok(summary);
        }

        [HttpPost("sales/products")]
        public IActionResult GetSalesByProductIds([FromBody] ProductSalesRequest request)
        {
            var orders = _db.Orders.Where(o => request.ProductIds.Contains(o.Id) && o.OrderDate >= request.DateStart && o.OrderDate <= request.DateEnd).ToList();

            var summary = new
            {
                Orders = orders.Select(o => new
                {
                    OrderId = o.Id,
                    Count = _db.OrderItems.Where(oi => oi.OrderId == o.Id).Sum(oi => oi.Quantity), // Исправлено: Count()
                    Summ = _db.OrderItems.Where(oi => oi.OrderId == o.Id).Sum(oi => oi.Price),
                    OrderDate = o.OrderDate,
                    UserName = _db.Customers.Where(c => c.Id == o.CustomerId).Select(c => c.Name).FirstOrDefault(),
                    Products = _db.OrderItems.Where(oi => oi.OrderId == o.Id)
                        .Select(oi => new
                        {
                            ProductId = oi.ProductId,
                            Count = oi.Quantity,
                            Price = oi.Price
                        }).ToList()
                }).ToList()
            };

            return Ok(summary);
        }
    }
}
