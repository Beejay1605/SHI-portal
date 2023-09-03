using Dapper;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Financials.output;
using Domain.Entity;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Distributor.Controllers;

[Area("Distributor")]
public class EarningsController : Controller
{
    public EarningsController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IEntityMapper mapper, IFileHelper fileHelper)
    {
        this.connectionFactory = connectionFactory;
        _unitOfWork = unitOfWork;
        this.currentUserService = currentUserService;
        _mapper = mapper;
        _fileHelper = fileHelper;
    }

    private readonly IDbConnectionFactory connectionFactory;
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ICurrentUserService currentUserService;
    private readonly IEntityMapper _mapper;
    private readonly IFileHelper _fileHelper;
    
    
    // GET
    public async Task<IActionResult> Index()
    {
        ViewData["SidebarLocation"] = "Earnings";
        
        return View();
    }

    [HttpGet]
    [ClaimRequirement("Distributor")]
    public async Task<IActionResult> MyEarnings()
    {
        var user = await currentUserService.CurrentUser();
        var conn = await connectionFactory.CreateConnectionAsync();
        int distId = user.id;
        ViewBag.ID = distId;
        string pairingQuery = @"SELECT ep.`ID`, 
                bt1.`DISTRIBUTOR_REF` as LEFT_BIN_ID, 
                bt2.`DISTRIBUTOR_REF` as RIGHT_BIN_ID ,
                ep.`BENEF_BIN_ID`,
                ep.`BENEF_DIST_ID`,
                ep.`IS_ENCASH`,
                ep.`LEVEL`,
                ep.`CREATED_DT` ,
                ep.`AMOUNT`
                FROM earnings_pairing as ep
                LEFT JOIN binary_tree as bt1 ON ep.`LEFT_BIN_ID` = bt1.`ID`
                LEFT JOIN binary_tree as bt2 ON ep.`RIGHT_BIN_ID` = bt2.`ID`
                WHERE ep.`BENEF_DIST_ID`=@id
                AND (ep.`LEFT_BIN_ID` IS NOT NULL AND ep.`RIGHT_BIN_ID` IS NOT NULL)
                ORDER BY CREATED_DT DESC LIMIT 200";
        var pairingResult = conn.Query<EarningsPairingEntity>(pairingQuery,new
        {
            id = distId
        });

        string referalQuery = @"SELECT ef.`BENEF_BINARY_REF`, 
            bt1.`DISTRIBUTOR_REF` as FROM_BINARY_REF,  
            ef.`BENEF_DISTRIBUTOR_REF`,
            ef.`BONUS_TYPE`,
            ef.`CREATED_DT`,
            ef.`IS_ENCASH`,
            ef.`ENCASH_REQUEST_BY`,
            ef.`AMOUNT`
            FROM earnings_referal as ef
            LEFT JOIN binary_tree as bt1 ON ef.`FROM_BINARY_REF` = bt1.`ID` 
            WHERE ef.`BENEF_DISTRIBUTOR_REF`=@id
            ORDER BY CREATED_DT DESC LIMIT 200";
        var referalResult = conn.Query<EarningsReferalEntity>(referalQuery, new
        {
            id = distId
        });
        
        string unilevelQuery = @"SELECT * FROM earnings_uni_level
            WHERE `DISTRIBUTOR_REF`=@id
            ORDER BY  CREATED_DATE DESC LIMIT 200";
        var uniLevelResult = conn.Query<EarningsUniLevelEntity>(unilevelQuery, new
        {
            id = distId
        });

        string payoutQuery = @"SELECT * FROM payout_transaction 
         WHERE DISTRIBUTOR_REF=@id
         ORDER BY CREATED_DT DESC LIMIT 250";
        var payoutResult = conn.Query<PayoutTransactionsEntity>(payoutQuery, new
        {
            id = distId
        }).ToList();
        
        var ids = pairingResult.Select(x => x.BENEF_DIST_ID).ToList();
        
        ids.AddRange(pairingResult.Select(x => (x.RIGHT_BIN_ID ?? 0)).ToList());
        ids.AddRange(pairingResult.Select(x => (x.LEFT_BIN_ID ?? 0)).ToList());
        
        ids.AddRange(referalResult.Select(x => x.BENEF_DISTRIBUTOR_REF).ToList());
        ids.AddRange(referalResult.Select(x => x.FROM_BINARY_REF).ToList());
        
        ids.AddRange(uniLevelResult.Select(x => x.DISTRIBUTOR_REF).ToList());
        ids.AddRange(payoutResult.Select(x => x.DISTRIBUTOR_REF).ToList());

        ids = ids.Distinct().ToList();

        string distQuery = @"SELECT * FROM distributors_details
            WHERE DISTRIBUTOR_ID IN @distId";
        var distResult = conn.Query<DistributorsDetailsEntity>(distQuery, new
        {
            distId = ids
        }).ToList();
        
        
        var result = new FinancialOutputDto();
        result.payouts = payoutResult.Select(x =>
        {
            var temp = _mapper.payoutTransactionsMapper.Map(x);
            var temp_dist = distResult.Where(d => d.DISTRIBUTOR_ID == x.DISTRIBUTOR_REF).First();
            temp.distributors_details = _mapper.distributorDetailsMap.Map(temp_dist);
            return temp;
        }).ToList();

        result.pairings = pairingResult.Select(x =>
        {
            var temp = _mapper.earningsPairingMapper.Map(x);
            
            var temp_dist = distResult.Where(d => d.DISTRIBUTOR_ID == x.BENEF_DIST_ID).First();
            temp.ben_dist_details = _mapper.distributorDetailsMap.Map(temp_dist);

            if(x.RIGHT_BIN_ID != null)
            {
                var temp_right = distResult.Where(r =>
                    r.DISTRIBUTOR_ID == x.RIGHT_BIN_ID).First();
                temp.right_dist_details = _mapper.distributorDetailsMap.Map(temp_right);
            }

            if (x.LEFT_BIN_ID != null)
            {
                var temp_left = distResult.Where(r =>
                    r.DISTRIBUTOR_ID == x.LEFT_BIN_ID).First();
                temp.left_dist_details = _mapper.distributorDetailsMap.Map(temp_left);
            }
             
            return temp;
        }).ToList();

        result.referals = referalResult.Select(x =>
        {
            var temp2 = _mapper.earningsReferalMapper.Map(x);

            var temp_dist = distResult.Where(d =>
                d.DISTRIBUTOR_ID == x.BENEF_DISTRIBUTOR_REF).First();
            temp2.dist_details = _mapper.distributorDetailsMap.Map(temp_dist);
            
            var temp_from = distResult.Where(d =>
                d.DISTRIBUTOR_ID == x.FROM_BINARY_REF).First();
            temp2.from_dist_details = _mapper.distributorDetailsMap.Map(temp_from);
            
            return temp2;
        }).ToList();

        result.unilevel = uniLevelResult.Select(x =>
        {
            var temp3 = _mapper.earningsUnilevelMapper.Map(x);

            var dist_temp2 = distResult.Where(d =>
                d.DISTRIBUTOR_ID == x.DISTRIBUTOR_REF).First();
            temp3.dist_details = _mapper.distributorDetailsMap.Map(dist_temp2);
            
            return temp3;
        }).ToList();
        
        conn.Close();
        conn.Dispose();
        return Ok(result);
    }
}