namespace Domain.DTO.Operations.Team.input;

public class EncodePayinsInputDto
{
    public int distributor_id { get; set; }
    public int upline_id { get; set; }
    public string binary_data { get; set; }
    public string key_code { get; set; }
    public int imaginary_upline_id { get; set; }
    public int encoded_by { get; set; }
}