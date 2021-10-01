using Example.API.DataAccess.Repository.Abstract;
using Example.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Example.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductsController(IRepository<Product> repository) => _repository = repository;

        // GET: api/Products
        /// <summary>
        /// Brings all registered products in the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _repository.GetAll();

            return products != null ? Ok(products) : NoContent();
        }

        // GET: api/Products/5
        /// <summary>
        /// Retrieves specific product data from the warehouse based on the value <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (ProductExists(id) is false)
                return NotFound();

            var product = await _repository.GetById(id);

            return Ok(product);
        }

        // PUT: api/Products/5
        /// <summary>
        ///  Updates the <paramref name="product"/> data in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (ProductExists(id) is false)
                return NotFound();

            if (id != product.Id)
                return BadRequest();

            _repository.Update(product);

            return NoContent();
        }

        // POST: api/Products
        /// <summary>
        /// Adds <b><paramref name="product"/></b> data to database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            await _repository.Create(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        /// <summary>
        /// Deletes the <paramref name="id"/> datas from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if(ProductExists(id) is false)
                return NotFound();

            var product = await _repository.GetById(id);
            if (product is null)
                return NotFound();

            _repository.Delete(product);

            return NoContent();
        }

        /// <summary>
        /// It checks if the product with <paramref name="id"/> value exists on the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ProductExists(int id) => _repository.GetById(id)?.Result != null;
    }
}
