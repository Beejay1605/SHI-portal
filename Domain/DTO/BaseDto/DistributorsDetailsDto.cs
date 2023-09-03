using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.BaseDto;

public class DistributorsDetailsDto
{
    public int ident { get; set; }
    public Guid user_ref_ident { get; set; }
    public int ui_id { get; set; }
    public string first_name { get; set; } 

    public string last_name { get; set; } 

    public string middle_name { get; set; } 

    public string Email { get; set; } 
    public string Username { get;set; } 
    public string suffix_name { get; set; } 

    public string complete_address { get; set; } 
    public DateTime birth_date { get; set; }
    
    public string gender { get; set; }


    public string mobile_number { get; set; } 

    public string fb_messenger_account { get; set; } 

    public string tin { get; set; } 
    public string type_of_account { get; set; } 
    public int accounts_count { get; set; } 
    public int? upline_ref_id { get; set; }
    public string placement_location { get; set; } 
    public string user_picture_base_64 { get; set; } 

    public virtual string status { get; set; } 
    public virtual string account_type { get; set; }
    public DistributorsDetailsDto? upline_details { get; set; }
    public virtual string remarks { get; set; }
 
    public int created_by { get; set; }
 
    public int? updated_by { get; set; }
}
