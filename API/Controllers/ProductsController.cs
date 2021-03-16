using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _ProductRepo;
        private readonly IGenericRepository<ProductBrand> _ProductBrandRepo;
        private readonly IGenericRepository<ProductType> _ProductTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> ProductRepo,
            IGenericRepository<ProductBrand> ProductBrandRepo,
            IGenericRepository<ProductType> ProductTypeRepo,
            IMapper mapper)
        {
            _ProductTypeRepo = ProductTypeRepo;
            _ProductBrandRepo = ProductBrandRepo;
            _ProductRepo = ProductRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();
            var products = await _ProductRepo.ListAsync(spec);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _ProductRepo.GetEntityWithSpec(spec);
            if (product == null) return NotFound(new ApiResponse(404));
            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _ProductBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _ProductTypeRepo.ListAllAsync());
        }
    }
}