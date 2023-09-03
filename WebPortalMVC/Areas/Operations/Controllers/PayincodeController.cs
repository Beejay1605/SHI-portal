using System.ComponentModel.DataAnnotations;
using Domain.DTO.Operations.Payincodes.input;
using Domain.DTO.Operations.Payincodes.output;
using Domain.Entity;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using System.Net;
using Dapper;
using Domain.DTO.BaseDto;
using Manager.Commons.Enums;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class PayincodeController : Controller
    {
        public PayincodeController(IQRCodeHelper qrCodeHelper, IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, ILogger<PayincodeController> logger, ICurrentUserService currentUser, IEncryptionHelper encryptionHelper, IKeyHelper keyHelper)
        {
            _qrCodeHelper = qrCodeHelper;
            this.connectionFactory = connectionFactory;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.currentUser = currentUser;
            this.encryptionHelper = encryptionHelper;
            _keyHelper = keyHelper;
        }

        private readonly IQRCodeHelper _qrCodeHelper;
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<PayincodeController> logger;
        private readonly ICurrentUserService currentUser;
        private readonly IEncryptionHelper encryptionHelper;
        private readonly IKeyHelper _keyHelper;
 

        [Route("Operations/Payin-codes")]
        public IActionResult Index()
        {
            ViewData["SidebarLocation"] = "Pay-in Codes";
            
            
            return View();
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> PayinCodesList(string? search)
        {
            search = search ?? "";
            PayinCodeOutputDto result = new PayinCodeOutputDto();
            var conn = await connectionFactory.CreateConnectionAsync();
            string queryString = @"SELECT * FROM payin_codes 
                WHERE (CAST(TRANSACTION_REF as CHAR) LIKE @search) LIMIT 500";

            if (string.IsNullOrEmpty(search))
            {
                queryString = @"SELECT * FROM payin_codes ORDER BY CREATED_AT DESC LIMIT 500";
            }
            
            var codesResult = conn.Query<PayinCodesEntity>(queryString, new
            {
                search = $"%{search.Replace(" ", "%")}%"
            });
            var distinctData = codesResult.DistinctBy(x => x.TRANSACTION_REF).ToList();

            var ownerIds = distinctData.Select(x => x.DISTRIBUTOR_REF).DistinctBy(x => x).ToList();

            var owners = conn.Query<DistributorsDetailsEntity>(@"
                SELECT * FROM distributors_details WHERE DISTRIBUTOR_ID IN @ids
            ", new
            {
                ids = ownerIds
            }).ToList();
            
            
            
            result.payins = distinctData.Select(x => new PayinCodeBaseDto
            {
                ident = x.ID,
                code = codesResult.Where(y => y.TRANSACTION_REF == x.TRANSACTION_REF).Count().ToString(),
                distributor_ident = x.DISTRIBUTOR_REF,
                tran_ident = x.TRANSACTION_REF,
                date_created = x.CREATED_AT,
                date_updated = x.UPDATED_AT,
                expiration_date = x.EXPIRATION_DT,
                created_by = x.CREATED_BY,
                updated_by = x.UPDATED_BY,
                distributorsDetails = owners.Where(z => z.DISTRIBUTOR_ID == x.DISTRIBUTOR_REF).Select(
                        m => new DistributorsDetailsDto{
                            ident = m.DISTRIBUTOR_ID,
                            first_name = m.FIRSTNAME,
                            last_name = m.LASTNAME
                        }).FirstOrDefault()
            }).ToList();
            
            conn.Close();
            conn.Dispose();
            return Ok(result);
        }


        [Route("Operations/Payin-codes/Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["SidebarLocation"] = "Pay-in Codes";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        [Route("Operations/Payin-code/Generate")]
        public async Task<IActionResult> Generate(PayinCodeGenerateInputDto input)
        {
            var current_user = await currentUser.CurrentUser();


            var conn = await connectionFactory.CreateConnectionAsync();
            
            //searching transaction Number
            var transaction = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE CONCAT(TRANSACTION_NUMBER ,ID) = @id
            ", new
            {
                id = input.ReferenceNumber
            }).FirstOrDefault();

            if (transaction == null)
            {
                return BadRequest("OR Code/Number does'nt exist.");
            }

            if (transaction.TRANSACTION_TYPE != TransactionTypeEnum.PURCHASE.ToString())
            {
                return BadRequest("OR Code/Number is not valid.");
            }
            
            if (transaction.VOID_STATUS == true)
            {
                return BadRequest("OR Code/Number is already voided.");
            }
            
            if (transaction.IS_CODE_GENERATED == true)
            {
                return BadRequest("OR Code/Number is already use to generate code.");
            }
            
            //searching for pos transaction products
            var posResult = conn.Query<PointSaleTransactionsEntity>(@"
                SELECT * FROM pos_transactions WHERE  TRANSACTION_REF=@tran
            ", new
            {
                tran = transaction.ID
            });
            
            //searching for packages
            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE  ID IN @ids AND IS_PACKAGE = 1
            ", new
            {
                ids = posResult.Select(x => x.PRODUCT_REF).ToList()
            });

            if (productResult.Count() == 0 || productResult == null)
            {
                return BadRequest("OR Code/Number has no packages.");
            }
            
            //query to get distrutor from transaction number
            var distributor = new DistributorsDetailsEntity
            {
                DISTRIBUTOR_ID = posResult.First().DISTRIBUTOR_REF ?? 0,
                FIRSTNAME = "SUPREME_HERBS",
                LASTNAME = "INTL"
            };

            var result = new List<PayinCodeGenerateOutputDto>();

            int totalCodes = 0;
            foreach (var prod in productResult)
            {
                var pos_temp = posResult.Where(x => x.PRODUCT_REF == prod.ID).FirstOrDefault();
                totalCodes += (pos_temp.QUANTITY);
            }
            
            for(int x = 0; x < totalCodes; x++)
            {
                string generatedCode = _keyHelper.GenerateAlphaNumeric(16).ToUpper();
                string qr_data = $"{distributor.DISTRIBUTOR_ID}|{generatedCode}|{distributor.FIRSTNAME}|{distributor.LASTNAME}";
                string qr_encrypted = encryptionHelper.Encrypt(qr_data) ;

                string qrcode_base_64_image = await _qrCodeHelper.CreateQRCode(qr_encrypted) ;

                result.Add(new PayinCodeGenerateOutputDto
                {
                    qr_code_base64 = qrcode_base_64_image,
                    payin_code = generatedCode,
                    expiration_date = DateTime.Now.AddDays(7),
                });
            }

            try
            {
                
                await unitOfWork.PayinCodesRepository.AddRangeAsync(result.Select(x => new PayinCodesEntity
                {
                    TRANSACTION_REF = Int32.Parse(input.ReferenceNumber.Substring(6)),
                    PAYIN_CODE = x.payin_code,
                    DISTRIBUTOR_REF = distributor.DISTRIBUTOR_ID,
                    CREATED_AT = DateTime.UtcNow,
                    EXPIRATION_DT = x.expiration_date,
                    CREATED_BY = current_user.id,
                    UPDATED_BY = current_user.id
                }).ToList());
                transaction.IS_CODE_GENERATED = true;
                unitOfWork.TransactionsRepository.Update(transaction);
                await unitOfWork.CommitAsync();
                
                conn.Close();
                conn.Dispose(); 
            }
            catch(Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }


            return Ok(result);
        }

        [Route("Operations/Payin-codes/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["SidebarLocation"] = "Pay-in Codes";
            return View();
        }
        
    }
}
