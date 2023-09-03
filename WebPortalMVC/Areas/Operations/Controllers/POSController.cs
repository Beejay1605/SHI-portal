using System.Net;
using Dapper;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.POS.Input;
using Domain.DTO.Operations.POS.Output;
using Domain.DTO.Operations.Products.Output;
using Domain.Entity;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class POSController : Controller
    {
        public POSController(IDbConnectionFactory dbConnectionFactory, IUnitOfWork unitOfWork, ICurrentUserService currentUser, ILogger<POSController> logger, IFileHelper file, IKeyHelper keyHelper, IEntityMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _logger = logger;
            _file = file;
            _keyHelper = keyHelper;
            _mapper = mapper;
        }

        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<POSController> _logger;
        private readonly IFileHelper _file;
        private readonly IKeyHelper _keyHelper;
        private readonly IEntityMapper _mapper;
        
        public async Task<IActionResult> Index()
        {
            ViewData["SidebarLocation"] = "POS";
            
            
            return View();
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> GetAllProducts(string search)
        {
            search = search ?? string.Empty;
            GetAllProductsOutputDto result = new GetAllProductsOutputDto();
            var conn = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
        
                string queryString = @"SELECT * FROM products WHERE 
                    STATUS=@STATUS 
                       AND PRODUCT_NAME like @search
                    ORDER BY PRODUCT_NAME ASC";

                var productResult = conn.Query<ProductsEntity>(queryString, new
                {
                    STATUS = ProductStatusEnum.ACTIVE.ToString(),
                    search = $"%{search.Replace(" ","%").Replace(" ","%")}%"
                })?.ToList();

                var ids = productResult.Select(x => x.ID).ToList();

                var prodImageResult = conn.Query<ProductImagesEntity>(@"
                    SELECT * FROM product_images WHERE PRODUCT_REF IN @IDS", new
                {
                    IDS = ids
                })?.ToList();
                
                foreach (var pr in productResult)
                {
                    ProductBaseDto pbd = new ProductBaseDto();
                    pbd.ident = pr.ID;
                    pbd.p_code = pr.PRODUCT_CODE;
                    pbd.p_name = pr.PRODUCT_NAME;
                    pbd.p_price = pr.SRP_PRICE;
                    pbd.membership_price = pr.MEMBERS_PRICE;
                    pbd.non_membership_discounted_price = pr.NON_MEMBERS_DISCOUNTED_PRICE;
                    pbd.profit = pr.COMPANY_PROFIT;
                    pbd.total_payout = pr.PAYOUT_TOTAL;
                    pbd.p_category = pr.CATEGORY;
                    //pbd.p_cover_photo = _file.GetImageUrl(pbd.)

                    pbd.p_mini_desc = pr.MINI_DESCRIPTION;
                    pbd.p_status = pr.STATUS;

                    var pictures = prodImageResult.Where(
                        x => x.PRODUCT_REF == pr.ID).FirstOrDefault();
                    if (pictures != null)
                    {
                        pbd.picture = _file.GetImageUrl(pictures.PHOTO_PATH);
                    }
                    
                    result.Products.Add(pbd);
                }
 
                conn.Close();
                conn.Dispose(); 
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose(); 
            }
            return Ok(result);
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> Distributor(int id)
        {
            var conn = await _dbConnectionFactory.CreateConnectionAsync();
            var distributor = new DistributorsDetailsDto();
            try
            {
                if (id == 0)
                {
                    return BadRequest("Distributor does'nt exist");
                }
                
                var result = conn.Query<DistributorsDetailsEntity>(@"
                    SELECT * FROM distributors_details WHERE 
                             DISTRIBUTOR_ID = @id
                ", new
                {
                    id = id
                })?.FirstOrDefault();

                if (result == null)
                {
                    return BadRequest("Distributor does'nt exist");
                }

                distributor = new DistributorsDetailsDto
                {
                    ident = result.DISTRIBUTOR_ID,
                    user_ref_ident = result.USER_CRED_REF,
                    first_name = result.FIRSTNAME,
                    last_name = result.LASTNAME,
                    middle_name = result.MIDDLENAME,
                    suffix_name = result.SUFFIX,
                    ui_id = result.DISTRIBUTOR_ID
                };
                
                if (result.PICTURE_PATH != null)
                {
                    distributor.user_picture_base_64 = _file.GetImageUrl(result.PICTURE_PATH);
                }
                
                
                conn.Close();
                conn.Dispose(); 
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose(); 
            }
            
            return Ok(distributor);
        }


        [HttpPost]
        [ClaimRequirement("Operations")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseAction(PurchaseInputDto input)
        {
            var conn = await _dbConnectionFactory.CreateConnectionAsync();
            var ids = input.prod_quantity.Select(x => x.productId).ToList();
            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE  ID IN @ids
            ", new
            {
                ids = ids
            }).ToList();

            var packages = productResult.Where(x => x.IS_PACKAGE == true);
            if (packages.Count() > 0)
            {
                if (input.distributor_id == 0 )
                {
                    return BadRequest("You try to purchase package. Distributor is Required.");
                }
            }

            var inventoryResult = conn.Query<InventoryEntity>(@"
                SELECT * FROM inventory WHERE PRODUCT_REF IN @ids
            ", new
            {
                ids = ids
            });
            
            // validation for product stocks
            string message = string.Empty;
            foreach (var prod in productResult)
            {
                var inventory = inventoryResult.Where(x => x.PRODUCT_REF == prod.ID);
                int quantity = input.prod_quantity.Find(x => x.productId == prod.ID.ToString()).quantity;
                int total_stocks = inventory.Sum(x => x.QUANTITY);
                if (total_stocks < quantity)
                {
                    message += $"• <b>{prod.PRODUCT_NAME}</b> is stocks is not enough. \n";
                } 
            }

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest("Please check the inventory. \n \n<br/><br/>"+message);
            }
            
            try
            {
                var user = await _currentUser.CurrentUser();
                var transactions = new TransactionsEntity
                {
                    TRANSACTION_NUMBER = _keyHelper.GenerateAlphaNumeric(6).ToUpper(),
                    TRANSACTION_TYPE = TransactionTypeEnum.PURCHASE.ToString(),
                    VOID_STATUS = false,
                    CREATED_DATE_UTC = DateTime.UtcNow,
                    CREATED_BY = user.id
                };
                await _unitOfWork.TransactionsRepository.AddAsync(transactions);
                await _unitOfWork.CommitAsync();

                var inventoryEntity = new List<InventoryEntity>();
                var posTransactions = new List<PointSaleTransactionsEntity>();
                foreach (var prod in productResult)
                {
                    int quantity = input.prod_quantity.Find(x => x.productId == prod.ID.ToString()).quantity;
                    inventoryEntity.Add(new InventoryEntity
                    {
                        PRODUCT_REF = prod.ID,
                        QUANTITY = (-quantity),
                        ACTION = InventoryActionEnum.PURCHASE.ToString(),
                        VOID_STATUS = false,
                        DOC_PATH = string.Empty,
                        CREATE_DATE_UTC = DateTime.UtcNow,
                        TRANSACTION_REF = transactions.ID,
                        CREATED_BY = user.id
                    });

                    var pos = new PointSaleTransactionsEntity
                    {
                        TRANSACTION_REF = transactions.ID,
                        PRODUCT_REF = prod.ID,
                        SRP_PRICE = prod.SRP_PRICE,
                        COMPANY_PROFIT = prod.COMPANY_PROFIT,
                        PAYOUT_TOTAL = prod.PAYOUT_TOTAL,
                        QUANTITY = quantity,
                        PAYMENT_TYPE = PaymentTypeEnum.CASH.ToString(),
                        BRANCH = "MAIN"
                    };
                    if (input.distributor_id != 0)
                    {
                        pos.DISTRIBUTOR_REF = input.distributor_id;
                        pos.PER_UNIT_PRICE = prod.MEMBERS_PRICE ?? prod.SRP_PRICE;
                    }
                    else
                    {
                        pos.PER_UNIT_PRICE = prod.NON_MEMBERS_DISCOUNTED_PRICE ?? prod.SRP_PRICE;
                    }
                    
                    pos.MEMBERS_PRICE = prod.MEMBERS_PRICE;
                    pos.NON_MEMBERS_DISCOUNTED_PRICE = prod.NON_MEMBERS_DISCOUNTED_PRICE;
                    posTransactions.Add(pos);
                    
                    // inserting earnings for unilevel for non packages
                    // if (prod.IS_PACKAGE == false)
                    // {
                    //     DateTime currentDate = DateTime.Today;
                    //     DateTime nextMonth = currentDate.AddMonths(1);
                    //     DateTime firstDateOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                    //     
                    //     await _unitOfWork.EarningsUniLevel.AddAsync(new EarningsUniLevelEntity
                    //     {
                    //         DISTRIBUTOR_REF = input.distributor_id,
                    //         TRANSACTION_REF = transactions.ID,
                    //         PRODUCT_REF = prod.ID,
                    //         AMOUNT = ((prod.SRP_PRICE * (decimal)quantity) * (decimal)0.02),
                    //         CREATED_DATE = DateTime.UtcNow,
                    //         AVAILABILITY_DATE = firstDateOfNextMonth,
                    //         IS_ENCASH = false
                    //     });
                    // }
                }

                await _unitOfWork.PointSaleTransactionsRepository.AddRangeAsync(posTransactions);
                await _unitOfWork.InventoryRepository.AddRangeAsync(inventoryEntity);
                await _unitOfWork.CommitAsync();
                
                
                conn.Close();
                conn.Dispose();
                return Ok(transactions.ID);
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await _unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            } 
        }

        public async Task<IActionResult> ReceiptPurchase(int id)
        {
            ViewData["SidebarLocation"] = "POS";
            var conn = await _dbConnectionFactory.CreateConnectionAsync();

            var transaction = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE  ID = @id
            ", new
            {
                id = id
            }).FirstOrDefault();

            if (transaction == null)
            {
                return RedirectToAction("Index");
            }
            
            
            
            var posResult = conn.Query<PointSaleTransactionsEntity>(@"
                SELECT * FROM pos_transactions WHERE TRANSACTION_REF=@tranid
            ", new
            {
                tranid = id
            }).ToList();

            var prodIds = posResult.Select(x => x.PRODUCT_REF).ToList();

            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE  ID IN @ids
            ", new
            {
                ids = prodIds
            }).ToList();

            var result = new ReceiptPurchaseOutputDto();
            result.transaction = _mapper.transactionMapper.Map(transaction);
            
            
            var personnelResult = conn.Query<OperationsDetailsEntity>(@"
                SELECT * FROM operations_details WHERE  ID = @id
            ", new
            {
                id = transaction.CREATED_BY
            }).FirstOrDefault();
            result.transaction.created_by_name = $"{personnelResult.FIRST_NAME} {personnelResult.LAST_NAME}";
            
            foreach (var pos in posResult)
            {
                var product = productResult.Where(x => x.ID == pos.PRODUCT_REF).FirstOrDefault();
                if (product != null)
                {
                    var temp = _mapper.posTransactionMapper.Map(pos);
                    temp.product_details = _mapper.productMapper.Map(product);
                    result.posTransactions.Add(temp);
                }
            }

            
            if (posResult.First().DISTRIBUTOR_REF != null)
            {
                var distResult = conn.Query<DistributorsDetailsEntity>(@"
                    SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID = @id
                ", new
                {
                    id = posResult.First().DISTRIBUTOR_REF
                }).FirstOrDefault();
                result.distributor_details = _mapper.distributorDetailsMap.Map(distResult);
            }

            
            
            conn.Close();
            conn.Dispose();
            return View(result);
        }

        public async Task<IActionResult> Void(string? id)
        {
            ViewData["SidebarLocation"] = "POS";
            ViewBag.ID = id;
            if (id == null)
            {
                ViewBag.MESSAGE = "Transaction ID doesn't exist.";
                return View();
            }
            ViewBag.MESSAGE = "";
            
            var conn = await _dbConnectionFactory.CreateConnectionAsync();

            var transactions = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE CONCAT(TRANSACTION_NUMBER,ID) = @tranid OR ID = @tranid
            ", new
            {
                tranid = id
            }).FirstOrDefault();

            if (transactions == null)
            { 
                return View();
            }

            var result = new VoidOutputDto();
            result.transaction = _mapper.transactionMapper.Map(transactions);
            
            var posResult = conn.Query<PointSaleTransactionsEntity>(@"
                SELECT * FROM pos_transactions WHERE TRANSACTION_REF=@tranid
            ", new
            {
                tranid = transactions.ID
            }).ToList(); 
            var prodIds = posResult.Select(x => x.PRODUCT_REF).ToList();
            
            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE  ID IN @ids
            ", new
            {
                ids = prodIds
            }).ToList(); 
            
            var personnelResult = conn.Query<OperationsDetailsEntity>(@"
                SELECT * FROM operations_details WHERE  ID = @id
            ", new
            {
                id = transactions.CREATED_BY
            }).FirstOrDefault();
            result.transaction.created_by_name = $"{personnelResult.FIRST_NAME} {personnelResult.LAST_NAME}";
            
            foreach (var pos in posResult)
            {
                var product = productResult.Where(x => x.ID == pos.PRODUCT_REF).FirstOrDefault();
                if (product != null)
                {
                    var temp = _mapper.posTransactionMapper.Map(pos);
                    temp.product_details = _mapper.productMapper.Map(product);
                    result.pos_tran.Add(temp);
                }
            }
            
            
            if (posResult.First().DISTRIBUTOR_REF != null)
            {
                var distResult = conn.Query<DistributorsDetailsEntity>(@"
                    SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID = @id
                ", new
                {
                    id = posResult.First().DISTRIBUTOR_REF
                }).FirstOrDefault();
                result.distributors_details = _mapper.distributorDetailsMap.Map(distResult);
            }
            
            
            conn.Close();
            conn.Dispose();
            return View(result);
        }

        [HttpPost]
        [ClaimRequirement("Operations")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoidTransactionAction(string id)
        {
            ViewData["SidebarLocation"] = "POS";
            var conn = await _dbConnectionFactory.CreateConnectionAsync();
            
            var transactions = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE CONCAT(TRANSACTION_NUMBER,ID) = @tranid OR ID = @tranid
            ", new
            {
                tranid = id
            }).FirstOrDefault();

            if (transactions.VOID_STATUS == true)
            {
                return BadRequest("This transaction number is already voided.");
            }

            if (transactions.IS_ENCODED_UNILEVEL == true)
            {
                return BadRequest("This transaction number is already encoded unilevel points.");
            }

            var dt = transactions.CREATED_DATE_UTC.AddDays(1);
            if (transactions.CREATED_DATE_UTC >= dt)
            {
                return BadRequest("Transactions within 24 hours are allowed to void.");
            }
            
            //check if there is code that already use
            var payinCode = conn.Query<PayinCodesEntity>(@"
                SELECT * FROM payin_codes WHERE  TRANSACTION_REF = @TRANID
            ", new
            {
                TRANID = transactions.ID
            });

            if (payinCode.Where(x => x.IS_USED == true).FirstOrDefault() != null)
            {
                return BadRequest("This transaction number has already generated code that used to encode accounts.");
            }

            var inventoryResult = conn.Query<InventoryEntity>(@"
                SELECT * FROM inventory WHERE TRANSACTION_REF = @id
            ", new
            {
                id = transactions.ID
            }).ToList();

            try
            {
                var user = await _currentUser.CurrentUser();
                transactions.VOID_STATUS = true;
                _unitOfWork.TransactionsRepository.Update(transactions);
                await _unitOfWork.VoidInventoryRepository.AddRangeAsync(inventoryResult.Select(x =>
                { 
                    
                    var re =  _mapper.inventoryToVoidMapper.Map(x);
                    re.VOID_STATUS = true;
                    re.VOIDED_BY = user.id;
                    return re;
                }));
                _unitOfWork.PayinCodesRepository.DeleteList(payinCode); 
                _unitOfWork.InventoryRepository.DeleteList(inventoryResult.Select(x => new InventoryEntity
                {
                    ID = x.ID
                }));
                
                await _unitOfWork.CommitAsync();
                
                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                await _unitOfWork.RollbackAsync();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
            
        }
    }
}
