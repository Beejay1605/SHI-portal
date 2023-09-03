using Dapper;
using Domain.DTO;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Distributors.Input;
using Domain.DTO.Operations.Distributors.Output;
using Domain.Entity;
using FluentValidation;
using FluentValidation.Results;
using Manager.Commons.Const;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Repository.Repositories.Dapper;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using System.IO;
using System.Net;
using System.Security.Claims;
using Manager.Commons.Services;
using WebPortalMVC.Authorizations;
using WebPortalMVC.FluentValidations.Operations.Distributors;
using WebPortalMVC.FluentValidations.PublicPage;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class DistributorsController : Controller
    {
        public DistributorsController(IUnitOfWork unitOfWork, IDateTimeService dateTimeService, IFileHelper fileHelper, IEncryptionHelper encryptionHelper, IWebHostEnvironment environment, IDapperRepositories dapper, IDbConnectionFactory connectionFactory, IJwtTokenService tokenService, ICurrentUserService currentUser)
        {
            this.unitOfWork = unitOfWork;
            this.dateTimeService = dateTimeService;
            this.fileHelper = fileHelper;
            this.encryptionHelper = encryptionHelper;
            this.environment = environment;
            _dapper = dapper;
            this.connectionFactory = connectionFactory;
            this.tokenService = tokenService;
            _currentUser = currentUser;
        }

        private readonly IUnitOfWork unitOfWork; 
        private readonly IDateTimeService dateTimeService;
        private readonly IFileHelper fileHelper;
        private readonly IEncryptionHelper encryptionHelper;
        private readonly IWebHostEnvironment environment;
        private readonly IDapperRepositories _dapper;
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IJwtTokenService tokenService;
        private readonly ICurrentUserService _currentUser; 


        public async Task<IActionResult> RegistrationAsync()
        {
            ViewData["SidebarLocation"] = "Distributors";
            var result = new RegistrationOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();

            string query_string = @"SELECT
                                        DD.`FIRSTNAME` as first_name,
                                        DD.`LASTNAME` as last_name,
                                        DD.`MIDDLENAME` as middle_name,
                                        DD.`SUFFIX` as suffix_name,
                                        DD.`ADDRESS` as complete_address,
                                        DD.`BIRTH_DATE` AS birth_date,
                                        DD.`CONTACT_NUMBER` as mobile_number,
                                        DD.`MESSENGER_ACCOUNT` as fb_messenger_account,
                                        DD.`TIN_NUMBER`  as tin,
                                        DD.`ACCOUNT_TYPE` AS type_of_account,
                                        DD.`PICTURE_PATH` as user_picture_base_64,
                                        UD.`STATUS` as status,
                                        UD.STATUS FROM distributors_details as DD
                                        INNER JOIN user_credentials as UD on DD.USER_CRED_REF = UD.ID";
            var distributors = conn.Query<DistributorsDetailsDto>(query_string, new
            {
                STATUS = UserStatusEnum.ACTIVE.ToString()
            }).ToList();

            result.distributor = distributors;
            result.distributor.Insert(0, new DistributorsDetailsDto()
            {
                first_name = string.Empty,
                last_name = string.Empty,
                ident = 0
            });
            conn.Close();
            conn.Dispose();
            return View(result);
        }

       // [AuthorizationViewAttribute("Operations")]
        public async Task<IActionResult> Index()
        { 
            ViewData["SidebarLocation"] = "Distributors";
            // var result = new RegistrationOutputDto();
            // result.distributor = await ActiveDistributors("");
            // result.distributor = result.distributor.Where(x => x.status != UserStatusEnum.DELETED.ToString()).ToList();
            return View();
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DistributorsList(string search)
        {
            search = search ?? "";
            var result = new RegistrationOutputDto();
            result.distributor = await ActiveDistributors(search);
            result.distributor = result.distributor.Where(x => x.status == UserStatusEnum.ACTIVE.ToString()).ToList();
            return Ok(result);
        }

        private async Task<List<DistributorsDetailsDto>> ActiveDistributors(string search)
        {
            ViewData["SidebarLocation"] = "Distributors";
            var result = new RegistrationOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();


            string query_string = @"SELECT 
                                        DD.USER_CRED_REF as user_ref_ident,
                                        DD.FIRSTNAME as first_name,
                                        DD.LASTNAME as last_name,
                                        DD.MIDDLENAME as middle_name,
                                        DD.SUFFIX as suffix_name,
                                        DD.ADDRESS as complete_address,
                                        DD.BIRTH_DATE AS birth_date,
                                        DD.CONTACT_NUMBER as mobile_number,
                                        DD.MESSENGER_ACCOUNT as fb_messenger_account,
                                        DD.TIN_NUMBER  as tin,
                                        DD.ACCOUNT_TYPE AS account_type,
                                        DD.PICTURE_PATH as user_picture_base_64,
                                        DD.UPLINE_REF_ID as upline_ref_id,
                                        UD.STATUS as status,
                                        UD.EMAIL as Email,
                                        UD.USERNAME as Username,
                                        UD.STATUS as status,
                                        DD.NUMBER_OF_ACCOUNTS as accounts_count,
                                        DD.DISTRIBUTOR_ID as ui_id
                                        FROM distributors_details as DD
                                        INNER JOIN user_credentials as UD on DD.USER_CRED_REF = UD.ID
                                        WHERE CONCAT(DD.FIRSTNAME ,' ', DD.MIDDLENAME ,' ', DD.LASTNAME) LIKE @search
                                        ";
            

           
            var distributors = conn.Query<DistributorsDetailsDto>(query_string, new
            { 
                search = $"%{search.Replace(" ","%").Replace(" ", "%").Replace(" ", "%")}%"
            }).ToList();
            result.distributor = distributors.Select(x => new DistributorsDetailsDto
                        {
                            ident = x.ident,
                            user_ref_ident= x.user_ref_ident,
                            first_name = x.first_name,
                            last_name = x.last_name,
                            middle_name = x.middle_name,
                            ui_id= x.ui_id,
                            Email = x.Email,
                            Username = x.Username,
                            suffix_name = x.suffix_name,
                            complete_address = x.complete_address,
                            birth_date = x.birth_date,
                            mobile_number = x.mobile_number,
                            fb_messenger_account = x.fb_messenger_account,
                            tin = x.tin,
                            type_of_account = x.type_of_account,
                            accounts_count = x.accounts_count,
                            upline_ref_id = x.upline_ref_id,
                            user_picture_base_64 = x.user_picture_base_64,
                            placement_location = x.placement_location,
                            status = x.status,
                            account_type = x.account_type.Replace("_", ""),
                            upline_details = (distributors.Where(y => y.ident == x.upline_ref_id).FirstOrDefault() ?? new DistributorsDetailsDto()),
                        }).ToList();
            
            int cc = 0;

            foreach (var dist in result.distributor)
            {
                if (dist.user_picture_base_64 != null)
                {
                    try
                    {
                        dist.user_picture_base_64 = fileHelper.GetImageUrl(dist.user_picture_base_64);
                    }
                    catch (Exception ex)
                    {
                        dist.user_picture_base_64 = "";
                    }
                }
                cc++;
            }

            conn.Close();
            conn.Dispose();


            
            return (result.distributor);
        }
        
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Index");
            }

            ViewData["SidebarLocation"] = "Distributors"; 
            var conn = await connectionFactory.CreateConnectionAsync();

            string query_string = @"SELECT 
                                        DD.`FIRSTNAME` as first_name,
                                        DD.`LASTNAME` as last_name,
                                        DD.`MIDDLENAME` as middle_name,
                                        DD.`GENDER` as gender,
                                        DD.`SUFFIX` as suffix_name,
                                        DD.`ADDRESS` as complete_address,
                                        DD.`BIRTH_DATE` AS birth_date,
                                        DD.`CONTACT_NUMBER` as mobile_number,
                                        DD.`MESSENGER_ACCOUNT` as fb_messenger_account,
                                        DD.`TIN_NUMBER`  as tin,
                                        DD.`ACCOUNT_TYPE` AS type_of_account,
                                        DD.`PICTURE_PATH` as user_picture_base_64,
                                        UD.`STATUS` as status,
                                        UD.`EMAIL` as email,
                                        UD.STATUS FROM distributors_details as DD
                                        INNER JOIN user_credentials as UD on DD.USER_CRED_REF = UD.ID
                                        WHERE DD.DISTRIBUTOR_ID =@id";

            var distributors = conn.Query<DistributorsDetailsDto>(query_string, new
            {
                id = id
            }).FirstOrDefault();


            if (distributors != null)
            { 
                if (distributors?.user_picture_base_64 != null)
                {
                    try
                    {
                        distributors.user_picture_base_64 = fileHelper.GetImageUrl(distributors.user_picture_base_64);
                    }
                    catch (Exception ex)
                    {
                        distributors.user_picture_base_64 = "";
                    }
                }
            }
             
            conn.Close();
            conn.Dispose();
            return View(distributors);
        }
  

        [ClaimRequirement("Operations")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationAction(RegistrationInputDto input)
        {
            input.middlename = input.middlename ?? "";
            input.firstname = input.firstname ?? "";
            input.lastname = input.lastname ?? "";
            input.suffix = input.suffix ?? "";
            input.age = input.age ?? "";
            input.sex = input.sex ?? "";
            input.completeAddress = input.completeAddress ?? "";
            input.contact = input.contact ?? "";
            input.email = input.email ?? "";
            input.msessenger = input.msessenger ?? "";
            input.tin = input.tin ?? "";
            input.directupLineCode = input.directupLineCode;  

            RegistrationFValidation registration_obj = new RegistrationFValidation();
            ValidationResult result = registration_obj.Validate(input);


            string query_duplicate = @"SELECT * FROM distributors_details WHERE LASTNAME=@LASTNAME";
            var conn = await connectionFactory.CreateConnectionAsync();
            var distributors = conn.Query<DistributorsDetailsDto>(query_duplicate, new
            {
                LASTNAME = input.lastname.ToUpper()
            }).ToList();

            var same_lastnames = distributors;
            distributors = distributors.Where(x => x.first_name == input.firstname.ToUpper() &&
                                x.middle_name == input.firstname.ToUpper() &&
                                    x.birth_date.ToString("yyyyMMdd") == input.dateOfBirth.ToString("yyyyMMdd")).ToList();

            if (distributors.Count() > 0)
            {

                var errors = new List<ValidationFailure>();
                errors.Add(new ValidationFailure
                {
                    PropertyName = "firstname",
                    ErrorMessage = "This person is already registered"
                });
                errors.Add(new ValidationFailure
                {
                    PropertyName = "lastname",
                    ErrorMessage = "This person is already registered"
                });
                errors.Add(new ValidationFailure
                {
                    PropertyName = "middlename",
                    ErrorMessage = "This person is already registered"
                });
                errors.Add(new ValidationFailure
                {
                    PropertyName = "dateOfBirth",
                    ErrorMessage = "This person is already registered"
                });
                result.Errors =  errors.ToList();
            }

            if (!result.IsValid)
            {
                return StatusCode(400, result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }

            try
            {
                Guid user_id = Guid.NewGuid();
                string user_path = Path.Combine(environment.ContentRootPath, FilesConst.DISTRIBUTORS_PICTURE_PATH);
                var currentUser = await _currentUser.CurrentUser();
                var details = new DistributorsDetailsEntity
                {
                    USER_CRED_REF = user_id,
                    FIRSTNAME = input.firstname.ToUpper(),
                    LASTNAME = input.lastname.ToUpper(),
                    MIDDLENAME = input.middlename.ToUpper(), 
                    ADDRESS = input.completeAddress.ToUpper(),
                    CONTACT_NUMBER = (input.contact ?? ""),
                    GENDER = (input?.sex ?? ""),
                    MESSENGER_ACCOUNT = input.msessenger,
                    BIRTH_DATE = input.dateOfBirth, 
                    TIN_NUMBER = input.tin,
                    ACCOUNT_TYPE = (input.noOfAccount > 1 ? AccountTypeEnum.MULTIPLE_ACCOUNT.ToString() : AccountTypeEnum.SINGLE_ACCOUNT.ToString()), 
                    NUMBER_OF_ACCOUNTS = input.noOfAccount,
                    CREATED_BY = currentUser.id
                };

                if (input.user_picture != null)
                {
                    details.PICTURE_PATH = FilesConst.DISTRIBUTORS_PICTURE_PATH + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(input.user_picture.FileName);
                }

                if (input.directupLineCode != 0)
                {
                    details.UPLINE_REF_ID = input.directupLineCode;
                }

                if (input.suffix != null)
                {
                    details.SUFFIX = input.suffix.ToUpper();
                }
                else
                {
                    details.SUFFIX = "";
                }
                var user_default_password = encryptionHelper.Encrypt(input.lastname.ToUpper() + input.dateOfBirth.ToString("yyyyMMdd"));

                var user_credentials = new UserCredentialsEntity
                {
                    ID = user_id,
                    USERNAME = (input.firstname.Substring(0,1) + (input.middlename.Length > 0 ? input.middlename.Substring(0,1) : "" ) +input.lastname + (same_lastnames.Count() + 1)).ToLower(),
                    PASSWORD =  user_default_password,
                    EMAIL = input.email,
                    CREATED_AT_UTC = DateTime.UtcNow,
                    STATUS = UserStatusEnum.ACTIVE.ToString(),
                    ACCESS_LEVEL = AccessLevelEnum.Distributor.ToString(),
                    REMARKS = ""
                };
                 
                if (!Directory.Exists(user_path))
                {
                    Directory.CreateDirectory(user_path);
                }



                if (input.user_picture != null)
                {
                    using (var fileStream = new FileStream(details.PICTURE_PATH, FileMode.Create)){
                        await input.user_picture.CopyToAsync(fileStream);
                    } 
                     
                }


                await unitOfWork.UserCredentialsRepository.AddAsync(user_credentials);
                await unitOfWork.DistributorsDetailsRepository.AddAsync(details);
                await unitOfWork.CommitAsync();

                conn.Close();
                conn.Dispose();
                return Ok();


            }catch(Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await unitOfWork.RollbackAsync(); 
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            } 
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Index");
            }
             

            ViewData["SidebarLocation"] = "Distributors";
             
            var conn = await connectionFactory.CreateConnectionAsync();

            string query_string_dist = @"SELECT 
                                        DD.USER_CRED_REF as user_ref_ident,
                                        DD.DISTRIBUTOR_ID as ui_id,
                                        DD.`FIRSTNAME` as first_name,
                                        DD.`LASTNAME` as last_name,
                                        DD.`MIDDLENAME` as middle_name,
                                        DD.`GENDER` as gender,
                                        DD.`SUFFIX` as suffix_name,
                                        DD.`ADDRESS` as complete_address,
                                        DD.`BIRTH_DATE` AS birth_date,
                                        DD.`CONTACT_NUMBER` as mobile_number,
                                        DD.`MESSENGER_ACCOUNT` as fb_messenger_account,
                                        DD.`TIN_NUMBER`  as tin,
                                        DD.`ACCOUNT_TYPE` AS type_of_account,
                                        DD.`PICTURE_PATH` as user_picture_base_64,
                                        DD.UPLINE_REF_ID as upline_ref_id,
                                        UD.`STATUS` as status,
                                        UD.`EMAIL` as email,
                                        UD.REMARKS as remarks
                                        FROM distributors_details as DD
                                        INNER JOIN user_credentials as UD on DD.USER_CRED_REF = UD.ID
                                        WHERE DD.DISTRIBUTOR_ID =@id";

            var result = conn.Query<RegistrationOutputDto>(query_string_dist, new
            {
                id = id
            }).FirstOrDefault();

            if (result != null)
            {
                if (result?.user_picture_base_64 != null)
                {
                    try
                    {
                        result.user_picture_base_64 = fileHelper.GetImageUrl(result.user_picture_base_64);
                        
                    }
                    catch (Exception ex)
                    {
                        result.user_picture_base_64 = "";
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            } 

            string query_string = @"SELECT 
                                    DD.USER_CRED_REF as user_ref_ident,
                                    DD.`FIRSTNAME` as first_name,
                                    DD.`LASTNAME` as last_name,
                                    DD.`MIDDLENAME` as middle_name,
                                    DD.`SUFFIX` as suffix_name,
                                    DD.`ADDRESS` as complete_address,
                                    DD.`BIRTH_DATE` AS birth_date,
                                    DD.`CONTACT_NUMBER` as mobile_number,
                                    DD.`MESSENGER_ACCOUNT` as fb_messenger_account,
                                    DD.`TIN_NUMBER`  as tin,
                                    DD.`ACCOUNT_TYPE` AS type_of_account,
                                    DD.`PICTURE_PATH` as user_picture_base_64,
                                    DD.UPLINE_REF_ID as upline_ref_id,
                                    UD.`STATUS` as status,
                                    UD.REMARKS as remarks
                                    FROM distributors_details as DD
                                    INNER JOIN user_credentials as UD on DD.USER_CRED_REF = UD.ID
                                    WHERE UD.`STATUS` = @STATUS";

            var distributors = conn.Query<DistributorsDetailsDto>(query_string, new
            {
                STATUS = UserStatusEnum.ACTIVE.ToString()
            }).ToList();

            result.distributor = distributors; 
            if (result.upline_ref_id == null)
            {

                result.distributor.Insert(0, new DistributorsDetailsDto()
                {
                    first_name = string.Empty,
                    last_name = string.Empty,
                    ident = 0
                });
            }

            foreach (UserStatusEnum status in (UserStatusEnum[])Enum.GetValues(typeof(UserStatusEnum)))
            {
                result.status_list.Add(status.ToString());
            }

            conn.Close();
            conn.Dispose();
            return View(result);
        }


        [HttpDelete]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DeleteAction(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("User does'nt exist");
            }

            var conn = await connectionFactory.CreateConnectionAsync();
            var user_cred = conn.Query<UserCredentialsEntity>(@"SELECT * FROM USER_CREDENTIALS 
                                WHERE ID=@ID", new
            {
                ID = id
            }).FirstOrDefault();

            if (user_cred == null)
            {
                return BadRequest("User id does'nt exist");
            }


            try
            {

                user_cred.STATUS = UserStatusEnum.DELETED.ToString();
                user_cred.DELETE_AT_UTC = DateTime.UtcNow;

                unitOfWork.UserCredentialsRepository.Update(user_cred);
                await unitOfWork.CommitAsync();

                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch(Exception ex)
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
        public async Task<IActionResult> UpdateAction(RegistrationInputDto input)
        {

            #region validation
            input.middlename = input.middlename ?? "";
            input.firstname = input.firstname ?? "";
            input.lastname = input.lastname ?? "";
            input.suffix = input.suffix ?? "";
            input.age = input.age ?? "";
            input.sex = input.sex ?? "";
            input.completeAddress = input.completeAddress ?? "";
            input.contact = input.contact ?? "";
            input.email = input.email ?? "";
            input.msessenger = input.msessenger ?? "";
            input.tin = input.tin ?? "";
            input.directupLineCode = input.directupLineCode;

            RegistrationFValidation registration_obj = new RegistrationFValidation();
            ValidationResult result = registration_obj.Validate(input);

            var conn = await connectionFactory.CreateConnectionAsync();
            
            if (!result.IsValid)
            {
                return StatusCode(400, result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }

            #endregion
            try
            {
                var details = conn.Query<DistributorsDetailsEntity>(@"
                    SELECT * FROM distributors_details WHERE DISTRIBUTOR_ID = @ID
                ", new
                {
                    ID = input.det_id
                }).FirstOrDefault();

                if (details == null)
                {
                    return BadRequest("User does'nt exist");
                }

                var user_cred = conn.Query<UserCredentialsEntity>(@"
                    SELECT * FROM user_credentials WHERE ID = @USER_CRED_REF
                ", new
                {
                    USER_CRED_REF = details.USER_CRED_REF
                }).FirstOrDefault();

                if (user_cred == null)
                {
                    return BadRequest("User does'nt exist");
                }

                string user_path = Path.Combine(environment.ContentRootPath, FilesConst.DISTRIBUTORS_PICTURE_PATH);

                details.FIRSTNAME = input.firstname.ToUpper();
                details.LASTNAME = input.lastname.ToUpper();
                details.MIDDLENAME = input.middlename.ToUpper();
                details.ADDRESS = input.completeAddress.ToUpper();
                details.CONTACT_NUMBER = (input.contact ?? "");
                details.GENDER = (input?.sex ?? "");
                details.MESSENGER_ACCOUNT = input.msessenger;
                details.BIRTH_DATE = input.dateOfBirth;
                details.TIN_NUMBER = input.tin;
                details.ACCOUNT_TYPE = (input.noOfAccount > 1 ? AccountTypeEnum.MULTIPLE_ACCOUNT.ToString() : AccountTypeEnum.SINGLE_ACCOUNT.ToString());
                details.NUMBER_OF_ACCOUNTS = input.noOfAccount;
                

                if (input.user_picture != null)
                {
                    
                    if (string.IsNullOrEmpty(details.PICTURE_PATH))
                    {
                        details.PICTURE_PATH = FilesConst.DISTRIBUTORS_PICTURE_PATH + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(input.user_picture.FileName);
                    }
                }



                if (input.directupLineCode != 0)
                {
                    details.UPLINE_REF_ID = input.directupLineCode;
                }

                if (input.suffix != null)
                {
                    details.SUFFIX = input.suffix.ToUpper();
                }
                else
                {
                    details.SUFFIX = "";
                }
                
                user_cred.EMAIL = input.email;
                user_cred.UPDATED_AT_UTC = DateTime.UtcNow;
                user_cred.STATUS = input.status;
                user_cred.REMARKS = input.remarks;

                if (!Directory.Exists(user_path))
                {
                    Directory.CreateDirectory(user_path);
                }

                if (Directory.Exists(details.PICTURE_PATH))
                {
                    System.IO.File.Delete(Path.Combine(environment.ContentRootPath, details.PICTURE_PATH)); 
                }

                if (input.user_picture != null)
                {
                    using (var fileStream = new FileStream(details.PICTURE_PATH, FileMode.Create))
                    {
                        await input.user_picture.CopyToAsync(fileStream);
                    } 
                }


                unitOfWork.DistributorsDetailsRepository.Update(details);
                unitOfWork.UserCredentialsRepository.Update(user_cred);
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
    }

}



