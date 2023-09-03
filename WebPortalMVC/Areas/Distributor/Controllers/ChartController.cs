using System.Net;
using Dapper;
using Domain.DTO.Distributor.Chart.input;
using Domain.DTO.Operations.Team.output;
using Domain.Entity;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Distributor.Controllers;

[Area("Distributor")]
public class ChartController : Controller
{
    public ChartController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, IEntityMapper mapper, IFileHelper fileHelper, IEncryptionHelper encryptionHelper, ICurrentUserService currentUser)
    {
        this.connectionFactory = connectionFactory;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileHelper = fileHelper;
        _encryptionHelper = encryptionHelper;
        _currentUser = currentUser;
    }

    private readonly IDbConnectionFactory connectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityMapper _mapper;
    private readonly IFileHelper _fileHelper;
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly ICurrentUserService _currentUser;
    
    // GET
    public IActionResult Index(string? tp)
    {
        ViewData["SidebarLocation"] = "Chart";
        if (!string.IsNullOrEmpty(tp))
        {
            ViewBag.binData = tp;
            ViewBag.isBinary = true;
        }
        else
        {
            ViewBag.binData = "";
            ViewBag.isBinary = false;
        }
        return View();
    }
    
    
    [HttpPost]
    [ClaimRequirement("Distributor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DistributorsTeamViaBinary(TeamInputDto input) 
    {
        var user = await _currentUser.CurrentUser();
        var conn = await connectionFactory.CreateConnectionAsync();
        try
        {
            int distId = user.id;
            var result = new TeamOutputDto();
            string dt = "";
            string queryFirstBinary = @"SELECT * FROM binary_tree WHERE
                    DISTRIBUTOR_REF=@dist_id ORDER BY ID ASC";
            
            if (!string.IsNullOrEmpty(input.bin_data))
            {
                dt = _encryptionHelper.DecodeBase64String(input.bin_data);
                dt = _encryptionHelper.Decrypt(dt);
                distId = Int32.Parse(dt);
                queryFirstBinary = @"SELECT * FROM binary_tree WHERE
                    ID=@dist_id ORDER BY ID ASC";
            }
            
            var firstRes = conn.Query<BinaryTreeEntity>(queryFirstBinary, new
            {
                dist_id = distId
            }).FirstOrDefault();

            if (firstRes == null)
            {
                return Ok(result);
            }
            
            string queryString = @"SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID=@id";
            var distributor = conn.Query<DistributorsDetailsEntity>(queryString, new
            {
                id = firstRes.DISTRIBUTOR_REF
            }).FirstOrDefault();
            
            if (distributor == null)
            {
                throw new ErrorException(HttpStatusCode.InternalServerError, "", "");
            }

            if (distributor.PICTURE_PATH != null)
            {
                distributor.PICTURE_PATH = _fileHelper.GetImageUrl(distributor.PICTURE_PATH);
            }
            
            var mapResult = _mapper.distributorDetailsMap.Map(distributor);
            
            result.details = mapResult;

            result.level_list = new List<BinaryDetailsOutputDto>();

            var distributors = new List<BinaryTreeEntity>();
            var resultEmptyDownlines = new List<BinaryDetailsOutputDto>();

            if (firstRes == null)
            {
                string paramUplineDetails =
                    _encryptionHelper.Encrypt(
                        $"|||{1}");
                
                result.level_list.Add(new BinaryDetailsOutputDto
                {
                    id = 0, 
                    imageUrl = string.Empty,
                    children = new List<BinaryDetailsOutputDto>(),
                    content = $"<div class=\"node-item\" style=\"background-color: #e0e0e0;\" name=\"no-distrbutors\" dist-id=\"{paramUplineDetails}\"><img src=\"/assets/img/userimg.png\" dist-id=\"{paramUplineDetails}\"/><p dist-id=\"{paramUplineDetails}\"><span class=\"material-symbols-sharp\">add</span></p><small>N/A</small></div>"
                });
                return Ok(result);
            }
            
            
            distributors.Add(firstRes);
            distributors[0].PARENT_BINARY_REF = null;
            resultEmptyDownlines.AddRange(await binaryNoDownlinesGetter(distributors, 1));

            var prevResult = distributors;
            
            int count = 0;
            int levelReach = 1;
            do
            {
                var resultBinary = new List<BinaryTreeEntity>();
                if (prevResult.Count() > 0)
                {
                    
                    resultBinary = await binaryGetter(prevResult);
                    distributors.AddRange(resultBinary);
                    resultEmptyDownlines.AddRange(await binaryNoDownlinesGetter(resultBinary,count + 2));
                    levelReach++;
                } 
                prevResult = resultBinary;
                count += 1;
            } while (count <= 4);
            { 
                result.max_level = levelReach;
            }
            var dist_ids = distributors.Select(x => x.DISTRIBUTOR_REF)
                                        .DistinctBy(x => x).ToList();
            
            string queryDist = @"SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID IN @ids";
            var distDetailResult = conn.Query<DistributorsDetailsEntity>(queryDist, new
            {
                ids = dist_ids
            }).ToList();
            
            
            result.level_list.AddRange(distributors.Select(dst =>
            {
                var details = distDetailResult.Where(d => d.DISTRIBUTOR_ID == dst.DISTRIBUTOR_REF).FirstOrDefault();
                
                string user_img_str = "";

                if (string.IsNullOrEmpty(details.PICTURE_PATH) ){
                    user_img_str = "/assets/img/userimg.png";
                } else {
                    user_img_str = $"data:image/jpg;base64,{_fileHelper.GetImageUrl(details.PICTURE_PATH)}";
                }
                
                string data = $"{dst.ID}";
                data = _encryptionHelper.Encrypt(data);
                data = _encryptionHelper.EncodeStringToBase64(data);
                return new BinaryDetailsOutputDto
                {
                    id = dst.ID,
                    parentId = dst.PARENT_BINARY_REF,
                    imageUrl = user_img_str,
                    children = new List<BinaryDetailsOutputDto>(),
                    content = $"<div class=\"node-item\" name=\"dist-node\" dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\"><img src=\"{user_img_str}\" dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\"/><p data=\"{data}\">{details.FIRSTNAME} {details.LASTNAME}</p><small dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\">PH{details.DISTRIBUTOR_ID}</small></div>"
                };
            }));


            result.level_list.AddRange(resultEmptyDownlines);
            
            //for dummy hidden binary
            result.level_list.Add(new BinaryDetailsOutputDto
            {
                id = 1,
                parentId = 0,
                imageUrl = string.Empty,
                children = new List<BinaryDetailsOutputDto>(),
                content = $"<div hidden class=\"node-item\" name=\"no-distrbutors\" data=\"na\"><img src=\"/assets/img/userimg.png\" data=\"\"/><p data=\"\">No Distributor</p><small>N/A</small></div>"
            });
            
            conn.Close();
            conn.Dispose();
            return Ok(result);
        }
        catch (Exception ex)
        {
            conn.Close();
            conn.Dispose();
            throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
        }
    }

    private async Task<List<BinaryDetailsOutputDto>> binaryNoDownlinesGetter(List<BinaryTreeEntity> param_bin, int level)
    {
        var result = new List<BinaryDetailsOutputDto>();
        foreach (var bin in param_bin)
        {
            if (bin.CHILD_LEFT_BINARY_REF == null)
            {
                string paramUplineDetails =
                    _encryptionHelper.Encrypt(
                        $"{bin.ID}|{bin.DISTRIBUTOR_REF}|{BinaryPositionsEnum.LEFT.ToString()}|{level}");
                result.Add(new BinaryDetailsOutputDto
                {
                    id = 0,
                    parentId = bin.ID,
                    imageUrl = string.Empty,
                    children = new List<BinaryDetailsOutputDto>(),
                    content = $"<div class=\"node-item\" style=\"background-color: #e0e0e0;\" name=\"no-distrbutors\" dist-id=\"{paramUplineDetails}\"><img src=\"/assets/img/userimg.png\" dist-id=\"{paramUplineDetails}\"/><p dist-id=\"{paramUplineDetails}\"><span class=\"material-symbols-sharp\">add</span></p></div>"
                }); 
                
            }
            
            if (bin.CHILD_RIGHT_BINARY_REF == null)
            {
                string paramUplineDetails =
                    _encryptionHelper.Encrypt(
                        $"{bin.ID}|{bin.DISTRIBUTOR_REF}|{BinaryPositionsEnum.RIGHT.ToString()}|{level}");
                
                
                result.Add(new BinaryDetailsOutputDto
                {
                    id = 0,
                    parentId = bin.ID,
                    imageUrl = string.Empty,
                    children = new List<BinaryDetailsOutputDto>(),
                    content = $"<div class=\"node-item\" style=\"background-color: #e0e0e0;\" name=\"no-distrbutors\" dist-id=\"{paramUplineDetails}\"><img src=\"/assets/img/userimg.png\" dist-id=\"{paramUplineDetails}\"/><p dist-id=\"{paramUplineDetails}\"><span class=\"material-symbols-sharp\">add</span></p></div>"
                }); 
            }
        }
        return result;
    }

    private async Task<List<BinaryTreeEntity>> binaryGetter(List<BinaryTreeEntity> param_bin)
    {
        
        var conn = await connectionFactory.CreateConnectionAsync();
        var result = new List<BinaryTreeEntity>();
        foreach (var bin in param_bin)
        {
            string queryDownLines = @"SELECT * FROM binary_tree WHERE
                        ID in @dist_id ORDER BY ID ASC";
            var downLine_2 = conn.Query<BinaryTreeEntity>(queryDownLines, new
            {
                dist_id = $"{(bin.CHILD_LEFT_BINARY_REF ?? 0)},{(bin.CHILD_RIGHT_BINARY_REF ?? 0)}".Split(",")
            }).ToList();
            result.AddRange(downLine_2);
        }
        conn.Close();
        conn.Dispose();
        return result;
    }

    
    private string specialCharReplacer(string param,  string searchString, string replaceString)
    {
        string originalString = param; 

        int index = originalString.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            originalString = originalString.Remove(index, searchString.Length).Insert(index, replaceString);
            index = originalString.IndexOf(searchString, index + replaceString.Length, StringComparison.OrdinalIgnoreCase);
        }

        return originalString;
    }

    
    
    
}