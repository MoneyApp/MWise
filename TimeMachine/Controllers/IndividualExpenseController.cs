using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeMachine.Models;

namespace TimeMachine.Controllers
{
    [Authorize]
    public class IndividualExpenseController : Controller
    {
        ApplicationDbContext dbContext = null;

        public IndividualExpenseController()
        {
            dbContext = new ApplicationDbContext();
        }

        // GET: IndividualExpense
        public ActionResult Index()
        {
            TempData["HeaderText"] = "Record Your Expenditure";
            TempData["IndividualExpenseActionType"] = "INSERT";

            IndividualExpenseDetailViewModel IndividualExpenseDetailVM = new IndividualExpenseDetailViewModel()
            {
                IndividualExpenseFormModel = new IndividualExpenseModel(),
                IndividualExpenseHistoryModel = dbContext.TbIndividualExpense.ToList().Where(t=>t.UserName == User.Identity.Name)
                                                         .OrderByDescending(t=>t.Date).OrderByDescending(t=>t.LastUpdated),
                CategoryList = GetCategoryList()
            };

            return View(IndividualExpenseDetailVM);
        }

        [HttpPost]
        public ActionResult SaveExpenseForm(IndividualExpenseDetailViewModel IndExpVM)
        {
            IndividualExpenseModel model = new IndividualExpenseModel();
            model.Id = IndExpVM.IndividualExpenseFormModel.Id;
            model.Date = IndExpVM.IndividualExpenseFormModel.Date;
            model.Category = IndExpVM.IndividualExpenseFormModel.Category;
            model.ItemName = IndExpVM.IndividualExpenseFormModel.ItemName;
            model.ItemPrice = IndExpVM.IndividualExpenseFormModel.ItemPrice;
            model.Notes = IndExpVM.IndividualExpenseFormModel.Notes;
            model.LastUpdated = DateTime.Now;
            model.UserName = User.Identity.Name;
            
            if(ModelState.IsValid)
            {
                if (TempData["IndividualExpenseActionType"].ToString() == "INSERT")
                {
                    dbContext.TbIndividualExpense.Add(model);
                    dbContext.SaveChanges();
                }
                else if (TempData["IndividualExpenseActionType"].ToString() == "UPDATE")
                {
                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult EditHistoryItem(int Id)
        {
            TempData["HeaderText"] = "Edit History Item";
            TempData["IndividualExpenseActionType"] = "UPDATE";

            IndividualExpenseDetailViewModel IndividualExpenseDetailVM = new IndividualExpenseDetailViewModel()
            {
                IndividualExpenseFormModel = dbContext.TbIndividualExpense.Find(Id),
                IndividualExpenseHistoryModel = null,
                CategoryList = GetCategoryList()
            };
            return View("EditHistoryItem",IndividualExpenseDetailVM);
        }

        [HttpGet]
        public ActionResult DeleteHistoryItem(int Id)
        {
            var model = dbContext.TbIndividualExpense.Find(Id);
            dbContext.TbIndividualExpense.Remove(model);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        private IList<SelectListItem> GetCategoryList()
        {
            IList<SelectListItem> CategoryList = new List<SelectListItem>();
            CategoryList.Add(new SelectListItem
            {
                Text = "Food",
                Value = "1",
                Selected = true,
            });
            CategoryList.Add(new SelectListItem
            {
                Text = "Household",
                Value = "2"
            });

            return CategoryList;
        }
    }
}