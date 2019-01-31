using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BackEndProduct.ViewModels
{
    public class GameItemViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string description { get; set; }
    }

    public class GameCreateViewModel
    {
        [Required(ErrorMessage = "Поле є обов'язковим")]
        public string title { get; set; }

        [Required(ErrorMessage = "Поле є обов'язковим")]
        public string image { get; set; }

        [Required(ErrorMessage = "Поле є обов'язковим")]
        //[EmailAddress(ErrorMessage ="Лох вкажи емайл")]
        //[RegularExpression("")]
        public string description { get; set; }
    }
}