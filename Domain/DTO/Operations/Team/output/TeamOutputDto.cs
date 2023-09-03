using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.Team.output;

public class TeamOutputDto 
{
    public BinaryTreeBaseDto distributor { get; set; }
    public DistributorsDetailsDto details { get; set; }
    public int max_level { get; set; }
    public List<BinaryDetailsOutputDto> level_list { get; set; } = new List<BinaryDetailsOutputDto>();
}

public class BinaryDetailsOutputDto
{
    public int id { get; set; }
    public int? parentId { get; set; }
    public string imageUrl { get; set; }
    public List<BinaryDetailsOutputDto> children { get; set; }
    public string content { get; set; }
    public bool is_last { get; set; } = false;
}