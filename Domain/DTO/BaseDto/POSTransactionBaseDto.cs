namespace Domain.DTO.BaseDto;

public class POSTransactionBaseDto
{
    public int ident { get; set; }
     
    public int transaction_ident { get; set; }
     
    public int product_ident { get; set; }
    
    public decimal srp_price { get; set; }
    public decimal? distributors_price { get; set; }
    public decimal? non_dist_price { get; set; }
     
    public decimal company_profit { get; set; }
     
    public decimal total_payout { get; set; }
     
    public decimal unit_price { get; set; }
     
    public int quantity { get; set; }
     
    public string payment { get; set; }
     
    public string branch { get; set; }
    

    public int? distributor_ident { get; set; }
     
    public decimal vat_percent { get; set; }
    
    public virtual ProductBaseDto product_details { get; set; }
}