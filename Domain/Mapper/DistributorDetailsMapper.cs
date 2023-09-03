using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class DistributorDetailsMapper : MapperBase<DistributorsDetailsEntity, DistributorsDetailsDto>
{
    public DistributorsDetailsDto Map(DistributorsDetailsEntity source)
    {
        return new DistributorsDetailsDto
        {
            ident = source.DISTRIBUTOR_ID,
            user_ref_ident = source.USER_CRED_REF,
            first_name = source.FIRSTNAME,
            last_name = source.LASTNAME,
            middle_name = source.MIDDLENAME,
            suffix_name = source.SUFFIX,
            complete_address = source.ADDRESS,
            mobile_number = source.CONTACT_NUMBER,
            birth_date = source.BIRTH_DATE,
            fb_messenger_account = source.MESSENGER_ACCOUNT,
            gender = source.GENDER,
            tin = source.TIN_NUMBER,
            account_type = source.ACCOUNT_TYPE,
            accounts_count = source.NUMBER_OF_ACCOUNTS,
            upline_ref_id = source.UPLINE_REF_ID,
            user_picture_base_64 = source.PICTURE_PATH,
            created_by = source.CREATED_BY,
            updated_by = source.UPDATED_BY
        };
    }

    public DistributorsDetailsEntity Reverse(DistributorsDetailsDto source)
    {
        return new DistributorsDetailsEntity
        {
            DISTRIBUTOR_ID = source.ident,
            USER_CRED_REF = source.user_ref_ident,
            FIRSTNAME = source.first_name,
            LASTNAME = source.last_name,
            MIDDLENAME = source.middle_name,
            SUFFIX = source.suffix_name,
            ADDRESS = source.complete_address,
            CONTACT_NUMBER = source.mobile_number,
            BIRTH_DATE = source.birth_date,
            MESSENGER_ACCOUNT = source.fb_messenger_account,
            GENDER = source.gender,
            TIN_NUMBER = source.tin,
            ACCOUNT_TYPE = source.account_type,
            NUMBER_OF_ACCOUNTS = source.accounts_count,
            UPLINE_REF_ID = source.upline_ref_id,
            PICTURE_PATH = source.user_picture_base_64,
            CREATED_BY = source.created_by,
            UPDATED_BY = source.updated_by
        };
    }
}