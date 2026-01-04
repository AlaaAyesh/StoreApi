
using AutoMapper;
using StoreCore;
using StoreCore.Dtos.Products;
using StoreCore.Entities;
using StoreCore.Helper;
using StoreCore.ServicesContract;
using StoreCore.Specificatons.product;

namespace StoreService.Service.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TypeBrandDto>> GetAllBrandsAsync()
        {
            return _mapper.Map<IEnumerable<TypeBrandDto>>(await UnitOfWork.Repository<ProductBrand, int>().GetAllAsync());

        }

        public async Task<PaginationResponse<ProductDto>> GetAllProductsAsync(ProductSpecParams productSpecParams)
        {
            var spec = new ProductSpecifications(productSpecParams);
            var products = await UnitOfWork.Repository<Product, int>().GetAllWithSpecificationAsync(spec);
            var mappedProducts = _mapper.Map<IEnumerable<ProductDto>>(products);
            var countSpec = new ProductWithCountSpecifiactions(productSpecParams);
            var cont = await UnitOfWork.Repository<Product, int>().GetCountAsync(countSpec);

            return new PaginationResponse<ProductDto>(
                productSpecParams.PageSize,
                productSpecParams.PageIndex,
                cont,
                mappedProducts
            );
        }


        public async Task<IEnumerable<TypeBrandDto>> GetAllTypesAsync()
        {
            return _mapper.Map<IEnumerable<TypeBrandDto>>(await UnitOfWork.Repository<ProductType, int>().GetAllAsync());
        }

        public Task<ProductDto> GetProductByIdAsync(int id)
        {
            var spec = new ProductSpecifications(id);

            var product = UnitOfWork.Repository<Product, int>().GetByIdWithSpecificationAsync(spec);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }
            return Task.FromResult(_mapper.Map<ProductDto>(product.Result));

        }
    }
}
