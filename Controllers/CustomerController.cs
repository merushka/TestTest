using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebApplication.Data;      // DatabaseContext
using WebApplication.Models;  
using LinqToDB;                

namespace WebApplication.Controllers
{
    
    [ApiController]
    [Route("customers")]
    public class CustomersController : ControllerBase
    {
        private readonly DatabaseContext _db;

        public CustomersController(DatabaseContext db)
        {
            _db = db;
        }

        
        //DTO для создания нового Customer.
        
        public class CustomerCreateDto
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

      
        // POST customers Создаёт нового клиента, возвращает 201 с данными (включая Id).
       
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CustomerCreateDto request)
        {
           
            var entity = new Customer
            {
                Name = request.Name,
                Email = request.Email
                // Id НЕ задаём, он как Identity в Oracle
            };

            var newId = _db.InsertWithIdentity(entity);
            entity.Id = Convert.ToInt32(newId);

            //  HTTP 201 (Created) + сам объект
            // CreatedAtAction, чтобы заголовок GET /customers/{id}
            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = entity.Id },
                entity
            );
        }

       
        // GET /customers/{id} Получает данные о клиенте по Id (который Oracle присвоил).
       
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                return NotFound(); // 404

            return Ok(customer); // 200 + JSON
        }
    }
}
