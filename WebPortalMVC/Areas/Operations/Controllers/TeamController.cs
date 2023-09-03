using System.Net;
using System.Text.RegularExpressions;
using Dapper;
using Domain.DTO;
using Domain.DTO.BaseDto;
using Domain.DTO.Operations.Payincodes.input;
using Domain.DTO.Operations.POS.Output;
using Domain.DTO.Operations.Team.input;
using Domain.DTO.Operations.Team.output;
using Domain.Entity;
using FluentValidation.Results;
using Manager.Commons.Const;
using Manager.Commons.Enums;
using Manager.Commons.Exceptions;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;
using WebPortalMVC.FluentValidations.Operations.Distributors;
using WebPortalMVC.FluentValidations.Operations.Payincode;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class TeamController : Controller
    {
        public TeamController(IDbConnectionFactory connectionFactory, IUnitOfWork unitOfWork, IEntityMapper mapper, IFileHelper fileHelper, IEncryptionHelper encryptionHelper, ICurrentUserService currentUser)
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
 

        [Route("Operations/Distributors/{id}/Team")]
        public async Task<IActionResult> Index(string id, string? tp)
        {
            ViewData["SidebarLocation"] = "Distributors";
            
            
            if (!string.IsNullOrEmpty(tp) && tp=="binary")
            {
                ViewBag.isBinary = true; 
                var mdl = new DistributorsDetailsDto();
                ViewBag.DATA = id;
                return View(new DistributorsDetailsDto());
            }else
            {
                ViewBag.isBinary = false;
            }
            var conn = await connectionFactory.CreateConnectionAsync();
            try
            {
                var result = new TeamOutputDto();
                string queryString = @"SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID=@id";
                var distributor = conn.Query<DistributorsDetailsEntity>(queryString, new
                {
                    id = Int32.Parse(id)
                }).FirstOrDefault();

                if (distributor == null)
                {
                    throw new ErrorException(HttpStatusCode.InternalServerError, "", "");
                }

                var mapResult = _mapper.distributorDetailsMap.Map(distributor);

                //result.distributor = mapResult;
                return View(mapResult);
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message, ex);
            }
            
            
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DistributorsTeam(int id)
        {

            var conn = await connectionFactory.CreateConnectionAsync();
            try
            {
                var result = new TeamOutputDto(); 
                
                string queryString = @"SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID=@id";
                var distributor = conn.Query<DistributorsDetailsEntity>(queryString, new
                {
                    id = id
                }).FirstOrDefault();
                
                if (distributor == null)
                {
                    throw new ErrorException(HttpStatusCode.InternalServerError, "", "");
                }

                if (!string.IsNullOrEmpty(distributor.PICTURE_PATH))
                {
                    distributor.PICTURE_PATH = _fileHelper.GetImageUrl(distributor.PICTURE_PATH);
                }
                
                var mapResult = _mapper.distributorDetailsMap.Map(distributor);
                
                result.details = mapResult;

                result.level_list = new List<BinaryDetailsOutputDto>();



                string queryFirstBinary = @"SELECT * FROM binary_tree WHERE
                            DISTRIBUTOR_REF=@dist_id ORDER BY ID ASC";
                var firstRes = conn.Query<BinaryTreeEntity>(queryFirstBinary, new
                {
                    dist_id = id
                }).FirstOrDefault();
                
                
                
                var distributors = new List<BinaryTreeEntity>();
                var resultEmptyDownlines = new List<BinaryDetailsOutputDto>();

                if (firstRes == null)
                {
                    string paramUplineDetails =
                        _encryptionHelper.Encrypt(
                            $"|||{1}");
                    result.level_list.Add(new BinaryDetailsOutputDto
                    {
                        id = 1, 
                        imageUrl = string.Empty,
                        children = new List<BinaryDetailsOutputDto>(),
                        content = $"<div class=\"node-item\" style=\"background-color: #e0e0e0;\" name=\"no-distrbutors\" dist-id=\"{paramUplineDetails}\"><img src=\"/assets/img/userimg.png\" dist-id=\"{paramUplineDetails}\"/><p dist-id=\"{paramUplineDetails}\"><span class=\"material-symbols-sharp\">add</span></p></div>"
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
                        content = $"<div class=\"node-item\" name=\"dist-node\" dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\"><img src=\"{user_img_str}\" dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\"/><p dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\">{details.FIRSTNAME} {details.LASTNAME}</p><small dist-id=\"{details.DISTRIBUTOR_ID}\" bin-id=\"{dst.ID}\" data=\"{data}\">PH{details.DISTRIBUTOR_ID}</small></div>"
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
        
        
        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DistributorsTeamViaBinary(string dt) // DISTRIBUTOR ID|BINARY ID
        {
            var conn = await connectionFactory.CreateConnectionAsync();
            try
            {
                dt = _encryptionHelper.DecodeBase64String(dt);
                dt = _encryptionHelper.Decrypt(dt);
                int distId = Int32.Parse(dt);
                var result = new TeamOutputDto(); 
                
                
                string queryFirstBinary = @"SELECT * FROM binary_tree WHERE
                            ID=@dist_id ORDER BY ID ASC";
                var firstRes = conn.Query<BinaryTreeEntity>(queryFirstBinary, new
                {
                    dist_id = distId
                }).FirstOrDefault();
                
                
                string queryString = @"SELECT * FROM distributors_details WHERE  DISTRIBUTOR_ID=@id";
                var distributor = conn.Query<DistributorsDetailsEntity>(queryString, new
                {
                    id = firstRes.DISTRIBUTOR_REF
                }).FirstOrDefault();
                
                if (distributor == null)
                {
                    throw new ErrorException(HttpStatusCode.InternalServerError, "", "");
                }

                distributor.PICTURE_PATH = _fileHelper.GetImageUrl(distributor.PICTURE_PATH);
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

                    if (string.IsNullOrEmpty(mapResult.user_picture_base_64) ){
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


        [Route("Operations/Distributors/{id:int}/Team/Encode/{data}")]
        public async Task<IActionResult> EncodeTeam(int id, string data)
        {
            var conn = await connectionFactory.CreateConnectionAsync();
            ViewData["SidebarLocation"] = "Distributors";
            ViewBag.ID = id;
            ViewBag.DATA = data;

            data = specialCharReplacer(data, "-xtempx93-xtemp-", "/");
            data = specialCharReplacer(data, "xpls-xtemp-xplus", "+");
            
            string delimitedStr = _encryptionHelper.Decrypt(data);
            
            var data_arr = (delimitedStr).Split("|");
            
            string queryString = @"SELECT * FROM distributors_details
                    WHERE DISTRIBUTOR_ID=@id";
            var result = conn.Query<DistributorsDetailsEntity>(queryString, new
            {
                id = data_arr[1]
            }).FirstOrDefault();
            
            
            
            ViewBag.POSITION = data_arr[2];
            ViewBag.UPLINE_FULLNAME = $"{result?.FIRSTNAME} {result?.LASTNAME} {result?.SUFFIX} (PH{result?.DISTRIBUTOR_ID})";
                
            conn.Close();
            conn.Dispose();
            return View();
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
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> EncodePayIns(EncodePayinsInputDto input)
        {
            EncodePayinFValidation encodePayin = new EncodePayinFValidation();
            ValidationResult result = encodePayin.Validate(input);
            
            if (!result.IsValid)
            {
                return StatusCode(400, result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }
            
            var conn = await connectionFactory.CreateConnectionAsync();
            
            input.binary_data = specialCharReplacer(input.binary_data, "-xtempx93-xtemp-", "/");
            input.binary_data = specialCharReplacer(input.binary_data, "xpls-xtemp-xplus", "+");
            
            string delimitedStr = _encryptionHelper.Decrypt(input.binary_data);
            
            var data_arr = (delimitedStr).Split("|");

            int? parentBinId = !string.IsNullOrEmpty(data_arr[0]) ? Int32.Parse(data_arr[0]) :  null;
            int? uplineId = !string.IsNullOrEmpty(data_arr[1]) ? Int32.Parse(data_arr[1]) :  null;
            string position = data_arr[2];
            int level = Int32.Parse(data_arr[3]);

            var error_result = new List<ErrorBaseDto>();
            if (uplineId == null)
            {
                if (input.distributor_id != input.upline_id)
                {
                    error_result.Add(new ErrorBaseDto
                    {
                        property_name = "distributor-id",
                        message = $"You can encode your first account only for PH{input.upline_id}"
                    });
                    return StatusCode(400, error_result.ToList());
                }
            }
            
            string keyCodeQuery = @"SELECT * FROM payin_codes 
                                            WHERE PAYIN_CODE=@code AND 
                                                IS_USED='0'"; 
            var keyResult = conn.Query<PayinCodesEntity>(keyCodeQuery, new
            {
                code = input.key_code
            }).FirstOrDefault();
                
            if (keyResult == null)
            {
                error_result.Add(new ErrorBaseDto
                {
                    property_name = "key-code",
                    message = "Invalid Pay-in Code"
                });
                return StatusCode(400, error_result.ToList());
            }
                
            if (keyResult.DISTRIBUTOR_REF != input.upline_id && keyResult.DISTRIBUTOR_REF != input.distributor_id)
            {
                    
                error_result.Add(new ErrorBaseDto
                {
                    property_name = "key-code",
                    message = "Pay-in Code is not for this Distributor"
                });
                return StatusCode(400, error_result.ToList());
            }
            
            
            

            if (input.imaginary_upline_id != 0)
            {
                var imaginaryParentCheck = conn.Query<BinaryTreeEntity>(@"SELECT * FROM binary_tree
                                                    WHERE DISTRIBUTOR_REF = @parentId", new
                {
                    parentId = input.imaginary_upline_id
                }).FirstOrDefault();
                // move this to first validation
                if (imaginaryParentCheck == null)
                {
                    error_result.Add(new ErrorBaseDto
                    {
                        property_name = "distributor-upline",
                        message = "Upline ID Number does'nt exist."
                    });
                    return StatusCode(400, error_result.ToList());
                }
            }
            
            try
            {

                var toEncode = new BinaryTreeEntity
                {
                    DISTRIBUTOR_REF = input.distributor_id,
                    POSITION = position,
                    LEVELS = level,
                    CREATED_AT_UTC = DateTime.UtcNow,
                };

                if (uplineId != null)
                {
                    toEncode.UPLINE_DETAILS_REF = uplineId;
                }


                BinaryTreeEntity? parentResult = new BinaryTreeEntity();

                BinaryTreeEntity? imaginaryParent = new BinaryTreeEntity();
                imaginaryParent = null;
                BinaryTreeEntity? imaginaryGrandParent = new BinaryTreeEntity();
                imaginaryGrandParent = null;
                if (parentBinId != null)
                {
                    toEncode.PARENT_BINARY_REF = parentBinId;
                    string parentQuery = @"SELECT * FROM binary_tree
                                                WHERE ID = @parentId";

                    parentResult = conn.Query<BinaryTreeEntity>(parentQuery, new
                    {
                        parentId = parentBinId
                    }).FirstOrDefault();

                    if (parentResult == null && parentBinId != null)
                    {
                        throw new ErrorException("No Parent ID found!");
                    }

                    if ((input.imaginary_upline_id) != 0)
                    {
                        if (parentResult.DISTRIBUTOR_REF != input.imaginary_upline_id)
                        {
                            imaginaryParent = conn.Query<BinaryTreeEntity>(@"SELECT * FROM binary_tree
                                                WHERE DISTRIBUTOR_REF = @parentId", new
                            {
                                parentId = input.imaginary_upline_id
                            }).FirstOrDefault();


                            if (imaginaryParent != null )
                            {
                                toEncode.IMAGINARY_UPLINE_BIN_REF = imaginaryParent.ID;
                                
                                if (imaginaryParent.IMAGINARY_UPLINE_BIN_REF != null)
                                {
                                    imaginaryGrandParent = conn.Query<BinaryTreeEntity>(@"SELECT * FROM binary_tree
                                                WHERE DISTRIBUTOR_REF = @parentId", new
                                    {
                                        parentId = imaginaryParent.IMAGINARY_UPLINE_BIN_REF
                                    }).FirstOrDefault();
                                }
                                else
                                {
                                    // if parent dont have imaginary upline
                                    imaginaryGrandParent = conn.Query<BinaryTreeEntity>(@"SELECT * FROM binary_tree
                                                WHERE ID = @parentId", new
                                    {
                                        parentId = imaginaryParent.PARENT_BINARY_REF
                                    }).FirstOrDefault();
                                }
                            }
                        }
                    }
                    else 
                    {
                        // checking if Node Parent have a Imaginary Upline
                        if (parentResult.IMAGINARY_UPLINE_BIN_REF != null)
                        {
                            imaginaryGrandParent = conn.Query<BinaryTreeEntity>(@"SELECT * FROM binary_tree
                                                    WHERE ID = @parentId", new
                            {
                                parentId = parentResult.IMAGINARY_UPLINE_BIN_REF
                            }).FirstOrDefault();
                        }
                    }
                    
                    _unitOfWork.BinaryTreeRepository.Update(parentResult);
                }
                
                if (!string.IsNullOrEmpty(position))
                {
                    toEncode.POSITION = position;
                }

                toEncode.PAYIN_CODE_REF = keyResult.ID;
                keyResult.IS_USED = true;
                await _unitOfWork.BinaryTreeRepository.AddAsync(toEncode);
                _unitOfWork.PayinCodesRepository.Update(keyResult);
                await _unitOfWork.CommitAsync();

                if (parentBinId != null)
                {

                    
                    #region DIRECT_REFERAL_BONUS

                    int benefBinID = parentBinId ?? 0;
                    int benefDistID = parentResult.DISTRIBUTOR_REF;
                    if (imaginaryParent != null)
                    {
                        benefBinID = imaginaryParent.ID;
                        benefDistID = imaginaryParent.DISTRIBUTOR_REF;
                    }
                    
                    
                    await _unitOfWork.EarningsReferal.AddAsync(new EarningsReferalEntity
                    {
                        BENEF_BINARY_REF = benefBinID,
                        FROM_BINARY_REF = toEncode.ID,
                        BONUS_TYPE = EarningsEnum.DIRECT_REFERAL.ToString(),
                        BENEF_DISTRIBUTOR_REF = benefDistID,
                        CREATED_DT = DateTime.UtcNow,
                        AMOUNT = IncomeConst.DIRECT_REFERAL_AMOUNT,
                        IS_ENCASH = false
                    }); 
                    #endregion

                    #region INDIRECT_REFERAL 
                    if (parentResult.PARENT_BINARY_REF != null)
                    {
                        string grandParentQuery = @"SELECT * FROM binary_tree
                                                WHERE ID = @parentId";

                        var grandParentResult = conn.Query<BinaryTreeEntity>(grandParentQuery, new
                        {
                            parentId = parentResult.PARENT_BINARY_REF
                        }).FirstOrDefault();

                        if (grandParentResult != null)
                        {
                            int benefGrandBinID = grandParentResult.ID;
                            int benefGrandDistID = grandParentResult.DISTRIBUTOR_REF;

                            if (imaginaryGrandParent != null)
                            {
                                benefGrandBinID = imaginaryGrandParent.ID;
                                benefGrandDistID = imaginaryGrandParent.DISTRIBUTOR_REF;
                            }
                            
                            await _unitOfWork.EarningsReferal.AddAsync(new EarningsReferalEntity
                            {
                                BENEF_BINARY_REF = benefGrandBinID,
                                FROM_BINARY_REF = toEncode.ID,
                                BONUS_TYPE = EarningsEnum.INDIRECT_REFERAL.ToString(),
                                CREATED_DT = DateTime.UtcNow,
                                BENEF_DISTRIBUTOR_REF = benefGrandDistID,
                                AMOUNT = IncomeConst.INDIRECT_REFERAL_AMOUNT,
                                IS_ENCASH = false
                            });
                        }
                    }

                    
                    #endregion

                    #region PAIRING_BONUS

                    var pairingIDsInvolve = new List<int?>();
                    // 4TH DEGREE ----------
                    
                    var pairingQuery = @"SELECT * FROM earnings_pairing 
                                    WHERE BENEF_BIN_ID=@PBinId AND IS_ENCASH=0";
                    var pairingResult = conn.Query<EarningsPairingEntity>(pairingQuery + " AND LEVEL=1", new
                    {
                        PBinId = parentResult.ID
                    }).FirstOrDefault();
                    if (pairingResult == null)
                    {
                        if (toEncode.POSITION == BinaryPositionsEnum.LEFT.ToString())
                        {
                            await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                            {
                                LEFT_BIN_ID = toEncode.ID,
                                BENEF_BIN_ID = parentResult.ID,
                                BENEF_DIST_ID = parentResult.DISTRIBUTOR_REF,
                                AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                IS_ENCASH = false,
                                LEVEL = 1,
                                CREATED_DT = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                            {
                                RIGHT_BIN_ID = toEncode.ID,
                                BENEF_BIN_ID = parentResult.ID,
                                BENEF_DIST_ID = parentResult.DISTRIBUTOR_REF,
                                AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                IS_ENCASH = false,
                                LEVEL = 1,
                                CREATED_DT = DateTime.UtcNow
                            });
                        }
                    }
                    else
                    {
                        if (toEncode.POSITION == BinaryPositionsEnum.LEFT.ToString())
                        {
                            pairingResult.LEFT_BIN_ID = toEncode.ID;
                            pairingResult.CREATED_DT = DateTime.UtcNow;
                        }
                        else
                        {
                            pairingResult.RIGHT_BIN_ID = toEncode.ID;
                            pairingResult.CREATED_DT = DateTime.UtcNow;
                        }
                        pairingIDsInvolve.Add(pairingResult.BENEF_DIST_ID);
                        _unitOfWork.EarningsPairing.Update(pairingResult);
                    }
                    
                    //--------------

                    #region THIRD_DEGREE

                    // 3rd DEGREE ------------
                    if (parentResult.PARENT_BINARY_REF != null)
                    {
                        // 3rd DEGREE --------------------

                        string grandParent = @"SELECT * FROM binary_tree
                                                WHERE ID = @parentId"; 
                        
                        var grandParentResult = conn.Query<BinaryTreeEntity>(grandParent, new
                        {
                            parentId = parentResult.PARENT_BINARY_REF
                        }).FirstOrDefault();
                        
                        var pairingResult_2 = conn.Query<EarningsPairingEntity>(pairingQuery + @" AND LEVEL=2
                            AND (LEFT_BIN_ID IS NULL OR RIGHT_BIN_ID IS NULL)", new
                        {
                            PBinId = parentResult.PARENT_BINARY_REF
                        }).FirstOrDefault();
                        
                        
                        if (pairingResult_2 != null)
                        {
                            if (parentResult.POSITION == BinaryPositionsEnum.LEFT.ToString())
                            {
                                pairingResult_2.LEFT_BIN_ID = toEncode.ID;
                                pairingResult_2.CREATED_DT = DateTime.UtcNow;
                            }
                            else
                            {
                                pairingResult_2.RIGHT_BIN_ID = toEncode.ID;
                                pairingResult_2.CREATED_DT = DateTime.UtcNow;
                            }
                            pairingIDsInvolve.Add(pairingResult_2.BENEF_DIST_ID);
                            _unitOfWork.EarningsPairing.Update(pairingResult_2);
                        }
                        else
                        {
                            if (parentResult.POSITION == BinaryPositionsEnum.LEFT.ToString())
                            {
                                await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                                {
                                    LEFT_BIN_ID = toEncode.ID,
                                    BENEF_BIN_ID = grandParentResult.ID,
                                    BENEF_DIST_ID = grandParentResult.DISTRIBUTOR_REF,
                                    AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                    IS_ENCASH = false,
                                    LEVEL = 2
                                });
                            }
                            else
                            {
                                await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                                {
                                    RIGHT_BIN_ID = toEncode.ID,
                                    BENEF_BIN_ID = grandParentResult.ID,
                                    BENEF_DIST_ID = grandParentResult.DISTRIBUTOR_REF,
                                    AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                    IS_ENCASH = false,
                                    LEVEL = 2
                                });
                            } 
                        }

                        // -------------------------------


                        #region SECOND_DEGREE

                        if (grandParentResult.PARENT_BINARY_REF != null)
                        {
                            var grandParentResult_2 = conn.Query<BinaryTreeEntity>(grandParent, new
                            {
                                parentId = grandParentResult.PARENT_BINARY_REF
                            }).FirstOrDefault();
                            
                            var pairingResult_3 = conn.Query<EarningsPairingEntity>(pairingQuery + @" AND LEVEL=3
                            AND (LEFT_BIN_ID IS NULL OR RIGHT_BIN_ID IS NULL)", new
                            {
                                PBinId = grandParentResult.PARENT_BINARY_REF
                            }).FirstOrDefault();

                            if (pairingResult_3 != null)
                            {
                                pairingIDsInvolve.Add(pairingResult_3.BENEF_DIST_ID);
                                if (grandParentResult.POSITION == BinaryPositionsEnum.LEFT.ToString())
                                {
                                    pairingResult_3.LEFT_BIN_ID = toEncode.ID; 
                                    pairingResult_3.CREATED_DT = DateTime.UtcNow;
                                }
                                else
                                {
                                    pairingResult_3.RIGHT_BIN_ID = toEncode.ID;
                                    pairingResult_3.CREATED_DT = DateTime.UtcNow;
                                }
                                pairingIDsInvolve.Add(pairingResult_3.BENEF_DIST_ID);
                                _unitOfWork.EarningsPairing.Update(pairingResult_3);
                            }
                            else
                            {
                                if (parentResult.POSITION == BinaryPositionsEnum.LEFT.ToString())
                                {
                                    await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                                    {
                                        LEFT_BIN_ID = toEncode.ID,
                                        BENEF_BIN_ID = grandParentResult_2.ID,
                                        BENEF_DIST_ID = grandParentResult_2.DISTRIBUTOR_REF,
                                        AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                        IS_ENCASH = false,
                                        LEVEL = 3
                                    });
                                }
                                else
                                {
                                    await _unitOfWork.EarningsPairing.AddAsync(new EarningsPairingEntity
                                    {
                                        RIGHT_BIN_ID = toEncode.ID,
                                        BENEF_BIN_ID = grandParentResult_2.ID,
                                        BENEF_DIST_ID = grandParentResult_2.DISTRIBUTOR_REF,
                                        AMOUNT = IncomeConst.PAIRING_BONUS_AMOUNT,
                                        IS_ENCASH = false,
                                        LEVEL = 3
                                    });
                                } 
                            }
                        }

                        #endregion

                    }
                    
                    
                    
                    #endregion
                    #endregion
                    
                    
                    await _unitOfWork.CommitAsync();
                    
                    string parentQuery = @""; 
                    if (position == BinaryPositionsEnum.LEFT.ToString())
                    {
                        parentQuery = @"UPDATE binary_tree SET CHILD_LEFT_BINARY_REF=@childId 
                                            WHERE ID = @parentId";
                    }
                    else
                    {
                        parentQuery = @"UPDATE binary_tree SET CHILD_RIGHT_BINARY_REF=@childId 
                                            WHERE ID = @parentId";
                    }
                    conn.Execute(parentQuery, new
                    {
                        childId = toEncode.ID,
                        parentId = parentBinId
                    });
                    
                    
                    // checking all the limit for 8 pairs a day
                    if (pairingIDsInvolve.Count() > 0)
                    {
                        string pairingCheckQuery = @"
                            SELECT * FROM earnings_pairing  as ep
                                    WHERE BENEF_DIST_ID IN @ids
                                        AND ep.`IS_ENCASH`=0 
                                        AND (ep.`LEFT_BIN_ID` IS NOT NULL AND ep.`RIGHT_BIN_ID` IS NOT NULL)
                                        AND ep.`AMOUNT` > 0
                        ";
                        var pairingCheckResult = conn.Query<EarningsPairingEntity>(pairingCheckQuery, new
                        {
                            ids = pairingIDsInvolve.Where(x => x != null).ToList()
                        }).Where(x => x.CREATED_DT.ToString("yyyyMMdd") == DateTime.UtcNow.ToString("yyyyMMdd")).ToList();

                        pairingIDsInvolve = pairingIDsInvolve.Distinct().ToList();
                        foreach (var pi in pairingIDsInvolve.Where(x => x != null))
                        {
                            var pcr = pairingCheckResult.Where(x => x.BENEF_DIST_ID == pi).ToList();
                            int c = 0;
                            foreach (var p in pcr)
                            {
                                if (c > 7)
                                {
                                    p.AMOUNT = 0;
                                    conn.Execute(@"UPDATE earnings_pairing SET AMOUNT=0
                                                         WHERE ID=@id", new
                                    {
                                        id = p.ID
                                    });
                                }
                                c++;
                            }
                        }

                    }
                    
                } 
                
                conn.Close();
                conn.Dispose();
                return Ok();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                conn.Close();
                conn.Dispose();
                return StatusCode(500);
            }
        }


        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> DistributorDetails(int id)
        {
            var conn = await connectionFactory.CreateConnectionAsync();
            string queryString = @"SELECT * FROM distributors_details
                    WHERE DISTRIBUTOR_ID=@id";
            var result = conn.Query<DistributorsDetailsEntity>(queryString, new
            {
                id = id
            }).FirstOrDefault();

            if (!string.IsNullOrEmpty(result.PICTURE_PATH))
            {
                result.PICTURE_PATH = _fileHelper.GetImageUrl(result.PICTURE_PATH);
            }
            
            
            var mapResult = _mapper.distributorDetailsMap.Map(result);

            var res = conn.Query<int>(@"
                            SELECT COUNT(ID) FROM binary_tree WHERE
                                DISTRIBUTOR_REF = @id", new
            {
                id = id
            }).FirstOrDefault();
            
            conn.Close();
            conn.Dispose();
            return Ok(new
            {
                details = mapResult,
                totalAccounts = res
            });
        }


        [Route("Operations/Distributors/{distId:int}/Team/Encode/Unilevel/{binaryId:int}")]
        public async Task<IActionResult> UniLevelEncode(int distId ,int binaryId,string? id)
        {
            var conn = await connectionFactory.CreateConnectionAsync();

             var binaryResult = conn.Query<BinaryTreeEntity>(@"
                 SELECT * FROM binary_tree WHERE DISTRIBUTOR_REF=@dr AND ID=@bi
             ", new
             {
                 dr = distId,
                 bi = binaryId
             }).FirstOrDefault();
            
             if (binaryResult == null)
             {
                 return RedirectToAction("Index", "Distributors");
             }
             
            ViewBag.DIST_ID = distId;
            ViewBag.BIN_ID = binaryId;
            ViewBag.ID = id;
            
            if (id == null)
            {
                ViewBag.MESSAGE = "Transaction ID doesn't exist.";
                return View();
            }
            ViewBag.MESSAGE = "";
             

            var transactions = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE CONCAT(TRANSACTION_NUMBER,ID) = @tranid OR ID = @tranid
            ", new
            {
                tranid = id
            }).FirstOrDefault();
            

            if (transactions == null)
            { 
                ViewBag.MESSAGE = "Transaction ID doesn't exist.";
                return View(new VoidOutputDto());
            }

            ViewBag.TRANID = transactions.ID;
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
        [ValidateAntiForgeryToken]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> EncodeUnilevelPoints(EncodeBinaryOutputDto input)
        {
            var conn = await connectionFactory.CreateConnectionAsync();
            
            var binaryResult = conn.Query<BinaryTreeEntity>(@"
                 SELECT * FROM binary_tree WHERE DISTRIBUTOR_REF=@dr AND ID=@bi
             ", new
            {
                dr = input.distributor_id,
                bi = input.binary_id
            }).FirstOrDefault();
            
            if (binaryResult == null)
            {
                return BadRequest("An error occured. please refresh the page");
            }
            
            var transactions = conn.Query<TransactionsEntity>(@"
                SELECT * FROM transactions WHERE CONCAT(TRANSACTION_NUMBER,ID) = @tranid OR ID = @tranid
            ", new
            {
                tranid = input.transaction_id
            }).FirstOrDefault();

            if (transactions == null)
            {  
                return BadRequest("Transaction ID doesn't exist.");
            }
            
            if (transactions.IS_ENCODED_UNILEVEL == true)
            {  
                return BadRequest("Transaction ID is already to encode on uni level.");
            }

            var totalPrice = conn.Query<ResultDto>(@"SELECT SUM(POS.`PER_UNIT_PRICE` * POS.`QUANTITY`) as result_decimal FROM pos_transactions AS POS 
                INNER JOIN products AS PR
                ON POS.`PRODUCT_REF` = PR.`ID`
                WHERE TRANSACTION_REF = @tranid
                AND PR.`IS_PACKAGE` = 0", new
            {
                tranid = input.transaction_id
            }).FirstOrDefault();

            if (totalPrice.result_decimal == 0)
            {
                return BadRequest("Transaction Number has no products counted to unilevel points.");
            } 
            var result = new List<EarningsUniLevelEntity>();
            
            DateTime currentDate = DateTime.Today;
            DateTime firstDayOfNextMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);

            decimal totalEarned = (totalPrice.result_decimal * IncomeConst.UNILEVEL_PERCENTAGE);
            
            result.Add(new EarningsUniLevelEntity
            {
                BINARY_REF = binaryResult.ID,
                DISTRIBUTOR_REF = binaryResult.DISTRIBUTOR_REF,
                TRANSACTION_REF = transactions.ID,
                CREATED_DATE = DateTime.UtcNow,
                AVAILABILITY_DATE = firstDayOfNextMonth,
                IS_ENCASH = false,
                AMOUNT = totalEarned
            });

            var prevBin = binaryResult;
            if (binaryResult.PARENT_BINARY_REF != null)
            {
                for (int c = 0; c < (IncomeConst.UNILEVEL_MAXIMUM_LEVEL - 1); c++)
                {
                    var binUplines = conn.Query<BinaryTreeEntity>(@"
                        SELECT * FROM binary_tree WHERE ID=@binId
                    ", new
                    {
                        binId = prevBin.PARENT_BINARY_REF
                    }).FirstOrDefault();

                    result.Add(new EarningsUniLevelEntity
                    {
                        BINARY_REF = binUplines.ID,
                        DISTRIBUTOR_REF = binUplines.DISTRIBUTOR_REF,
                        TRANSACTION_REF = transactions.ID,
                        CREATED_DATE = DateTime.UtcNow,
                        AVAILABILITY_DATE = firstDayOfNextMonth,
                        IS_ENCASH = false,
                        AMOUNT = totalEarned
                    });
                    prevBin = binUplines;
                    if (binUplines.PARENT_BINARY_REF == null)
                    {
                        break;
                    }
                }
            } 
            transactions.IS_ENCODED_UNILEVEL = true;
            
            try
            {
                await _unitOfWork.EarningsUniLevel.AddRangeAsync(result);
                _unitOfWork.TransactionsRepository.Update(transactions);
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
                throw new ErrorException(HttpStatusCode.InternalServerError, ex.Message,ex);
            } 
        }


    }
}
