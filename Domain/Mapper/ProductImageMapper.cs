using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class ProductImageMapper : MapperBase<ProductImagesEntity, ProductsImageDto>
{
    public ProductsImageDto Map(ProductImagesEntity source)
    {
        return new ProductsImageDto
        {
            img_id = source.ID,
            photo_base_64 = source.PHOTO_PATH,
            img_indx = source.IMG_INDEX
        };
    }

    public ProductImagesEntity Reverse(ProductsImageDto source)
    {
        return new ProductImagesEntity
        {
            ID = source.img_id,
            PHOTO_PATH = source.photo_base_64,
            IMG_INDEX = source.img_indx
        };
    }
}