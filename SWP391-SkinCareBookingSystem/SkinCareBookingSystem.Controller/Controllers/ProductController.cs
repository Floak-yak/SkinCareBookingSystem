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
        private readonly ITransactionService _transactionService;

        public ProductController(IProductService productService, IMapper mapper, ITransactionService transactionService)
        {
            _mapper = mapper;
            _productService = productService;
            _transactionService = transactionService;
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

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            if (!await _productService.UpdateProduct(request))
            {
                return BadRequest("Update fail");
            }
            return Ok("Update success");
        }

        [HttpPost("CheckoutCart")]
        public async Task<IActionResult> CheckoutCart([FromBody] CheckoutCartRequest request)
        {
            if (request is null) return BadRequest("Request is null");
            if (request.checkoutProductInformation is null) return BadRequest("Request is null");
            foreach (var item in request.checkoutProductInformation)
            {
                if (item.Amount <= 0 || item.Id <= 0)
                    return BadRequest("Invalid product");
            }
            return Ok(await _transactionService.CreateTransaction(request));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromQuery] int productId)
        {
            return Ok(_mapper.Map<GetProductResponse>(await _productService.GetProductById(productId)));
        }
    }
}
