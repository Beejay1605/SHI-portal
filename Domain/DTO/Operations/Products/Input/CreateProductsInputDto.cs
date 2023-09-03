using Domain.DTO.BaseDto;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Products.Input;

public class CreateProductsInputDto
{
    public Guid ID { get; set; }    
    public string name { get; set; }


    public Nullable<decimal> price { get; set; }
    public Nullable<decimal> membership_price { get; set; }
    public Nullable<decimal> non_membership_discounted_price { get; set; } 
    public Nullable<decimal> profit { get; set; }
    public Nullable<decimal> total_payout { get; set; }


    public string p_category { get; set; }
    public IFormFile cover_photo { get; set; }
    public string description { get; set; }
    public List<IFormFile> pictures { get; set; }
    public string mini_desc { get; set; }
    public bool is_package { get; set; }
    public List<PackageProductBaseDto> package_product { get; set; } = new List<PackageProductBaseDto>();
}
