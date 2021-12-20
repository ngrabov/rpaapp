using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public class Writer : IdentityUser<int>
    {
        [Display(Name = "First Name")]
        [Required]
        [PersonalData]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression(@"^[A-ZČĆŽĐŠ]+[a-zA-ZšđčćžŠĐŽĆČ\-\. ]*$", ErrorMessage = "Last name must start with an uppercase letter. Only letters and hyphen sign (-) are supported.")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        [Required]
        [PersonalData]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression(@"^[A-ZČĆŽĐŠ]+[a-zA-ZšđčćžŠĐŽĆČ\-\. ]*$", ErrorMessage = "Last name must start with an uppercase letter. Only letters and hyphen sign (-) are supported.")]
        public string LastName {get; set; }
    }
}