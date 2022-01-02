using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class Product_CommentMetaData
    {
        [Key]
        public int CommentID { get; set; }
        public int ProductID { get; set; }
        [Display(Name = "نام ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Nmae { get; set; }
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        public string Email { get; set; }
        [Display(Name = "وب سایت")]
        public string WebSite { get; set; }
        [Display(Name = "متن نظر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800)]
        public string Comment { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ParentID { get; set; }
    }
    [MetadataType(typeof(Product_CommentMetaData))]
    public partial class Product_Comment
    {

    }
}
