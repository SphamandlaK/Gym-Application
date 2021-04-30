using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class CommentsRating
    {
       
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int CommentId { get; set; }
            public string Comments { get; set; }
            public DateTime? ThisDateTime { get; set; }
            public int ArticleId { get; set; }
            public int? Rating { get; set; }
           // [Key, ForeignKey("GymMember")]
           [ForeignKey("GymMember")]
            public int Member_Id { get; set; }
            public virtual GymMember GymMember { get; set; }
    }
}