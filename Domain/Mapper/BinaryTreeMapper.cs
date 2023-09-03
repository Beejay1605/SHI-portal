using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class BinaryTreeMapper : MapperBase<BinaryTreeEntity, BinaryTreeBaseDto>
{
    
    public BinaryTreeBaseDto Map(BinaryTreeEntity source)
    {
        var destination = new BinaryTreeBaseDto();
        destination.ident = source.ID;
        destination.distibutor_ident = source.DISTRIBUTOR_REF;
        destination.level = source.LEVELS;
        destination.created_dt = source.CREATED_AT_UTC;
        destination.updated_dt = source.UPDATED_AT_UTC;
        destination.upline_ident = source.UPLINE_DETAILS_REF;
        destination.grand_upline_ident = source.GRAND_UPLINE_DETAILS_REF;
        destination.parent_bin_ident = source.PARENT_BINARY_REF;
        destination.child_left_bin_ident = source.CHILD_LEFT_BINARY_REF;
        destination.child_right_bin_ident = source.CHILD_RIGHT_BINARY_REF;
        destination.position = source.POSITION;

        return destination;
    }

    public BinaryTreeEntity Reverse(BinaryTreeBaseDto source)
    {

        var destination = new BinaryTreeEntity
        {
            ID = source.ident,
            DISTRIBUTOR_REF = source.distibutor_ident,
            UPLINE_DETAILS_REF = source.upline_ident,
            GRAND_UPLINE_DETAILS_REF = source.grand_upline_ident,
            PARENT_BINARY_REF = source.parent_bin_ident,
            CHILD_LEFT_BINARY_REF = source.child_left_bin_ident,
            CHILD_RIGHT_BINARY_REF = source.child_right_bin_ident,
            POSITION = source.position,
            LEVELS = source.level,
            CREATED_AT_UTC = source.created_dt,
            UPDATED_AT_UTC = source.updated_dt
        };
        return destination;
    }
}