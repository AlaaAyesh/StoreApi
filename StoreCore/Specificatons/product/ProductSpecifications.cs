using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StoreCore.Specificatons.product
{
    public class ProductSpecifications : BaseSpecifications<Entities.Product, int>
    {

        public ProductSpecifications(int id) : base (p => p.Id!.Equals(id))
        {
            ApplyInclude();
        }

        public ProductSpecifications( ProductSpecParams productSpecParams) : 
            base(
            p => 
            (string.IsNullOrEmpty(productSpecParams.Search) || p.Name.ToLower().Contains(productSpecParams.Search) )
            &&
            (!productSpecParams.BrandId.HasValue || p.BrandId == productSpecParams.BrandId)
            && 
            (!productSpecParams.TypeId.HasValue || p.TypeId == productSpecParams.TypeId)
            )
        {
            // Default constructor for cases where no criteria is specified
            // This will allow fetching all products with their brands and types
            ApplyInclude();
            if (productSpecParams.Sort != null)
            {
                switch (productSpecParams.Sort) {
                    
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;

                }

            }
            else {
                OrderBy = p => p.Name;

            }


            ApplyPaging((productSpecParams.PageIndex - 1) * productSpecParams.PageSize, productSpecParams.PageSize);

        }


        private void ApplyInclude()
        {
            Includes.Add(p => p.Brand!);
            Includes.Add(p => p.Type!);
        }
    }
}
