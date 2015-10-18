using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string MemberName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
