using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.Product;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet("IncludeName")]
        public async Task<IActionResult> SearchProductByName(string name)
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.Search(name));
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("MinMaxPrice")]
        public async Task<IActionResult> SearchProductWithMinMaxPrice(Decimal minPrice, Decimal maxPrice)
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.Search(minPrice, maxPrice));
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("PriceUnder")]
        public async Task<IActionResult> SearchProductByName(Decimal underPrice)
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.Search(underPrice));
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("CategoryId")]
        public async Task<IActionResult> SearchProductByCategoryId(int CategoryId)
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.Search(CategoryId));
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("SearchDesc")]
        public async Task<IActionResult> SearchProductOrederByDesc()
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.SearchDescPrice());
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("SearchAsc")]
        public async Task<IActionResult> SearchProductOrederByAsc()
        {
            List<GetProductResponse> products = _mapper.Map<List<GetProductResponse>>(await _productService.SearchAscPrice());
            if (products is null)
                return NotFound();
            return Ok(products);
        }

		[HttpPost("AddProduct")]
		public async Task<IActionResult> AddProducts([FromBody] List<CreateProductRequest> products)
		{
			if (products is null)
				return NotFound();

			return Ok(await _productService.AddProducts(products));
		}

		[HttpDelete("RemoveProduct")]
		public async Task<IActionResult> RemoveProduct([FromQuery] int productId)
		{
			return Ok(await _productService.RemoveProduct(productId));
		}
	}
}
