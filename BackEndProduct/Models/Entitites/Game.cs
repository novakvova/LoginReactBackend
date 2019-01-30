using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BackEndProduct.Models.Entitites
{
    [Table("tblGames")]
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(maximumLength:255)]
        public string Title { get; set; }

        [StringLength(maximumLength: 255)]
        public string Image { get; set; }

        [Required,StringLength(maximumLength:1000)]
        public string Description { get; set; }

    }
}