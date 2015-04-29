using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeMachine.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double StartAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double AmountLeft { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GroupUser> GroupUsers { get; set; }
        public virtual ICollection<GroupInvitation> GroupInvitations { get; set; }
        public virtual ICollection<GroupTransaction> GroupTransaction { get; set; }

    }

    public class GroupUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int Role { get; set; } //Admin - 1 Else 0

        [ForeignKey("GroupId")]
        public virtual ICollection<Group> Groups { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }

    public class GroupInvitation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SentFromUserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public int Status { get; set; } // Pending-0, Accepted-1

        [ForeignKey("GroupId")]
        public virtual ICollection<Group> Groups { get; set; }

        [ForeignKey("SentFromUserId")]
        public virtual ICollection<ApplicationUser> GroupInvitationUsers { get; set; }
    }

    public class GroupTransaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string SpentByUserId { get; set; }

        public string UpdatedByUserId { get; set; }

        [Required]
        public int ItemType { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public int ItemPrice { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdatedDate { get; set; }

        [Required]
        public string Comments { get; set; }

        public string NewTransactionId { get; set; }

        [ForeignKey("GroupId")]
        public virtual ICollection<Group> Group { get; set; }

        //[ForeignKey("SpentByUserId")]
        [InverseProperty("UsersThatSpent")]
        public virtual ApplicationUser SpentByUser { get; set; }

        //[ForeignKey("UpdatedByUserId")]
        [InverseProperty("UsersThatUpdated")]
        public virtual ApplicationUser UpdatedByUser { get; set; }
    }

    //******************  END OF DATABASE TABLES  **********************//

    //******************  Group View-Models       **********************//

    public class CreateGroupViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        public double Amount { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
    }

    public class GroupTransactionViewModel
    {
        [Required]
        [Display(Name="Date")]
        public string Date { get; set; }

        [Required]
        [Display(Name="Category")]
        public int ItemType { get; set; }

        [Required]
        [Display(Name="Item Name")]
        public string ItemName { get; set; }

        [Required]
        [Display(Name="Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name="Total Amount")]
        public int ItemPrice { get; set; }

        [Display(Name="Notes")]
        public string Comments { get; set; }

        [Required]
        [Display(Name="Who Spent")]
        public string SpentUser { get; set; }

        public IList<SelectListItem> CategoryList { get; set; }

        public IList<SelectListItem> GroupUsers { get; set; }
    }

    public class ViewGroupTransactionViewModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public int ItemPrice { get; set; }

        public string Comments { get; set; }

        public string SpentByUser { get; set; }
    }

    public class GroupIndexViewModel
    {
        public List<SelectListItem> GroupsList { get; set; }
        public CreateGroupViewModel CreateGroupVM { get; set; }
    }

    public class GroupInfoViewModel
    {
        public GroupTransactionViewModel CreateGroupTransactionVM { get; set; }
        public GroupInvitation GroupInvitationForm { get; set; }
        public List<SelectListItem> GroupUsersList { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name="Enter EmailId")]
        public string EmailId { get; set; }
        public GroupTransactionHistoryDateViewModel GroupTransactionHistoryDateVM { get; set; }
    }

    public class GroupTransactionHistoryViewModel
    {
        public string GroupName { get; set; }
        public IEnumerable<ViewGroupTransactionViewModel> ViewTransactionHistoryVM { get; set; }
        //public IEnumerable<GroupTransaction> ViewTransactionHistoryVM { get; set; }
    }

    public class GroupTransactionHistoryDateViewModel
    {
        [Display(Name="Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name="End Date")]
        public DateTime? EndDate { get; set; }
    }
}