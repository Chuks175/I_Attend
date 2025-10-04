#nullable disable

using System.ComponentModel.DataAnnotations;

namespace I_Attend
{
    [MetadataType(typeof(Custom_LabelMetaData))]
    public partial class Custom_Label
    {
        public int UserNames;
        public int Matric_Number;
        public int department;
        public int ImageData;
        public int Course_code;
        public int Password;
        public int Email;
        public void CustomLabel() { }

        //public static void Main()
        //{
        //    var Custom = new Custom_Label();
        //    Custom.CustomLabel();
        //}
    }

    public class Custom_LabelMetaData
    {
        [Required]
        [DataType(DataType.MultilineText)]
        //[ScaffoldColumn(false)]
        public object UserNames { get; set; }
        [Required]
        [DataType(DataType.Custom)]
        //[ScaffoldColumn(false)]
        public object Matric_Number { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        //[ScaffoldColumn(false)]
        public object department { get; set; }

        [Required]
        [DataType(DataType.Custom)]
        //[ScaffoldColumn(false)]
        public object ImageData { get; set; }

        [Required]
        [DataType(DataType.Custom)]
        public object Course_code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public object Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public object Email { get; set; }

    }
}
