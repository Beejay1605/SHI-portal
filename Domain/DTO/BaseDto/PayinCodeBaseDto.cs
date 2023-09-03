namespace Domain.DTO.BaseDto;

public class PayinCodeBaseDto
{
    public int ident { get; set; }
    public string code { get; set; }
    public int distributor_ident { get; set; }
    public int tran_ident { get; set; }
    public DateTime date_created { get; set; }
    public DateTime date_updated { get; set; }
    public DateTime expiration_date { get; set; }
    public int created_by { get; set; }
    public int? updated_by { get; set; }
    
    public bool is_used { get; set; }
    
    public virtual DistributorsDetailsDto distributorsDetails { get; set; }
}