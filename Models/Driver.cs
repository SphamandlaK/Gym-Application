using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class Driver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("ID")]
        public string Driver_ID { get; set; }

        [DisplayName("First name")]
        public string Driver_Name { get; set; }

        [DisplayName("Image")]
        public byte[] Driver_Image { get; set; }

        [DisplayName("Last name ")]
        public string Driver_Surname { get; set; }

        [DisplayName("ID Number")]
        public string Driver_IDNo { get; set; }

        [DataType(DataType.PhoneNumber)]

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                  ErrorMessage = "Entered Contact number format is not valid.")]
        public string Driver_CellNo { get; set; }

        [DisplayName("Residence address")]
        public string Driver_Address { get; set; }

        [DisplayName("Email address")]
        [EmailAddress]
        public string Driver_Email { get; set; }

        public virtual List<Manifest> Manifest { get; set; }
        public string Driver_Pass { get; set; }
    }
}