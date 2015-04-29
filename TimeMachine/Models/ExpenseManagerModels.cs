using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeMachine.Models
{
    public class IndividualExpenseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date")]
        public string Date { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required]
        [Display(Name = "Item Price")]
        [DataType(DataType.Currency)]
        public string ItemPrice { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public DateTime LastUpdated { get; set; }

        public string UserName { get; set; }
    }

    public class IndividualExpenseDetailViewModel
    {
        public IndividualExpenseModel IndividualExpenseFormModel { get; set; }
        public IEnumerable<IndividualExpenseModel> IndividualExpenseHistoryModel { get; set; }
        public IList<SelectListItem> CategoryList { get; set; }
    }
}