using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities;

namespace StoreCore.Dtos.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int? BrandId { get; set; }
        public string Brand { get; set; }

        public int? TypeId { get; set; }
        public string Type { get; set; }

    }
}
