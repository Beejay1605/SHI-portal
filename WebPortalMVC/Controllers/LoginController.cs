using Dapper;
using Domain.DTO;
using Domain.DTO.PublicPage.Login.Input;
using Domain.DTO.PublicPage.Login.Output;
using Domain.Entity;
using FluentValidation.Results;
using Manager.Commons.Const;
using Manager.Commons.Enums;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Dapper.Configs;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Repository.Repositories.EF.Interfaces;
using WebPortalMVC.Authorizations;
using WebPortalMVC.FluentValidations.PublicPage;
using WebPortalMVC.Models;

namespace WebPortalMVC.Controllers
{
    public class LoginController : Controller
    {
        public LoginController(ILogger<LoginController> logger, IDbConnectionFactory connectionFactory, IJwtTokenService jwtTokenService, IEncryptionHelper encryptionHelper, IKeyHelper keyHelper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.connectionFactory = connectionFactory;
            this.jwtTokenService = jwtTokenService;
            this.encryptionHelper = encryptionHelper;
            _keyHelper = keyHelper;
            _unitOfWork = unitOfWork;
        }


        private readonly ILogger<LoginController> _logger;
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IEncryptionHelper encryptionHelper;
        private readonly IKeyHelper _keyHelper;
        private readonly IUnitOfWork _unitOfWork;


        public IActionResult Index()
        {
            return View();
        }   
        
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginRequest(LoginInputDto param_data) 
        {
            var conn = await connectionFactory.CreateConnectionAsync();
            LoginFValidation login_obj = new LoginFValidation();
            ValidationResult result = login_obj.Validate(param_data);
            if (!result.IsValid)
            {
                return StatusCode(403,result.Errors.Select(x => new ErrorBaseDto
                {
                    property_name = x.PropertyName,
                    message = x.ErrorMessage
                }).ToList());
            }

            string query = @"SELECT * FROM user_credentials
                                WHERE USERNAME=@USERNAME";
            var login_result = conn.Query<UserCredentialsEntity>(query, new
            {
                USERNAME = param_data.username
            }).ToList();

            if (login_result.Count() == 0)
            {
                return StatusCode(400, "Username and password doesn't exist");
            }

            UserCredentialsEntity pword_matches = new UserCredentialsEntity();
            bool is_check = false;
            foreach (var x in login_result)
            {
                if (encryptionHelper.Decrypt(x.PASSWORD) == param_data.password)
                {
                    pword_matches = x;
                    is_check = true;
                }
            }
            if (is_check == false)
            {
                return StatusCode(400,"Username and password doesn't exist");
            }

            string redirection_string = "";
            string firstName = string.Empty;
            string lastName = string.Empty;
            string contactNumber = string.Empty;
            int userId;
            if (pword_matches == null)
            {
                return StatusCode(400, "Username and password doesn't exist");
            }
            
            if (pword_matches.ACCESS_LEVEL == AccessLevelEnum.Operations.ToString()) // OPERATIONS OR ADMIN
            {
                redirection_string = "/Operations/Dashboard/Index";
                var queryOpsDetails = @"SELECT * FROM operations_details WHERE `CRED_REF`= @USER_CRED_REF";
                var detailsOpsResult = conn.Query<OperationsDetailsEntity>(queryOpsDetails, new
                {
                    USER_CRED_REF = pword_matches.ID
                }).FirstOrDefault();
                
                firstName = detailsOpsResult.FIRST_NAME;
                lastName = detailsOpsResult.LAST_NAME;
                contactNumber = detailsOpsResult.MOBILE_NUMBER.ToString();
                userId = detailsOpsResult.ID;
            }
            else
            {
                redirection_string = "/Distributor/Dashboard/Index";
                var query_details = @"SELECT * FROM distributors_details WHERE USER_CRED_REF=@USER_CRED_REF";
                var details_result = conn.Query<DistributorsDetailsEntity>(query_details, new
                {
                    USER_CRED_REF = pword_matches.ID
                }).FirstOrDefault();


                firstName = details_result.FIRSTNAME;
                lastName = details_result.LASTNAME;
                contactNumber = details_result.CONTACT_NUMBER;
                userId = details_result.DISTRIBUTOR_ID;
            }

            string privateTokenKey = Guid.NewGuid().ToString()+ _keyHelper.GenerateAlphaNumeric(16);
            string encryptedPrivateToken = encryptionHelper.Encrypt(privateTokenKey);
            var token_result = jwtTokenService.GenerateJwtToken(userId, pword_matches.USERNAME, firstName, 
                lastName, pword_matches.ACCESS_LEVEL, pword_matches.EMAIL, contactNumber, encryptedPrivateToken, pword_matches.ID);

            string encryptedToken = encryptionHelper.Encrypt(token_result);
            string ipAddressString = string.Empty;


            await _unitOfWork.UserTokensRepository.AddAsync(new UserTokensEntity
            {
                USER_REF = pword_matches.ID,
                TOKEN = privateTokenKey,
                EXPIRATION_UTC = DateTime.UtcNow.AddMinutes(30),
                IS_USED = false
            });

            await _unitOfWork.CommitAsync();
            
            conn.Close();
            conn.Dispose();
            return Ok(new
            {
                token = encryptedToken,
                redirection = redirection_string
            });
        }

        [HttpGet]
        public async Task<IActionResult> NewAccessToken()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", "");
            token = encryptionHelper.Decrypt(token);
            
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(400, "Token is required");
            }
            
            bool validateExpiredToken = jwtTokenService.ValidateExpiredToken(token);

            if (validateExpiredToken == false)
            {
                return Unauthorized();
            }
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var token_claims = jsonToken as JwtSecurityToken;
            
            string user_id = token_claims.Claims.First(claim => claim.Type == "user_id").Value;
            string id_claim = token_claims.Claims.First(claim => claim.Type == "id").Value; 
            string Username = token_claims.Claims.First(claim => claim.Type == "Username").Value; 
            string firstname = token_claims.Claims.First(claim => claim.Type == "firstname").Value; 
            string lastname = token_claims.Claims.First(claim => claim.Type == "lastname").Value; 
            string ac_level = token_claims.Claims.First(claim => claim.Type == "ac_level").Value; 
            string email = token_claims.Claims.First(claim => claim.Type == "email").Value; 
            string contact_number = token_claims.Claims.First(claim => claim.Type == "contact_number").Value;
            string refreshToken = token_claims.Claims.First(claim => claim.Type == "rtid").Value;

            refreshToken = encryptionHelper.Decrypt(refreshToken);
            
            bool privateTknResult = await ValidatePrivateToken(refreshToken ,  user_id);
            if (privateTknResult == false)
            {
                return Unauthorized();
            }
            
            
            var conn = await connectionFactory.CreateConnectionAsync();
            try
            {
                
                string privateTokenKey = (Guid.NewGuid().ToString() + _keyHelper.GenerateAlphaNumeric(16));
                string encryptedPrivateToken = encryptionHelper.Encrypt(privateTokenKey);
                var token_result = jwtTokenService.GenerateJwtToken(Int32.Parse(id_claim), Username, firstname, 
                    lastname, ac_level, email, contact_number, encryptedPrivateToken, Guid.Parse(user_id));

                string encryptedToken = encryptionHelper.Encrypt(token_result);
                
                conn.Execute(@"
                        INSERT INTO user_tokens 
                        (USER_REF, TOKEN, EXPIRATION_UTC, IS_USED)
                        VALUES 
                        (@USER_REF, @TOKEN, @EXPIRATION_UTC, @IS_USED)
                    ", new
                {
                    USER_REF = user_id,
                    TOKEN = privateTokenKey, 
                    EXPIRATION_UTC = DateTime.UtcNow.AddMinutes(30),
                    IS_USED = 0
                });

                conn.Execute(@"
                        DELETE FROM user_tokens where EXPIRATION_UTC < @timeNow
                    ", new
                {
                    timeNow = DateTime.UtcNow
                });
                conn.Close();
                conn.Dispose();
                return Ok(encryptedToken);
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                return Unauthorized();
            }
            
        }

        [HttpGet]
        [ClaimRequirement("Operations")]
        public async Task<IActionResult> AuthenticateOperationPage()
        {
            return Ok();
        }
        
        
        [HttpGet]
        [ClaimRequirement("Distributor")]
        public async Task<IActionResult> AuthenticateDistributorPage()
        {
            return Ok();
        }

        private async Task<bool> ValidatePrivateToken(string token, string userCred)
        {
            bool result = false;
        
            var conn = await connectionFactory.CreateConnectionAsync();
            string query = @"SELECT * FROM user_tokens where TOKEN = @token 
            AND EXPIRATION_UTC >= @timeNow AND IS_USED='0' 
            AND USER_REF=@id";

            var tokenResult = conn.Query<UserTokensEntity>(query, new
            {
                TOKEN = token,
                timeNow = DateTime.UtcNow,
                id = userCred
            }).FirstOrDefault();

            if (tokenResult != null)
            {
                result = true;
            }
            conn.Close();
            conn.Dispose();
            return result;
        }
        
        [HttpPost]
        public async Task<IActionResult> TokenDecodeOperations(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("");
            }

            string jwtToken = encryptionHelper.Decrypt(token);
             
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken);
                var token_claims = jsonToken as JwtSecurityToken;
                var claimResult = new UserClaimsBase();
                claimResult.firstname = token_claims.Claims.First(claim => claim.Type == "firstname").Value;
                claimResult.lastname = token_claims.Claims.First(claim => claim.Type == "lastname").Value;
                
                return Ok(claimResult);
            }
            catch (Exception ex){}
            
            
            return Ok();
        }
    }

}