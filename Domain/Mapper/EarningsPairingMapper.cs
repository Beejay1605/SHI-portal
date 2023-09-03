using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class EarningsPairingMapper:  MapperBase<EarningsPairingEntity, EarningsPairingBaseDto>
{
    public EarningsPairingBaseDto Map(EarningsPairingEntity source)
    {
        return new EarningsPairingBaseDto
        {
            ident = source.ID,
            left_bin_ident = source.LEFT_BIN_ID,
            right_bin_ident = source.RIGHT_BIN_ID,
            ben_bin_ident = source.BENEF_BIN_ID,
            ben_dist_ident = source.BENEF_DIST_ID,
            earn_amount = source.AMOUNT,
            is_paid = source.IS_ENCASH,
            level = source.LEVEL,
            dt_created = source.CREATED_DT
        };
    }

    public EarningsPairingEntity Reverse(EarningsPairingBaseDto source)
    {
        return new EarningsPairingEntity
        {
            ID = source.ident,
            LEFT_BIN_ID = source.left_bin_ident,
            RIGHT_BIN_ID = source.right_bin_ident,
            BENEF_BIN_ID = source.ben_bin_ident,
            BENEF_DIST_ID = source.ben_dist_ident,
            AMOUNT = source.earn_amount,
            IS_ENCASH = source.is_paid,
            LEVEL = source.level,
            CREATED_DT = source.dt_created
        };
    }
}