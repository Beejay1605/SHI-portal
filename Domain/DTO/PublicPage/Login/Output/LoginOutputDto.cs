
namespace Domain.DTO.PublicPage.Login.Output;

public class LoginOutputDto
{
    public string access_token {get;set;}
    public string private_token {get;set;}
    public string message {get;set;}

    public List<ErrorBaseDto> errors {get;set;}
}