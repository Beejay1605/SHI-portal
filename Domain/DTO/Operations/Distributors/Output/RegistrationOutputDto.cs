using Domain.DTO.BaseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Distributors.Output;

public class RegistrationOutputDto
{
    public List<DistributorsDetailsDto> distributor { get; set; } = new List<DistributorsDetailsDto>();




    public Guid ident { get; set; }
    public int ui_id { get; set; }
    public string first_name { get; set; }

    public string last_name { get; set; }

    public string middle_name { get; set; }

    public string Email { get; set; }
    public string Username { get; set; }
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
    public virtual List<string> status_list { get; set; } = new List<string>();
    public virtual string remarks { get; set; }
}



 