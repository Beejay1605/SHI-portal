using Dapper;
using Domain.DTO.Distributor.Dashboard.output;
using Domain.Entity;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Distributor.Controllers;

[Area("Distributor")]
public class DashboardController : Controller
{
    public DashboardController(IDbConnectionFactory connectionFactory, IEntityMapper mapper, IEncryptionHelper encryptionHelper, ICurrentUserService currentUser)
    {
        this.connectionFactory = connectionFactory;
        _mapper = mapper;
        _encryptionHelper = encryptionHelper;
        _currentUser = currentUser;
    }

    private readonly IDbConnectionFactory connectionFactory; 
    private readonly IEntityMapper _mapper; 
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly ICurrentUserService _currentUser;
    
    // GET
    public IActionResult Index()
    {
        ViewData["SidebarLocation"] = "Dashboard";

        
        
        return View();
    }

    [HttpGet]
    [ClaimRequirement("Distributor")]
    public async Task<IActionResult> DashboardData()
    {
        var conn = await connectionFactory.CreateConnectionAsync();
        var user = await _currentUser.CurrentUser();
        var result = new DashboardOutputDto();

        string payoutQuery = @"SELECT * FROM payout_transaction 
                                    WHERE DISTRIBUTOR_REF=@id 
                                    ORDER BY CREATED_DT DESC";
        var payoutResult = conn.Query<PayoutTransactionsEntity>(payoutQuery, new
        {
            id = user.id
        });
        result.total_earn = payoutResult.Sum(x => x.TOTAL_AMOUNT);
        
        
        
        string transactionQuery = @"SELECT 
                                        tr.* 
                                        FROM transactions as tr
                                        INNER JOIN pos_transactions as pt
                                        ON tr.`ID`=pt.`TRANSACTION_REF`
                                        WHERE tr.`TRANSACTION_TYPE`='PURCHASE'
                                        AND pt.`DISTRIBUTOR_REF`=@id";
        var transactionResult = conn.Query<TransactionsEntity>(transactionQuery, new
        {
            id = user.id
        });

        string posQuery = @"SELECT * FROM pos_transactions
                                WHERE DISTRIBUTOR_REF=@ids";
        var posResult = conn.Query<PointSaleTransactionsEntity>(posQuery, new
        {
            ids = user.id
        });

        result.transaction_history = transactionResult.Select(x =>
        {
            var z = new TransachtionHistoryOutputDto();
            z.type = "PURCHASE";
            z.created_dt = x.CREATED_DATE_UTC.ToString("MM/dd/yyyy HH:mm tt");
            var pd = posResult.Where(o =>
                o.TRANSACTION_REF == x.ID).ToList();
            z.amount = pd.Sum(s => (s.PER_UNIT_PRICE * s.QUANTITY));
            return z;
        }).ToList();

        foreach (var prt in payoutResult)
        {
            var z = new TransachtionHistoryOutputDto();
            z.type = "PAYOUT";
            z.created_dt = prt.CREATED_DT.ToString("MM/dd/yyyy HH:mm tt");
            z.amount = prt.TOTAL_AMOUNT;
            result.transaction_history.Add(z);
        }

        var prodIds = posResult.Select(x => x.PRODUCT_REF).ToList();
        
        string productsQuery = @"SELECT * FROM products WHERE
                           ID IN @ids";
        var productsResult = conn.Query<ProductsEntity>(productsQuery, new
        {
            ids = prodIds
        });

        result.purchase_history = posResult.Select(x =>
        {
            var t = _mapper.posTransactionMapper.Map(x);
            var pr = productsResult.Where(p => p.ID == x.PRODUCT_REF).FirstOrDefault();
            if (pr != null)
            {
                t.product_details = _mapper.productMapper.Map(pr);
            }
            return t;
        }).ToList();

        result.payout_history = payoutResult.Select(x =>
        {
            return _mapper.payoutTransactionsMapper.Map(x);
        }).ToList();
        
        return Ok(result);
    }
}