using Domain.DTO.BaseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Products.Output
{
    public class ProductEditOutputDto
    {
        public int ID { get; set; }
        public string p_code { get; set; }
        public string p_name { get; set; }
        
        
        public decimal price { get; set; }
        public Nullable<decimal> membership_price { get; set; }
        public Nullable<decimal> non_membership_discounted_price { get; set; } 
        public decimal profit { get; set; }
        public decimal total_payout { get; set; }
        
        
        public string category { get; set; }
        public string? cover_photo { get; set; }
        public string? description { get; set; }
        public string mini_desc { get; set; }
        public DateTime created_dt { get; set; }
        public DateTime updated_dt { get; set; }
        public DateTime deleted_dt { get; set; }
        public string status { get; set; }
        public string? remarks { get; set; }
        public bool package { get; set; }

        public int created_by { get; set; }

        public int? updated_by { get; set; }
        public bool is_package { get; set; }

        public  List<ProductsImageDto> product_img { get; set; }

        public List<PackageProductBaseDto> package_products { get; set; } = new List<PackageProductBaseDto>();
        public List<ProductBaseDto> products { get; set; } = new List<ProductBaseDto>();
    }
}
