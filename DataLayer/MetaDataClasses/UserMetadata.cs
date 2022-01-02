using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class UserMetadata
    {

        [Key]
        public int UserID { get; set; }

         [Display(Name = "نقش کاربر")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        public int RoleID { get; set; }

        [Display(Name = "نام کاربری ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = " ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage="ایمیل معتبر نیست")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Password { get; set; }
        
        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = " تاریخ عضویت")]
        public System.DateTime RegisterDate { get; set; }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {

    }
}
