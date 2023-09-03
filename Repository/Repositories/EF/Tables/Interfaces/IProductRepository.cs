﻿using Domain.DTO.Operations.Products.Input;
using Domain.Entity;
using Repository.Repositories.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF.Tables.Interfaces;

public interface IProductRepository : IRepository<ProductsEntity>
{
}
