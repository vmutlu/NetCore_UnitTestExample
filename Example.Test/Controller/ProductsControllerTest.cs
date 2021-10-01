using Example.API.Controllers;
using Example.API.DataAccess.Repository.Abstract;
using Example.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Example.Test.Controller
{
    public class ProductsControllerTest
    {
        #region Properties

        private readonly Mock<IRepository<Product>> _mockRepository;
        private readonly ProductsController _apiController;
        private List<Product> _products;

        #endregion

        #region Constructor Method

        public ProductsControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _apiController = new ProductsController(_mockRepository.Object);

            AddProductToList();
        }

        #endregion

        #region Tests Methods

        [Fact]
        public async Task GetProducts_ActionExecutes_ReturnApiWithProduct()
        {
            var expectedDataCount = 4;

            _mockRepository.Setup(x => x.GetAll()).ReturnsAsync(_products);
            var result = await _apiController.GetProducts();
            var response = Assert.IsType<OkObjectResult>(result);

            var retProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(response.Value);

            Assert.Equal(expectedDataCount, retProducts.Count());
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProduct_ProductNull_ReturnNotFoundResult()
        {
            Product product = null;
            _mockRepository.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var productResult = await _apiController.GetProduct(0);
            var retNotFound = Assert.IsType<NotFoundResult>(productResult);

            Assert.Equal((int)HttpStatusCode.NotFound, retNotFound.StatusCode);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(12)]
        [InlineData(13)]
        public async Task GetProduct_ValidId_ReturnApiWithProduct(int id)
        {
            Product product = _products.FirstOrDefault(x => x.Id == id);
            _mockRepository.Setup(x => x.GetById(id)).ReturnsAsync(product);

            var result = await _apiController.GetProduct(id);
            var retOkResultWithProduct = Assert.IsType<OkObjectResult>(result);
            var retProduct = Assert.IsType<Product>(retOkResultWithProduct.Value);

            Assert.Equal(id, retProduct.Id);
            Assert.Equal(product.Color, retProduct.Color);
            Assert.Equal(product.Price, retProduct.Price);
            Assert.Equal(product.Stock, retProduct.Stock);
            Assert.Equal(product.Name, retProduct.Name);

        }

        [Theory]
        [InlineData(5)]
        public void UpdateProduct_IdNotEqualProductID_ReturnBadRequestResult(int id)
        {
            Product product = _products.FirstOrDefault(x => x.Id == id);

            var result = _apiController.UpdateProduct(0, product);

            Task.Run(() =>
            {
                Assert.IsType<BadRequestResult>(result);
            });
        }

        [Theory]
        [InlineData(100)]
        public void UpdateProduct_ActionExecutes_ReturnNoContentResultAndVerifyUpdateMethod(int id)
        {
            Product product = _products.FirstOrDefault(x => x.Id == id);
            _mockRepository.Setup(x => x.Update(product));

            dynamic result = _apiController.UpdateProduct(id, product);

            Task.Run(() =>
            {
                Assert.Equal((int)HttpStatusCode.NotFound, result?.StatusCode);
            });
        }

        [Fact]
        public async Task AddProduct_ActionExecutes_ReturnCreatedAtActionAndVerifyCreateMethod()
        {
            var expectedActionName = "GetProduct";
            Product product = _products.FirstOrDefault();
            _mockRepository.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _apiController.AddProduct(product);
            var creatActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepository.Verify(x => x.Create(product), Times.Once);
            Assert.Equal(expectedActionName, creatActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteProduct_ProductIsNull_ReturnNotFoundResult()
        {
            const int id = 0;
            Product product = null;

            _mockRepository.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _apiController.DeleteProduct(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(14)]
        public async Task DeleteProduct_ActionExecutes_ReturnNoContentResultAndVerifyDeleteMethod(int id)
        {
            Product product = _products.FirstOrDefault(x => x.Id == id);

            _mockRepository.Setup(x => x.GetById(id)).ReturnsAsync(product);
            _mockRepository.Setup(x => x.Delete(product));

            dynamic result = await _apiController.DeleteProduct(id);
            _mockRepository.Verify(x => x.Delete(product), Times.Once);

            Assert.Equal((int)HttpStatusCode.NoContent, result?.StatusCode);
        }

        #endregion

        private void AddProductToList()
        {
           _products = new List<Product>()
           {
                new Product()
                {
                    Color = "Green", Id = 5, Name = "Example", Price = 85m, Stock = 50
                },
                new Product()
                {
                    Color = "Black", Id = 12, Name = "Example 2", Price = 12m, Stock = 100
                },
                 new Product()
                {
                    Color = "Blue", Id = 13, Name = "Example 3", Price = 15m, Stock = 200
                },
                  new Product()
                {
                    Color = "White", Id = 14, Name = "Example 4", Price = 72m, Stock = 300
                }
            };
        }
    }
}
