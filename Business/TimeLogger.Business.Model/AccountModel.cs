using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeLogger.Business.Model
{
    public class UserClaimResponse
    {
        public string name { get; set; }
    }

    public class ConfirmEmailModel
    {
        [Required]
        public string UserId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Verification code can't be null")]
        public string Code { get; set; }
    }

    public class PasswordResetModel
    {

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Reset code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and repeat password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordChangeModel
    {
        [Required]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New password")]
        [Compare("NewPassword", ErrorMessage = "The new password and repeat password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class SetPasswordModel
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

    public class LoginModel
    {
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ExternalLoginModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsPersistent { get; set; }
        public bool BypassTwoFactor { get; set; }
    }

    public class RegisterExternalModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }

    public class RegisterUserModel
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

        [MaxLength(100, ErrorMessage = "Address must be less than 100 characters.")]
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

    public class RegisterMerchantModel
    {
        // Company info
        [MaxLength(255, ErrorMessage = "Company name cannot be more than 255 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "We need the name of the company.")]
        public string CompanyName { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "We need the company phone to continue.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid company phone number.")]
        public string CompanyPhone { get; set; }

       // [Required(AllowEmptyStrings = false, ErrorMessage = "We need the address of the company to continue.")]
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
        // [Required(AllowEmptyStrings = false, ErrorMessage = "We need the cell phone to continue.")]
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
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool CreateActivated { get; set; }
    }

    public class ActivationModel
    {
        public string Code { get; set; }
        public string Email { get; set; }
    }

    public class ResetModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }

    public class ResetPasswordByAdminModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The NewPassword and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class SendCodeModel
    {
        public string SelectedProvider { get; set; }

        //public ICollection<System.Web.Mvc.SelectListItem> Provider { get; set; }
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }

    public class VerifyCodeModel
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
    public class ChangeEmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AddPhoneNumberModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class VerifyPhoneNumberModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class AddLoginModel
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }

    public class RemoveLoginModel
    {
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }
}
