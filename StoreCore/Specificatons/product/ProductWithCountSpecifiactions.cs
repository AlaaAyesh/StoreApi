

namespace StoreCore.Specificatons.product
{
    public class ProductWithCountSpecifiactions:BaseSpecifications<Entities.Product, int>
    {
        public ProductWithCountSpecifiactions(ProductSpecParams productSpecParams) : base(
    p =>
    (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search))&&
    (!productSpecParams.BrandId.HasValue || p.BrandId == productSpecParams.BrandId)
    &&
    (!productSpecParams.TypeId.HasValue || p.TypeId == productSpecParams.TypeId)
    )
        {

            

        }

    }
}
