using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class Product_GroupMetaData
    {

        [Key]
        public int GroupID { get; set; }

        [Display(Name = "عنوان گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string GroupTitle { get; set; }

         [Display(Name = "زیر گروه")]
        public Nullable<int> ParentID { get; set; }
    }

    [MetadataType(typeof(Product_GroupMetaData))]
    public partial class Product_Groups
    {

    }
}
