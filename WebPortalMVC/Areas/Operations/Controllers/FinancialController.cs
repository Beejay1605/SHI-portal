using System.Net;
using Dapper;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Financials.input;
using Domain.DTO.Operations.Financials.output;
using Domain.Entity;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Operations.Controllers;

[Area("Operations")]
public class FinancialController : Controller
{
    public FinancialController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IEntityMapper mapper, IFileHelper fileHelper)
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
    
    
     
    public async Task<IActionResult> Index()
    {
        ViewData["SidebarLocation"] = "Financial";
        var conn = await connectionFactory.CreateConnectionAsync();

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
                WHERE (ep.`LEFT_BIN_ID` IS NOT NULL AND ep.`RIGHT_BIN_ID` IS NOT NULL)
                ORDER BY CREATED_DT DESC LIMIT 200";
        var pairingResult = conn.Query<EarningsPairingEntity>(pairingQuery);

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
            ORDER BY CREATED_DT DESC LIMIT 200";
        var referalResult = conn.Query<EarningsReferalEntity>(referalQuery);
        
        string unilevelQuery = @"SELECT * FROM earnings_uni_level
            ORDER BY  CREATED_DATE DESC LIMIT 200";
        var uniLevelResult = conn.Query<EarningsUniLevelEntity>(unilevelQuery);

        string payoutQuery = @"SELECT * FROM payout_transaction ORDER BY CREATED_DT DESC LIMIT 250";
        var payoutResult = conn.Query<PayoutTransactionsEntity>(payoutQuery).ToList();
        
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
        return View(result);
    }
    
    public async Task<IActionResult> Search(int id)
    {
        ViewData["SidebarLocation"] = "Financial";
        var conn = await connectionFactory.CreateConnectionAsync();

        int distId = id;
        ViewBag.ID = id;
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
        return View("Index",result);
    }

    public async Task<IActionResult> Payout(int? id)
    {
        ViewData["SidebarLocation"] = "Financial";

        int distId = id ?? 0;
        ViewBag.ID = distId;
        
        var result = new FinancialOutputDto();
        result.details = new DistributorsDetailsDto();
        if (id == null)
        { 
            return View(result);
        }
        var conn = await connectionFactory.CreateConnectionAsync();

        string pairingQuery = @"SELECT ep.`ID`, 
                bt1.`DISTRIBUTOR_REF` as LEFT_BIN_ID, 
                bt2.`DISTRIBUTOR_REF` as RIGHT_BIN_ID ,
                ep.`BENEF_BIN_ID`,
                ep.`BENEF_DIST_ID`,
                ep.`AMOUNT`,
                ep.`IS_ENCASH`,
                ep.`LEVEL`,
                ep.`CREATED_DT` ,
                ep.`AMOUNT`
                FROM earnings_pairing as ep
                LEFT JOIN binary_tree as bt1 ON ep.`LEFT_BIN_ID` = bt1.`ID`
                LEFT JOIN binary_tree as bt2 ON ep.`RIGHT_BIN_ID` = bt2.`ID`
                WHERE ep.`BENEF_DIST_ID`=@distId 
                AND ep.`IS_ENCASH`=0 
                AND (ep.`LEFT_BIN_ID` IS NOT NULL AND ep.`RIGHT_BIN_ID` IS NOT NULL)
                ORDER BY CREATED_DT DESC LIMIT 200";
        var pairingResult = conn.Query<EarningsPairingEntity>(pairingQuery, new
        {
            distId = distId
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
            WHERE ef.`BENEF_DISTRIBUTOR_REF`=@distId 
                AND ef.`IS_ENCASH`=0 
            ORDER BY CREATED_DT DESC LIMIT 200";
        var referalResult = conn.Query<EarningsReferalEntity>(referalQuery, new
        {
            distId = distId
        });
        
        string unilevelQuery = @"SELECT * FROM earnings_uni_level
            WHERE DISTRIBUTOR_REF=@distId
                AND `IS_ENCASH`=0 
            ORDER BY  CREATED_DATE DESC LIMIT 200";
        var uniLevelResult = conn.Query<EarningsUniLevelEntity>(unilevelQuery, new
        {
            distId = distId
        });

        var ids = pairingResult.Select(x => x.BENEF_DIST_ID).ToList();
        
        ids.AddRange(pairingResult.Select(x => (x.RIGHT_BIN_ID ?? 0)).ToList());
        ids.AddRange(pairingResult.Select(x => (x.LEFT_BIN_ID ?? 0)).ToList());
        
        ids.AddRange(referalResult.Select(x => x.BENEF_DISTRIBUTOR_REF).ToList());
        ids.AddRange(referalResult.Select(x => x.FROM_BINARY_REF).ToList());
        
        ids.AddRange(uniLevelResult.Select(x => x.DISTRIBUTOR_REF).ToList());

        ids = ids.Distinct().ToList();

        string distQuery = @"SELECT * FROM distributors_details
            WHERE DISTRIBUTOR_ID IN @distId";
        var distResult = conn.Query<DistributorsDetailsEntity>(distQuery, new
        {
            distId = ids
        }).ToList();
        
        
        
        
        
        if (distResult.FirstOrDefault() != null)
        {
            result.details = _mapper.distributorDetailsMap.Map(distResult.FirstOrDefault());
        }

        if (result.details.user_picture_base_64 != null)
        {
            result.details.user_picture_base_64 = _fileHelper.GetImageUrl(result.details.user_picture_base_64);
        }
        
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
        return View(result);
    }
    
    [HttpPost]
    [ClaimRequirement("Operations")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayout(FinancialInputDto input)
    {
        ViewData["SidebarLocation"] = "Financial";

        int distId = input.distributor_ident;
        
        if (input.distributor_ident == null)
        { 
            return BadRequest();
        }
        var conn = await connectionFactory.CreateConnectionAsync();

        
        string pairingQuery = @"SELECT ep.*
                FROM earnings_pairing as ep
                WHERE ep.`BENEF_DIST_ID`=@distId 
                AND ep.`IS_ENCASH`=0 
                AND (ep.`LEFT_BIN_ID` IS NOT NULL AND ep.`RIGHT_BIN_ID` IS NOT NULL)
                ORDER BY CREATED_DT DESC LIMIT 200";
        var pairingResult = conn.Query<EarningsPairingEntity>(pairingQuery, new
        {
            distId = distId
        }).ToList();

        string referalQuery = @"SELECT ef.*
            FROM earnings_referal as ef
            WHERE ef.`BENEF_DISTRIBUTOR_REF`=@distId 
                AND ef.`IS_ENCASH`=0 
            ORDER BY CREATED_DT DESC LIMIT 200";
        var referalResult = conn.Query<EarningsReferalEntity>(referalQuery, new
        {
            distId = distId
        }).ToList();
        
        string unilevelQuery = @"SELECT * FROM earnings_uni_level
            WHERE DISTRIBUTOR_REF=@distId
                AND `IS_ENCASH`=0 
            ORDER BY  CREATED_DATE DESC LIMIT 200";
        var uniLevelResult = conn.Query<EarningsUniLevelEntity>(unilevelQuery, new
        {
            distId = distId
        }).ToList();

        
        decimal totalAmount = 0;
        totalAmount += pairingResult.Sum(x => x.AMOUNT);
        totalAmount += referalResult.Sum(x => x.AMOUNT);
        totalAmount += uniLevelResult.Sum(x => x.AMOUNT);

        if (totalAmount <= 0)
        {
            return BadRequest("You dont have amount to payout");
        }
        
        try
        {
            var user = await currentUserService.CurrentUser();
            
            var payout = new PayoutTransactionsEntity
            {
                DISTRIBUTOR_REF = distId,
                TOTAL_AMOUNT = totalAmount,
                CREATED_DT = DateTime.UtcNow,
                CREATED_BY = user.id
            };
            await _unitOfWork.PayoutTransactions.AddAsync(payout);
            await _unitOfWork.CommitAsync();
            
            foreach (var x in pairingResult)
            {
                x.IS_ENCASH = true;
                x.PAYOUT_TRAN_REF = payout.ID;
                _unitOfWork.EarningsPairing.Update(x);
            }
            foreach (var x in referalResult)
            {
                x.IS_ENCASH = true;
                x.PAYOUT_TRAN_REF = payout.ID;
                _unitOfWork.EarningsReferal.Update(x);
            }
            foreach (var x in uniLevelResult)
            {
                x.IS_ENCASH = true;
                x.PAYOUT_TRAN_REF = payout.ID;
                _unitOfWork.EarningsUniLevel.Update(x);
            }
            await _unitOfWork.CommitAsync();
            
            return Ok();
        }
        catch (Exception ex)
        {
            conn.Close();
            conn.Dispose();
            await _unitOfWork.CommitAsync();
            throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
        }
         
    }
}