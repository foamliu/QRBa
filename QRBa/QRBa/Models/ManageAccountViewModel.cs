using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Models
{
    public class ManageAccountViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "昵称")]
        public string Name { get; set; }
    }
}
