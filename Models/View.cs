#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace I_Attend.Models
{
    public class View
    {
        //readonly View _view;
        public int Id { get; set; }
        
        [Required]
        public string UserNames { get; set; }
        [Required]
        public string Department { get; set; }
        [Required ]
        public string Email { get; set; }
        [Required]
        public string Matric_Number { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public View()
        {
          //_view = view;  
        }

        
    }
    public class LoginViewModel
    {
        //[Required]
        //public string UserNames { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Matric_Number { get; set; }

        //[Required]
        //public string EmailConfirmed { get; set; } = string.Empty;

      
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserNames { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Matric_Number { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }
}

