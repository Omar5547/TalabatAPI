using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specification;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : APIBaseController
    {
       
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
       
       

        public ProductsController(IMapper mapper , IUnitOfWork unitOfWork)
        {
            
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            
           
        }
        [CachedAttribute(300)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
        {
            var spec = new ProductWithBrandAndTypeSpec(Params);
            var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
           var CountSpec = new ProductWithFiltrationsCountAsync(Params);
            var Count = await  _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(Params.PageIndex,Params.PageSize,MappedProducts,Count));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var spec = new ProductWithBrandAndTypeSpec(id);
            var Product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            if (Product is null) return NotFound(new ApiResponse(404));
            var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(Product);
            return Ok(MappedProduct);
        }
        ///  Get All Types
        [HttpGet("types")]
        //BaseUrl/api/Products/Types
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var Types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }
        ///  Get All Brands
        //  BaseUrl/api/Products/Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
        }
    }
}
