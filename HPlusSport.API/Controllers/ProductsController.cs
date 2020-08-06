using HPlusSport.API.Models;
using HPlusSport.API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HPlusSport.API.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/v{v:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        public ProductsController(ShopContext context)
        {
            _shopContext = context;
            _shopContext.Database.EnsureCreated();
        }
        /*[HttpGet]
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _shopContext.Products.ToArrayAsync();
        }*/
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery]ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products;
            if (queryParameters.MinPrice != null && queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value 
                                        && p.Price <= queryParameters.MaxPrice.Value);
            }
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku.Equals(queryParameters.Sku));
            }
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom<Product>(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }
            products = products.Skip(queryParameters.Size * (queryParameters.Page)).Take(queryParameters.Size);
            return Ok(await products.ToArrayAsync());
        }
        [HttpGet("{id}")]
        //[HttpGet]
        //[Route("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody]Product product)
        {
            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute]int id, [FromBody] Product product)
        {

            Product existingProduct = _shopContext.Products.Where(p => p.Id == id).FirstOrDefault<Product>();
            if (existingProduct != null)
            {
                existingProduct.IsAvailable = product.IsAvailable;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Sku = product.Sku;
                existingProduct.CategoryId = product.CategoryId;
                await _shopContext.SaveChangesAsync();
            }

            else
            {
                return NotFound();
            }

            return CreatedAtAction(
                "GetProduct",
                new { id },
                product
            );
            /*if (id != product.Id) return BadRequest();
            _shopContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_shopContext.Products.Find(id) == null)
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();*/
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            _shopContext.Remove(product);
            await _shopContext.SaveChangesAsync();
            return product;
        }
    }
}
