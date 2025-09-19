#nullable disable



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;

//namespace I_Attend.Models
//{
//    public class View
//    {
//        public int Id { get; set; }

//        [Required]
//        public string UserNames { get; set; }
//        [Required]
//        public string Department { get; set; }
//        [Required]
//        public string Email { get; set; }
//        [Required]
//        public string Matric_Number { get; set; }
//        [Required]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }
//        public byte[] ImageData { get; set; } // Removed [Required]
//        public string CourseCode { get; set; } // Added for course selection

//        public View()
//        {
//        }
//    }

//    public class LoginViewModel
//    {
//        [Required]
//        [DataType(DataType.EmailAddress)]
//        public string Email { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }
//        [Required]
//        public string Matric_Number { get; set; }
//    }

//    public class RegisterViewModel
//    {
//        [Required]
//        public string UserNames { get; set; }

//        [Required]
//        public string Department { get; set; }

//        [Required]
//        [EmailAddress]
//        public string Email { get; set; }

//        [Required]
//        public string Matric_Number { get; set; }

//        [Required]
//        public string CourseCode { get; set; } 

//        [Required]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        [Compare("Password", ErrorMessage = "Passwords do not match.")]
//        public string ConfirmPassword { get; set; }
//    }
//}





using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Matric_Number { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        [Required]
        [DisplayName("Course_code")]
        public string Course_code { get; set; } 


        //public View()
        //{
        //    //_view = view;
        //    //CourseList = newList<SelectListItem>();
        //}


    }
    public class Course_List
    {
        //public string Course_code { get; set; }
        [Required]
        [DisplayName("Course_code")]
        public string Course_code { get; set; }

        [Required]
        public string Matric_Number { get; set; }
    }
    public class AdminViewModel
    {
        [Required]
        public string User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
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

