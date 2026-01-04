using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreCore.Dtos.Products;
using StoreCore.Helper;
using StoreCore.Specificatons.product;

namespace StoreCore.ServicesContract
{
    public interface IProductService
    {
        Task<PaginationResponse<ProductDto>> GetAllProductsAsync( ProductSpecParams productSpecParams);
        Task<IEnumerable<TypeBrandDto>> GetAllTypesAsync();
        Task<IEnumerable<TypeBrandDto>> GetAllBrandsAsync();

 
        Task<ProductDto> GetProductByIdAsync(int id);




    }
}
