using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Models
{
    public class ExternalAuthenticationProvider
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string HandlerType { get; set; }
    }

    public class ExternalLoginViewModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsPersistent { get; set; }
        public bool BypassTwoFactor { get; set; }
    }

    public class RegisterExternalViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string UserName { get; set; }
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        public string UserName { get; set; }

        //[Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Remember this machine?")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RecoveryCodesViewModel
    {
        public string[] RecoveryCodes { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class UserLoginInfo
    {
        public string Provider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }
    }

    public class ExternalLoginsViewModel
    {
        public IList<UserLoginInfoModel> CurrentLogins { get; set; }

        public IList<AuthenticationSchemeModel> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        public string StatusMessage { get; set; }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public List<AuthenticationSchemeModel> ExternalAuthenticationProviders { get; set; }
    }

    public class RegisterUserViewModel
    {
        [Display(Name = "FirstName")]
        [StringLength(30, MinimumLength = 0, ErrorMessage = "The first name must contain less than 30 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [StringLength(30, MinimumLength = 0, ErrorMessage = "The last name must contain less than 30 characters.")]
        public string LastName { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid phone number.")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "We need the cell phone to continue.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Address { get; set; }

        public string ZipCode { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool CreateActivated { get; set; }
    }

    public class RegisterMerchantViewModel
    {
        // Company info
        [MaxLength(255, ErrorMessage = "Company name cannot be more than 255 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "We need the name of the company.")]
        public string CompanyName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid company phone number.")]
        public string CompanyPhone { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "We need the address of the company to continue.")]
        [MaxLength(100, ErrorMessage = "Company address must be less than 100 characters.")]
        public string CompanyAddress { get; set; }

        public string CompanyZipCode { get; set; }

        [MaxLength(255, ErrorMessage = "Company website cannot have more than 255 characters.")]
        [Url(ErrorMessage = "The URL is not valid.")]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "The URL is not valid. e.g http://google.com")]
        public string CompanyWebsite { get; set; }


        // User info
        [Required(ErrorMessage = "We need the name of the manager to continue.")]
        [MaxLength(30, ErrorMessage = "Name cannot have more than 30 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "We need the manager's last name to continue.")]
        [MaxLength(30, ErrorMessage = "Last name cannot have more than 30 characters.")]
        public string LastName { get; set; }

        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "We need the email to continue.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid phone number.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Address { get; set; }

        public string ZipCode { get; set; }

        [Required(ErrorMessage = "We need the password to continue.")]
        [StringLength(14, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "We need you to confirm the password to continue.")]
        [Compare("Password", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool CreateActivated { get; set; }
    }

    public class ConfirmEmailViewModel
    {
        [Required]
        public string UserId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Verification code can't be null")]
        public string Code { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ManageLoginsViewModel
    {
        //public IList<UserLoginInfo> CurrentLogins { get; set; }
        //public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
    }



    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("roles")]
        public string Roles { get; set; }

        [JsonProperty("error")]
        public string ErrorHeader { get; set; }

        [JsonProperty("error_description")]
        public string Error { get; set; }

    }

    public class AddLoginViewModel
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }

    public class RemoveLoginViewModel
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class UserAuthenticationInfo
    {
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        //public IList<UserLoginInfo> Logins { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorType { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public IList<ExternalAuthenticationProvider> OtherLogins { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class AuthenticatorViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        public string SharedKey { get; set; }

        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        public string AuthenticatorUri { get; set; }
    }

    public class UserClaim
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Picture { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateUserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Picture { get; set; }
    }

    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AccountDetail
    {
        public static Token TokenInfo { get; set; }

        public static UserClaim UserInfo { get; set; }

        public static LoginViewModel LoginInfo { get; set; }

        public static List<Role> Roles { get; set; }

        public static List<ExternalLoginViewModel> ExternalLoginUrls { get; set; }
    }
}