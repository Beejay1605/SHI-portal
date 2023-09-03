using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.BaseDto
{
    public class ProductsImageDto
    {
      
        public int img_id { get; set; }

        public string photo_base_64 { get; set; }

        public int img_indx { get; set; }

   
    }
}
