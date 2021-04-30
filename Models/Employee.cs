using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
	public class Employee
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("ID")]
        public string Employee_ID { get; set; }

        [DisplayName("First name")]
        public string Employee_Name { get; set; }

        [DisplayName("Image")]
        public byte[] Employee_Image { get; set; }

        [DisplayName("Last name ")]
        public string Employee_Surname { get; set; }

        [DisplayName("ID Number")]
        public string Employee_IDNo { get; set; }

        [DataType(DataType.PhoneNumber)]

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                  ErrorMessage = "Entered Contact number format is not valid.")]
        public string Employee_CellNo { get; set; }

        [DisplayName("Residence address")]
        public string Employee_Address { get; set; }

        [DisplayName("Email address")]
        [EmailAddress]
        public string Employee_Email { get; set; }
        public string Employee_Pass { get;set; }

        public virtual List<Manifest> Manifest { get; set; }
       
    }
}