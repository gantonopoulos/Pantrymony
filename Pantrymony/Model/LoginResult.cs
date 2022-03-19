namespace Pantrymony.Model;

public class LoginResult
{
	public string message { get; set; }
	public string email { get; set; }
	public string jwtBearer { get; set; }
	public bool success { get; set; }
}