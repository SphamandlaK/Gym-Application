//using SE_Decor_DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApplication.Models.EquipmentBooking
{
    public class Book_Item_Equipment
    {
        [Key]
        public int ItemId { get; set; }
        public string userId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }

        [DisplayName("Item Name")]
        public string ItemName { get; set; }

        [DisplayName("Item Description")]
        public string Description { get; set; }
        public int? ItemCode { get; set; }
        public virtual Item Item { get; set; }
        public int? Category_ID { get; set; }
        public virtual Category Category { get; set; }

        [DisplayName("Rent-Date-From"), DataType(DataType.Date)]
        public DateTime Date_From { get; set; }
        [DisplayName("Return-Date"), DataType(DataType.Date)]
        public DateTime Date_To { get; set; }
        [DisplayName("Item Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [DisplayName("Item Image")]
        public byte[] Picture { get; set; }
        [Display(Name = "Booking Cost")]
        [DataType(DataType.Currency)]
        public double Booking_Cost { get; set; }
        [DataType(DataType.Currency)]
        public double Deposit { get; set; }
        public int numOfDays { get; set; }
        public string status { get; set; } = "Pending";
        public decimal CalcNum_of_Days(Book_Item_Equipment equipment)
        {

            TimeSpan difference = equipment.Date_To.Subtract(equipment.Date_From);
            var Days = difference.TotalDays;
            return Convert.ToDecimal(Days);
        }

    }
}
