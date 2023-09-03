using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Products.Output
{
    public class ProductsOutputDto
    {

        [Key]
        public int ID { get; set; }
        public string p_code { get; set; }
        public string p_name { get; set; }
        public decimal price { get; set; }
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

        public string? product_img { get; set; }


    }
}
