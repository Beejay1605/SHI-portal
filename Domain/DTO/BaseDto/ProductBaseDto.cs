using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.BaseDto;

public class ProductBaseDto
{ 
    public int ident { get; set; }
     
    public string p_code { get; set; }
 
    public string p_name { get; set; }
 
    public decimal p_price { get; set; }

    public decimal? membership_price { get; set; }
    public decimal? non_membership_discounted_price { get; set; }
    public decimal profit { get; set; }
    public decimal total_payout { get; set; }


    public string p_category { get; set; }
    public string? p_cover_photo { get; set; }
    public string? p_description { get; set; }
 
    public string p_mini_desc { get; set; }
    public DateTime p_created_dt { get; set; }
    public DateTime p_updated_dt { get; set; } 
 
    public string p_status { get; set; }
    public string? p_remarks { get; set; }
    public bool p_is_package { get; set; }
     
    public string picture { get; set; }
}
