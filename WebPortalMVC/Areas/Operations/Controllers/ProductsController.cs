using Dapper;
using Domain.DTO;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Distributors.Output;
using Domain.DTO.Operations.Products.Input;
using Domain.DTO.Operations.Products.Output;
using Domain.Entity;
using FluentValidation;
using FluentValidation.Results;
using Manager.Commons.Const;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using System.Net;
using WebPortalMVC.Authorizations;
using WebPortalMVC.FluentValidations.Operations.Distributors;
using WebPortalMVC.FluentValidations.Operations.Products;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class ProductsController : Controller
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;
        private readonly IFileHelper fileHelper;
        private readonly IWebHostEnvironment environment;

        public ProductsController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileHelper fileHelper)
        {
            this.connectionFactory = connectionFactory;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.fileHelper = fileHelper;
        }


        public async Task<IActionResult> Index()
        {
            ViewData["SidebarLocation"] = "Products";
            return View();
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> getProductsAction()
        {
            var result = new List<ProductsOutputDto>();
            var conn = await connectionFactory.CreateConnectionAsync();

            var query_string = @"SELECT * FROM products where STATUS <> @status";

            var products = conn.Query<ProductsEntity>(query_string, new
            {
                status = ProductStatusEnum.DELETED.ToString()
            });

            var query_profile = @"select * from product_images where PRODUCT_REF IN @ids and IMG_INDEX = 0";

            var prof_img = conn.Query<ProductImagesEntity>(query_profile, new
            {
                ids = products.Select(p => p.ID).ToList()
            });

            foreach (var pr in products)
            {

                if(pr.COVER_PHOTO_PATH != null)
                {
                    try
                    {
                        pr.COVER_PHOTO_PATH = fileHelper.GetImageUrl(pr.COVER_PHOTO_PATH);
                    }
                    catch(Exception ex) 
                    {
                        pr.COVER_PHOTO_PATH = "";
                    }
                }
                var img = prof_img.Where(x => x.PRODUCT_REF == pr.ID).FirstOrDefault();
                string base_64_String = string.Empty;

                if (img  != null)
                {
                    if(img.PHOTO_PATH != null)
                    {
                        try
                        {
                            base_64_String = fileHelper.GetImageUrl(img.PHOTO_PATH);
                        }
                        catch(Exception ex) 
                        {
                            base_64_String = string.Empty;
                        }
                    }
                }
                if(pr.DESCCRIPTION_FILE_PATH != null)
                {
                    try
                    {
                        pr.DESCCRIPTION_FILE_PATH = await fileHelper.getTextFileContent(pr.DESCCRIPTION_FILE_PATH);
                    }
                    catch (Exception ex)
                    {
                        pr.DESCCRIPTION_FILE_PATH = string.Empty;
                    }
                }
                result.Add(new ProductsOutputDto
                {
                    ID = pr.ID,
                    p_code = pr.PRODUCT_CODE,
                    p_name = pr.PRODUCT_NAME,
                    price = pr.SRP_PRICE,
                    category = pr.CATEGORY,
                    mini_desc = pr.MINI_DESCRIPTION,
                    cover_photo = pr.COVER_PHOTO_PATH,
                    description = pr.DESCCRIPTION_FILE_PATH,
                    created_dt = pr.CREATED_AT_UTC,
                    updated_dt = pr.UPDATED_AT_UTC,
                    deleted_dt = pr.DELETED_AT_UTC,
                    status = pr.STATUS,
                    remarks = pr.REMARKS,
                    package = pr.IS_PACKAGE,
                    created_by = pr.CREATED_BY,
                    updated_by = pr.UPDATED_BY,
                    product_img = base_64_String
                }) ;
            }

            conn.Close();
            conn.Dispose();

            return Ok(result);
        }
        
        
        public async Task<IActionResult> Create()
        {

            //var s = await currentUserService.CurrentUser();
            var result = new CreatePageOutputDto();

            var conn = await connectionFactory.CreateConnectionAsync();

            var pr = conn.Query<ProductsEntity>(@"
                    SELECT * FROM PRODUCTS WHERE STATUS = @STATUS
            ", new
            {
                STATUS = ProductStatusEnum.ACTIVE.ToString(),
            }).ToList(); 

            var p_image_r = conn.Query<ProductImagesEntity>(@"
                    SELECT * FROM product_images WHERE  PRODUCT_REF in @IDS
            ", new
            {
                IDS = pr.Select(x => x.ID).ToList(),
            }).ToList();

            foreach (var x in pr)
            {
                var temp = new ProductBaseDto
                {
                    ident = x.ID,
                    p_code = x.PRODUCT_CODE,
                    p_name = x.PRODUCT_NAME,
                    p_price = x.SRP_PRICE,
                    membership_price = x.MEMBERS_PRICE,
                    non_membership_discounted_price = x.NON_MEMBERS_DISCOUNTED_PRICE,
                    profit = x.COMPANY_PROFIT,
                    total_payout = x.PAYOUT_TOTAL,
                    p_category = x.CATEGORY,
                    p_mini_desc = x.MINI_DESCRIPTION,
                    p_created_dt = x.CREATED_AT_UTC,
                    p_status = x.STATUS,
                    p_remarks = x.REMARKS,
                    p_is_package = x.IS_PACKAGE,
                };

                var imgs = p_image_r.Where(z => z.PRODUCT_REF == x.ID).ToList();

                if (imgs.Count() > 0)
                {
                    try
                    {
                        temp.picture = fileHelper.GetImageUrl(imgs[0].PHOTO_PATH);
                    }
                    catch
                    {
                        temp.picture = "";
                    }
                }
                result.products.Add(temp);
            }

            conn.Close();
            conn.Dispose(); 
            return View(result);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var result = new ProductEditOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();

            string query_string = @"select * from products where ID = @id and STATUS <> @status";

            var products = conn.Query<ProductsEntity>(query_string, new
            {
                id = id,
                status = ProductStatusEnum.DELETED.ToString()
            }).FirstOrDefault();
            

            if(products == null)
            {
                return Redirect("Index");
            }

            var query_img = @"select * from product_images where PRODUCT_REF = @id";

            var img_data = conn.Query<ProductImagesEntity>(query_img, new
            {
                id = id

            }).ToList();

            if(img_data.Count() > 0)
            {
                foreach(var img in img_data)
                {
                    try
                    {
                        img.PHOTO_PATH = fileHelper.GetImageUrl(img.PHOTO_PATH);
                    }
                    catch (Exception ex) 
                    {
                        img.PHOTO_PATH = "";
                    }
                }
            }

            if (!string.IsNullOrEmpty(products.COVER_PHOTO_PATH))
            {
                try
                {
                    products.COVER_PHOTO_PATH = fileHelper.GetImageUrl(products.COVER_PHOTO_PATH);
                }
                catch (Exception ex)
                {
                    products.COVER_PHOTO_PATH = "";
                }
            }

            if(!string.IsNullOrEmpty(products.DESCCRIPTION_FILE_PATH))
            {
                try
                {
                    products.DESCCRIPTION_FILE_PATH = await fileHelper.getTextFileContent(products.DESCCRIPTION_FILE_PATH);
                }
                catch (Exception ex)
                {
                    products.DESCCRIPTION_FILE_PATH = "";
                }
            }


            result = new ProductEditOutputDto
            {
                ID = products.ID,
                p_code = products.PRODUCT_CODE,
                p_name = products.PRODUCT_NAME,
                
                
                price = products.SRP_PRICE,
                membership_price = products.MEMBERS_PRICE,
                non_membership_discounted_price = products.NON_MEMBERS_DISCOUNTED_PRICE,
                profit = products.COMPANY_PROFIT,
                total_payout = products.PAYOUT_TOTAL,
                
                
                category = products.CATEGORY,
                cover_photo = products.COVER_PHOTO_PATH,
                description = products.DESCCRIPTION_FILE_PATH,
                mini_desc = products.MINI_DESCRIPTION,
                created_dt = products.CREATED_AT_UTC,
                updated_dt = products.UPDATED_AT_UTC,
                deleted_dt = products.DELETED_AT_UTC,
                status = products.STATUS,
                remarks = products.REMARKS,
                package = products.IS_PACKAGE,
                created_by = products.CREATED_BY,
                updated_by = products.UPDATED_BY,
                is_package = products.IS_PACKAGE,
                product_img = img_data.Select(x => new ProductsImageDto
                {

                    img_id = x.ID,
                    photo_base_64 = x.PHOTO_PATH,
                    img_indx = x.IMG_INDEX

                }).ToList()
            };

            
            

            var pr = conn.Query<ProductsEntity>(@"
                    SELECT * FROM PRODUCTS WHERE STATUS = @STATUS
            ", new
            {
                STATUS = ProductStatusEnum.ACTIVE.ToString(),
            }).ToList();

            var p_image_r = conn.Query<ProductImagesEntity>(@"
                    SELECT * FROM product_images WHERE  PRODUCT_REF in @IDS
            ", new
            {
                IDS = pr.Select(x => x.ID).ToList(),
            }).ToList();



            foreach (var x in pr)
            {
                var temp = new ProductBaseDto
                {
                    ident = x.ID,
                    p_code = x.PRODUCT_CODE,
                    p_name = x.PRODUCT_NAME,
                    p_price = x.SRP_PRICE,
                    p_category = x.CATEGORY,
                    p_mini_desc = x.MINI_DESCRIPTION,
                    p_created_dt = x.CREATED_AT_UTC,
                    p_status = x.STATUS,
                    p_remarks = x.REMARKS,
                    p_is_package = x.IS_PACKAGE,
                };

                var imgs = p_image_r.Where(z => z.PRODUCT_REF == x.ID).ToList();

                if (imgs.Count() > 0)
                {
                    try
                    {
                        temp.picture = fileHelper.GetImageUrl(imgs[0].PHOTO_PATH);
                    }
                    catch
                    {
                        temp.picture = "";
                    }
                }
                result.products.Add(temp);
            }

            if (products.IS_PACKAGE == true)
            {
                var package_result = conn.Query<PackageProductsEntity>(@"
                    SELECT * FROM package_products WHERE  PACKAGE_ID = @ID
                ", new
                {
                    ID = products.ID
                }).ToList();

                var package_product_result = conn.Query<ProductsEntity>(@"
                    SELECT * FROM products WHERE ID IN @IDS
                ", new
                {
                    IDS = package_result.Select(x => x.PRODUCT_REF).ToList()
                }).ToList();


                result.package_products = package_result.Select(x => new PackageProductBaseDto
                {
                    ident = x.ID,
                    p_ident = x.PACKAGE_ID,
                    single_p_ref = x.PRODUCT_REF,
                    quantity = x.QUANTITY
                }).ToList();


                var package_query_img = @"select * from product_images where PRODUCT_REF IN @id";

                var package_img_data = conn.Query<ProductImagesEntity>(package_query_img, new
                {
                    id = package_result.Select(x => x.PRODUCT_REF)
                }).ToList();

                foreach (var pp in result.package_products)
                {
                    var prod_details = package_product_result.Where(x => x.ID == pp.single_p_ref).FirstOrDefault();

                    pp.single_product = new ProductBaseDto
                    {
                        ident = prod_details.ID,
                        p_code = prod_details.PRODUCT_CODE,
                        p_name = prod_details.PRODUCT_NAME,
                        p_price = prod_details.SRP_PRICE,
                        p_category = prod_details.CATEGORY,
                        p_mini_desc = prod_details.MINI_DESCRIPTION,
                        p_status = prod_details.STATUS,
                        p_remarks = prod_details.REMARKS,
                        p_is_package = prod_details.IS_PACKAGE
                    };

                    var imgs = package_img_data.Where(x => x.PRODUCT_REF == prod_details.ID).FirstOrDefault();
                    if (imgs != null)
                    {
                        try
                        {
                            pp.single_product.picture = fileHelper.GetImageUrl(imgs.PHOTO_PATH);
                        }
                        catch (Exception ex)
                        {
                            pp.single_product.picture = "";
                        }
                    }
                }
            }



            conn.Close();
            conn.Dispose();

            return View(result);
        }



        [HttpPut]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> UpdateProductsAction(UpdateProductsInputDto input)
        {
            UpdateProductsFValidation create_validation = new UpdateProductsFValidation();
            ValidationResult result = create_validation.Validate(input);

            if (!result.IsValid)
            {
                return StatusCode(400, result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }

            var conn = await connectionFactory.CreateConnectionAsync();

            try
            {

                var product_details = conn.Query<ProductsEntity>(@"SELECT * FROM products WHERE ID = @id", new
                {
                    id = input.ID
                }).FirstOrDefault(); 

                if (product_details == null)
                {
                    return BadRequest("Product Does'nt exist");
                }

                var images_result = conn.Query<ProductImagesEntity>(@"SELECT * FROM product_images
                        WHERE PRODUCT_REF=@ID", new
                {
                    id = input.ID
                });


                product_details.PRODUCT_NAME = input.name;
                product_details.SRP_PRICE = (input.price ?? 0);
                product_details.COMPANY_PROFIT = (input.profit ?? 0);
                product_details.PAYOUT_TOTAL = (input.total_payout ?? 0);
                product_details.CATEGORY = input.p_category;
                product_details.MINI_DESCRIPTION = input.mini_desc;

                if (input.membership_price != null)
                {
                    product_details.MEMBERS_PRICE = input.membership_price;
                }
                
                if (input.non_membership_discounted_price != null)
                {
                    product_details.NON_MEMBERS_DISCOUNTED_PRICE = input.non_membership_discounted_price;
                }

                unitOfWork.ProductImageRepository.DeleteList(images_result);
                if (product_details.DESCCRIPTION_FILE_PATH != null)
                {
                    await fileHelper.replaceTextFile((input.description ?? ""), product_details.DESCCRIPTION_FILE_PATH );
                }
                else
                {
                    await fileHelper.replaceTextFile((input.description ?? ""), $"{FilesConst.PRODUCT_DESCRIPTION_PATH}{Guid.NewGuid().ToString()}.txt");
                }

                string folderName = Guid.NewGuid().ToString();
                string img_path = string.Empty;
                if (string.IsNullOrEmpty(product_details.COVER_PHOTO_PATH))
                {
                    img_path = FilesConst.PRODUCT_IMAGES_PATH + folderName + @"\";
                }
                else
                {
                    var arrPath = product_details.COVER_PHOTO_PATH.Replace(FilesConst.PRODUCT_IMAGES_PATH, "").Split(@"\");
                    img_path = FilesConst.PRODUCT_IMAGES_PATH + arrPath[0] + @"\";
                }
                
                if (input.cover_photo != null)
                {
                    product_details.COVER_PHOTO_PATH = img_path + $"COVER{System.IO.Path.GetExtension(input.cover_photo.FileName)}";
                    await fileHelper.replaceFile(product_details.COVER_PHOTO_PATH, input.cover_photo);
                }
                else
                {
                    await fileHelper.deleteFile(product_details.COVER_PHOTO_PATH);
                    product_details.COVER_PHOTO_PATH = null;
                }

                foreach (var image in images_result)
                {
                    await fileHelper.deleteFile(image.PHOTO_PATH);
                }
                 
                if (!Directory.Exists(img_path))
                {
                    Directory.CreateDirectory(img_path);
                }
                var product_imgs = new List<ProductImagesEntity>();
                int idx = 0;
                if (input.pictures != null)
                {
                    foreach (var img in input.pictures)
                    {
                        var img_entity = new ProductImagesEntity
                        {
                            IMG_INDEX = idx,
                            PRODUCT_REF = product_details.ID
                        };
                        
                        string picture_path = img_path + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(img.FileName);
                        var picture_result = await fileHelper.saveFile(picture_path, img);
                        img_entity.PHOTO_PATH = picture_path;
                        if (picture_result == true)
                        {
                            product_imgs.Add(img_entity);
                        }
                        idx++;
                    }
                }

                unitOfWork.ProductRepository.Update(product_details);
                await unitOfWork.ProductImageRepository.AddRangeAsync(product_imgs);

                if (input.is_package == true)
                {
                    var to_delete = conn.Query<PackageProductsEntity>(@"SELECT * FROM package_products 
                            where PACKAGE_ID=@PACKAGE_ID", new
                    {
                        PACKAGE_ID = product_details.ID
                    }).ToList();

                    unitOfWork.PackageProductsRepository.DeleteList(to_delete);

                    var package_products = input.package_product.Select(x => new PackageProductsEntity
                    {
                        PACKAGE_ID = product_details.ID,
                        PRODUCT_REF = x.single_p_ref,
                        QUANTITY = x.quantity,
                        CREATE_UPDATE_DT = DateTime.UtcNow
                    }).ToList();

                    await unitOfWork.PackageProductsRepository.AddRangeAsync(package_products);
                }

                await unitOfWork.CommitAsync();

                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> CreateAction(CreateProductsInputDto input)
        {
            ProductsFValidation create_validation = new ProductsFValidation();
            ValidationResult result = create_validation.Validate(input);

            if (!result.IsValid)
            {
                return StatusCode(400, result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }

            var conn = await connectionFactory.CreateConnectionAsync();

            try
            {
                var product_Id = Guid.NewGuid();
                string productCode = product_Id.ToString("N").Substring(0, 8).ToUpper() + DateTime.UtcNow.ToString("yyMMddHHmm");

                var currentUser = await currentUserService.CurrentUser();
                var product = new ProductsEntity
                {
                    PRODUCT_NAME = input.name,
                    PRODUCT_CODE = productCode,

                    SRP_PRICE = (input.price ?? 0),
                    // MEMBERS_PRICE = (input.membership_price ?? 0),
                    // NON_MEMBERS_DISCOUNTED_PRICE = (input.non_membership_discounted_price ?? 0),
                    COMPANY_PROFIT = (input.profit ?? 0),
                    PAYOUT_TOTAL = (input.total_payout ?? 0),

                    CATEGORY = input.p_category,
                    //DESCRIPTIONS = input.description,
                    STATUS = ProductStatusEnum.ACTIVE.ToString(),
                    IS_PACKAGE = input.is_package,
                    CREATED_AT_UTC = DateTime.UtcNow,
                    CREATED_BY = currentUser.id,
                    MINI_DESCRIPTION = (input.mini_desc ?? string.Empty)

                };

                if (input.membership_price != null)
                {
                    product.MEMBERS_PRICE = input.membership_price;
                }
                
                if (input.non_membership_discounted_price != null)
                {
                    product.NON_MEMBERS_DISCOUNTED_PRICE = input.non_membership_discounted_price;
                }

                string folderNameDesc = Guid.NewGuid().ToString();
                var _desc_result = await fileHelper.saveTextFile(input.description,
                    FilesConst.PRODUCT_DESCRIPTION_PATH, folderNameDesc + ".txt");
                if (_desc_result == true)
                {
                    product.DESCCRIPTION_FILE_PATH = FilesConst.PRODUCT_DESCRIPTION_PATH + folderNameDesc + ".txt";
                }

                string folderPathName = Guid.NewGuid().ToString();
                string img_path = FilesConst.PRODUCT_IMAGES_PATH + folderPathName + @"\";
                if (!Directory.Exists(img_path))
                {
                    Directory.CreateDirectory(img_path);
                }
                if(input.cover_photo != null)
                {
                    string cover_path = img_path + $"COVER{System.IO.Path.GetExtension(input.cover_photo.FileName)}";
                    var _img_cover = await fileHelper.saveFile(cover_path, input.cover_photo);
                    if (_img_cover == true)
                    {
                        product.COVER_PHOTO_PATH = cover_path;
                    }
                }
              
                await unitOfWork.ProductRepository.AddAsync(product);
                await unitOfWork.CommitAsync();
                var product_imgs = new List<ProductImagesEntity>();
                int idx = 0;
                if (input.pictures != null)
                {
                    foreach (var img in input.pictures)
                    { 
                        var img_entity = new ProductImagesEntity
                        {
                            IMG_INDEX = idx,
                            PRODUCT_REF = product.ID
                        };
                        string picture_path = img_path + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(img.FileName);
                        var picture_result = await fileHelper.saveFile(picture_path, img);
                        img_entity.PHOTO_PATH = picture_path;
                        if (picture_result == true)
                        {
                            product_imgs.Add(img_entity);
                        } 
                        await unitOfWork.ProductImageRepository.AddAsync(img_entity);
                        idx++;
                    }
                }
                
                await unitOfWork.CommitAsync();
                if (input.is_package == true)
                {
                    var package_products = input.package_product.Select(x => new PackageProductsEntity
                    {
                        PACKAGE_ID = product.ID,
                        PRODUCT_REF = x.single_p_ref,
                        QUANTITY = x.quantity,
                        CREATE_UPDATE_DT = DateTime.UtcNow
                    }).ToList();
                    await unitOfWork.PackageProductsRepository.AddRangeAsync(package_products);
                } 

                await unitOfWork.CommitAsync();

                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }

        [HttpDelete]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DeleteAction(int id)
        {
            if (id == 0)
            {
                return BadRequest("Product doesn't exist");
            }


            var conn = await connectionFactory.CreateConnectionAsync();
            var product_details = conn.Query<ProductsEntity>(@"SELECT * FROM products where ID = @id", new
            {
                ID = id
            }).FirstOrDefault();

            try
            {

                product_details.STATUS = UserStatusEnum.DELETED.ToString();
                product_details.DELETED_AT_UTC = DateTime.UtcNow;

                unitOfWork.ProductRepository.Update(product_details);
                await unitOfWork.CommitAsync();

                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
        }
         
        [Route("Operations/Products/Index/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            ViewData["SidebarLocation"] = "Products";
            var result = new ProductEditOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();

            string query_string = @"select * from products where ID = @id and STATUS <> @status";

            var products = conn.Query<ProductsEntity>(query_string, new
            {
                id = id,
                status = ProductStatusEnum.DELETED.ToString()

            }).FirstOrDefault();


            if (products == null)
            {
                return Redirect("Index");
            }

            var query_img = @"select * from product_images where PRODUCT_REF = @id";

            var img_data = conn.Query<ProductImagesEntity>(query_img, new
            {
                id = id

            }).ToList();

            if (img_data.Count() > 0)
            {
                foreach (var img in img_data)
                {
                    try
                    {
                        img.PHOTO_PATH = fileHelper.GetImageUrl(img.PHOTO_PATH);
                    }
                    catch (Exception ex)
                    {
                        img.PHOTO_PATH = "";
                    }

                }
            }

            if (!string.IsNullOrEmpty(products.COVER_PHOTO_PATH))
            {
                try
                {
                    products.COVER_PHOTO_PATH = fileHelper.GetImageUrl(products.COVER_PHOTO_PATH);
                }
                catch (Exception ex)
                {
                    products.COVER_PHOTO_PATH = "";
                }
            }

            if (!string.IsNullOrEmpty(products.DESCCRIPTION_FILE_PATH))
            {
                try
                {
                    products.DESCCRIPTION_FILE_PATH = await fileHelper.getTextFileContent(products.DESCCRIPTION_FILE_PATH);
                }
                catch (Exception ex)
                {
                    products.DESCCRIPTION_FILE_PATH = "";
                }
            }
            result = new ProductEditOutputDto
            {
                ID = products.ID,
                p_code = products.PRODUCT_CODE,
                p_name = products.PRODUCT_NAME,
                
                price = products.SRP_PRICE,
                membership_price = products.MEMBERS_PRICE,
                non_membership_discounted_price = products.NON_MEMBERS_DISCOUNTED_PRICE,
                profit = products.COMPANY_PROFIT,
                total_payout = products.PAYOUT_TOTAL,
                
                category = products.CATEGORY,
                cover_photo = products.COVER_PHOTO_PATH,
                description = products.DESCCRIPTION_FILE_PATH,
                mini_desc = products.MINI_DESCRIPTION,
                created_dt = products.CREATED_AT_UTC,
                updated_dt = products.UPDATED_AT_UTC,
                deleted_dt = products.DELETED_AT_UTC,
                status = products.STATUS,
                remarks = products.REMARKS,
                package = products.IS_PACKAGE,
                created_by = products.CREATED_BY,
                updated_by = products.UPDATED_BY,
                product_img = img_data.Select(x => new ProductsImageDto
                {

                    img_id = x.ID,
                    photo_base_64 = x.PHOTO_PATH,
                    img_indx = x.IMG_INDEX

                }).ToList()
            };

            if (products.IS_PACKAGE == true)
            {
                var package_result = conn.Query<PackageProductsEntity>(@"
                    SELECT * FROM package_products WHERE  PACKAGE_ID = @ID
                ", new
                {
                    ID = products.ID
                }).ToList();

                var package_product_result = conn.Query<ProductsEntity>(@"
                    SELECT * FROM products WHERE ID IN @IDS
                ", new
                {
                    IDS = package_result.Select(x => x.PRODUCT_REF).ToList()
                }).ToList();


                result.package_products = package_result.Select(x => new PackageProductBaseDto
                {
                    ident = x.ID,
                    p_ident = x.PACKAGE_ID,
                    single_p_ref = x.PRODUCT_REF,
                    quantity = x.QUANTITY
                }).ToList();


                var package_query_img = @"select * from product_images where PRODUCT_REF IN @id";

                var package_img_data = conn.Query<ProductImagesEntity>(package_query_img, new
                {
                    id = package_result.Select(x => x.PRODUCT_REF)
                }).ToList();

                foreach (var pp in result.package_products)
                {
                    var prod_details = package_product_result.Where(x => x.ID == pp.single_p_ref).FirstOrDefault();

                    pp.single_product = new ProductBaseDto
                    {
                        ident = prod_details.ID,
                        p_code = prod_details.PRODUCT_CODE,
                        p_name = prod_details.PRODUCT_NAME,
                        p_price = prod_details.SRP_PRICE,
                        p_category = prod_details.CATEGORY, 
                        p_mini_desc = prod_details.MINI_DESCRIPTION,
                        p_status = prod_details.STATUS,
                        p_remarks = prod_details.REMARKS,
                        p_is_package = prod_details.IS_PACKAGE
                    };

                    var imgs = package_img_data.Where(x => x.PRODUCT_REF == prod_details.ID).FirstOrDefault();
                    if (imgs != null)
                    {
                        try
                        {
                            pp.single_product.picture = fileHelper.GetImageUrl(imgs.PHOTO_PATH);
                        }
                        catch (Exception ex)
                        {
                            pp.single_product.picture = "";
                        }
                    }
                } 
            }


            conn.Close();
            conn.Dispose();

            return View("Details",result);
        }


        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> getSignleProductsAction(int id)
        {
            var result = new ProductsOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();

            var query_string = @"SELECT * FROM products where ID=@ID AND STATUS <> @status";

            var products = conn.Query<ProductsEntity>(query_string, new
            {
                ID = id,
                status = ProductStatusEnum.DELETED.ToString()
            }).FirstOrDefault();


            if (products == null)
            {
                return Ok(new ProductsOutputDto());
            }

            var query_profile = @"select * from product_images where PRODUCT_REF=@id and IMG_INDEX = 0";

            var prof_img = conn.Query<ProductImagesEntity>(query_profile, new
            {
                id = id
            }).FirstOrDefault();

            string base_64_String = string.Empty;
            if (prof_img != null)
            {
                try
                {
                    base_64_String = fileHelper.GetImageUrl(prof_img.PHOTO_PATH);
                }
                catch
                {
                    base_64_String = string.Empty;
                }
            }

            var pr = products;
            result = new ProductsOutputDto
            {
                ID = pr.ID,
                p_code = pr.PRODUCT_CODE,
                p_name = pr.PRODUCT_NAME,
                price = pr.SRP_PRICE,
                category = pr.CATEGORY,
                mini_desc = pr.MINI_DESCRIPTION, 
                created_dt = pr.CREATED_AT_UTC,
                updated_dt = pr.UPDATED_AT_UTC,
                deleted_dt = pr.DELETED_AT_UTC,
                status = pr.STATUS,
                remarks = pr.REMARKS,
                package = pr.IS_PACKAGE,
                created_by = pr.CREATED_BY,
                updated_by = pr.UPDATED_BY,
                product_img = base_64_String
            };

            conn.Close();
            conn.Dispose();

            return Ok(result);
        }

    }
}
