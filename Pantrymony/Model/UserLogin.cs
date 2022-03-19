using System.ComponentModel.DataAnnotations;

namespace Pantrymony.Model;

public class UserLogin
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string email { get; set; } // NOTE: email will be the username, too

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string password { get; set; }
}

public class ConfirmedUserLogin : UserLogin
{
    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("password", ErrorMessage = "Password and confirm password do not match.")]
    public string confirmpwd { get; set; }
}