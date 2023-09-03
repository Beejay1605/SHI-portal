using System.Net;
using Dapper;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Inventory.input;
using Domain.DTO.Operations.Inventory.output;
using Domain.Entity;
using Manager.Commons.Const;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class InventoryController : Controller
    {
        public InventoryController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileHelper fileHelper, IEntityMapper mapper)
        {
            this.connectionFactory = connectionFactory;
            _unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.fileHelper = fileHelper;
            _mapper = mapper;
        }

        private readonly IDbConnectionFactory connectionFactory;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ICurrentUserService currentUserService;
        private readonly IFileHelper fileHelper;
        private readonly IEntityMapper _mapper;

        public IActionResult Index()
        {
            ViewData["SidebarLocation"] = "Inventory";
            return View();
        }


        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> GetProductInventory(string search)
        {
            search = search ?? "";
            
            var result = new List<InventoryOutputDto>(); 
            var conn = await connectionFactory.CreateConnectionAsync(); 
            var products = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE STATUS='ACTIVE' AND
                (PRODUCT_NAME LIKE @src OR  PRODUCT_CODE LIKE @src)
            ", new
            {
                src = "%" + search.Replace(" ","%").Replace(" ","%") + "%"
            });

            var inventory = conn.Query<InventoryEntity>(@"
                SELECT * FROM inventory WHERE VOID_STATUS='0'
            ");

            result = products.Select(x => new InventoryOutputDto
            {
                product_details = _mapper.productMapper.Map(x),
                inventory = inventory.Where(i => i.PRODUCT_REF == x.ID)
                                .Select(n => _mapper.inventoryMapper.Map(n)).ToList()
            }).ToList();
             
            conn.Close();
            conn.Dispose();
            return Ok(result);
        }

        public async Task<IActionResult> Create(string code)
        {
            var result = new CreateInventoryOutputDto(); 
            var conn = await connectionFactory.CreateConnectionAsync(); 
            
            
            
            var productResult = conn.Query<ProductsEntity>(@"
                        SELECT * FROM products WHERE PRODUCT_CODE=@pcode
                    ", new
            {
                pcode = code
            }).FirstOrDefault();

            if (productResult == null)
            {
                return RedirectToAction("Index");
            }

            var inventory = conn.Query<InventoryEntity>(@"
                SELECT * FROM inventory WHERE PRODUCT_REF=@pref
            ", new
            {
                pref = productResult.ID
            }).ToList();

            result.inventory = inventory.Select(n => _mapper.inventoryMapper.Map(n)).ToList();
            result.product_details = _mapper.productMapper.Map(productResult); 
        
            conn.Close();
            conn.Dispose();
            return View(result);
        }
        
        public async Task<IActionResult> RemoveStocks(string code)
        {
            var result = new CreateInventoryOutputDto(); 
            var conn = await connectionFactory.CreateConnectionAsync(); 
            
            var productResult = conn.Query<ProductsEntity>(@"
                        SELECT * FROM products WHERE PRODUCT_CODE=@pcode
                    ", new
            {
                pcode = code
            }).FirstOrDefault();

            if (productResult == null)
            {
                return RedirectToAction("Index");
            }

            var inventory = conn.Query<InventoryEntity>(@"
                SELECT * FROM inventory WHERE PRODUCT_REF=@pref
            ", new
            {
                pref = productResult.ID
            }).ToList();

            result.inventory = inventory.Select(n => _mapper.inventoryMapper.Map(n)).ToList();
            result.product_details = _mapper.productMapper.Map(productResult); 
        
            conn.Close();
            conn.Dispose();
            return View(result);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> CreateAction(CreateInventoryInputDto input)
        {
            if (input.product_id == 0)
            {
                return BadRequest("An error occured. please refresh the page");
            }
            else if (input.quantity <= 0)
            {
                return BadRequest("Inventory Stocks to be added is required");
            }
            var user = await currentUserService.CurrentUser();
            var conn = await connectionFactory.CreateConnectionAsync();

            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE ID=@id
            ", new
            {
                id = input.product_id
            }).FirstOrDefault();

            if (productResult == null)
            {
                return BadRequest("An error occured. please refresh the page");
            }

            try
            {
                var inventory = new InventoryEntity
                {
                    PRODUCT_REF = productResult.ID,
                    QUANTITY = input.quantity,
                    VOID_STATUS = false,
                    ACTION = InventoryActionEnum.ADD_STOCKS.ToString(),
                    CREATE_DATE_UTC = DateTime.UtcNow,
                    CREATED_BY = user.id
                };
            
                string folderNameDesc = Guid.NewGuid().ToString();
                var _desc_result = await fileHelper.saveTextFile(input.doc_body,
                    FilesConst.INVENTORY_DOCUMENTATION_PATH, folderNameDesc + ".txt");
            
                if (_desc_result == true)
                {
                    inventory.DOC_PATH = FilesConst.INVENTORY_DOCUMENTATION_PATH + folderNameDesc + ".txt";
                }
            
                await _unitOfWork.InventoryRepository.AddAsync(inventory);
                await _unitOfWork.CommitAsync();
                
                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            { 
                conn.Close();
                conn.Dispose();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> RemoveAction(CreateInventoryInputDto input)
        {
            if (input.product_id == 0)
            {
                return BadRequest("An error occured. please refresh the page");
            }
            else if (input.quantity <= 0)
            {
                return BadRequest("Inventory Stocks to be added is required");
            }
            var user = await currentUserService.CurrentUser();
            var conn = await connectionFactory.CreateConnectionAsync();

            var productResult = conn.Query<ProductsEntity>(@"
                SELECT * FROM products WHERE ID=@id
            ", new
            {
                id = input.product_id
            }).FirstOrDefault();

            if (productResult == null)
            {
                return BadRequest("An error occured. please refresh the page");
            }

            try
            {
                var inventory = new InventoryEntity
                {
                    PRODUCT_REF = productResult.ID,
                    QUANTITY = (-input.quantity),
                    VOID_STATUS = false,
                    ACTION = InventoryActionEnum.REMOVE_STOCKS.ToString(),
                    CREATE_DATE_UTC = DateTime.UtcNow,
                    CREATED_BY = user.id
                };

                if (inventory.QUANTITY > 0)
                {
                    inventory.QUANTITY = (-inventory.QUANTITY);
                }
            
                string folderNameDesc = Guid.NewGuid().ToString();
                var _desc_result = await fileHelper.saveTextFile(input.doc_body,
                    FilesConst.INVENTORY_DOCUMENTATION_PATH, folderNameDesc + ".txt");
            
                if (_desc_result == true)
                {
                    inventory.DOC_PATH = FilesConst.INVENTORY_DOCUMENTATION_PATH + folderNameDesc + ".txt";
                }
            
                await _unitOfWork.InventoryRepository.AddAsync(inventory);
                await _unitOfWork.CommitAsync();
                
                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            { 
                conn.Close();
                conn.Dispose();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
            
        }
         
    }
}