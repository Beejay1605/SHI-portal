using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Distributors.Input;

public  class RegistrationInputDto 
{
    
    public string firstname { get; set; } = string.Empty;
    public string middlename { get; set; } = "";
    public string lastname { get; set; } = string.Empty;
    public string suffix { get; set; } = string.Empty;
    public string age { get; set; } = string.Empty;
    public string sex { get; set; } = string.Empty;
    public string completeAddress { get; set; } = string.Empty;
    public string contact { get; set; } 
    public DateTime dateOfBirth { get; set; }  
    public string email { get; set; } = string.Empty;
    public string msessenger { get; set; } = string.Empty;
    public string tin { get; set; } = string.Empty;
    public int noOfAccount { get; set; }
    public int directupLineCode { get; set; } 
    public string placementCode { get; set; } = string.Empty;
    public DateTime purchaseDate { get; set; } 

    public IFormFile user_picture { get; set; }
    public string status { get; set; } = string.Empty;
    public string remarks { get; set; }
     
    public int det_id { get; set; }
}
