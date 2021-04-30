using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
	public class Comment
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public DateTime? ThisDateTime { get; set; }

        public string Driver_ID { get; set; }

        public int? Rating { get; set; }
    }
}