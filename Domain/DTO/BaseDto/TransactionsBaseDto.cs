namespace Domain.DTO.BaseDto;

public class TransactionsBaseDto
{
    public int ident { get; set; }
    public string tran_number { get; set; }
    public string tran_type { get; set; }
    public bool void_status { get; set; }
    public DateTime created_dt { get; set; }
    public DateTime updated_dt { get; set; }
     
    public int created_by { get; set; } 
 
    public int? updated_by { get; set; }

    public virtual string created_by_name {
        get;
        set;
    }
    
    public bool is_code_generated { get; set; }
}