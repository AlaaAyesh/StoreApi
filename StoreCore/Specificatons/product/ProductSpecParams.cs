using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCore.Specificatons.product
{
    public class ProductSpecParams
    {
        public string ? Sort { get; set; }
        public int ? BrandId { get; set; }
        public int ? TypeId { get; set; }

        public int PageSize { get; set; } = 5;
         public int PageIndex { get; set; } = 1;

        private string? _search;
        public string ? Search {
            get => _search;
            set => _search = value?.ToLower();
        }


    }
}
