namespace Domain.Mapper;

public interface MapperBase<SourceEntity, DestinationDTO>
{
    DestinationDTO Map(SourceEntity source);

    SourceEntity Reverse(DestinationDTO source);
}