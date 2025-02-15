using CommonLayer.Models;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using System.Linq;

namespace FundooNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FundooDBContext context;

        public ProductController(FundooDBContext context)
        {
            this.context = context;
        }

        [HttpPost("add")]
        public IActionResult AddProduct([FromBody] ProductEntity product)
        {
            if (product == null)
            {
                return BadRequest(new { Success = false, Message = "Failed to add Product" });
            }

            context.Products.Add(product);
            context.SaveChanges();
            return Ok(new { Success = true, Message = "Product Added Successfully", Data = product });
        }

        [HttpPut("update")]
        public IActionResult UpdateProduct(int id, ProductEntity updatedProduct)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return BadRequest(new { Success = false, Message = "Product not found" });
            }

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            context.SaveChanges();
            return Ok(new { Success = true, Message = "Product Updated Successfully", Data = product });
        }

        [HttpDelete("delete")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return BadRequest(new { Success = false, Message = "Product not found" });
            }

            context.Products.Remove(product);
            context.SaveChanges();
            return Ok(new { Success = true, Message = "Product Deleted Successfully" });
        }
    }
}
