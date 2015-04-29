using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using TimeMachine.Models;
using TimeMachine.BaseCode;
using RestSharp;

namespace TimeMachine.Controllers
{
    [Authorize]
    public class GroupExpenseController : Controller
    {
        ApplicationDbContext dbContext = null;

        public GroupExpenseController()
        {
            dbContext = new ApplicationDbContext();
        }

        // GET: GroupExpense
        public ActionResult Index()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            // Find GroupId of particular user
            var GroupIds = dbContext.TbGroupUser.Where(
                t => t.UserId.Equals(
                    dbContext.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name)).Id
                    )
                ).Select(t => t.GroupId);

            // Find GroupNames of particular user
            foreach (int grpId in GroupIds)
            {
                list.Add(
                    new SelectListItem()
                    {
                        Text = dbContext.TbGroup.FirstOrDefault(t => t.GroupId.Equals(grpId)).GroupName,
                        Value = grpId.ToString()
                    });
            };

            GroupIndexViewModel model = new GroupIndexViewModel()
            {
                GroupsList = list,
                CreateGroupVM = new CreateGroupViewModel()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateGroup(CreateGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Making the Group
                Group GroupRow = new Group();
                GroupRow.GroupName = model.GroupName;
                GroupRow.StartAmount = model.Amount;
                GroupRow.AmountLeft = model.Amount;
                GroupRow.StartDate = Convert.ToDateTime(model.StartDate, new CultureInfo("es-ES"));
                GroupRow.EndDate = Convert.ToDateTime(model.EndDate, new CultureInfo("es-ES"));
                GroupRow.LastUpdated = DateTime.Now;
                dbContext.TbGroup.Add(GroupRow);
                dbContext.SaveChanges();

                int NewGroupId = GroupRow.GroupId;

                //Add Data in Group user table
                GroupUser GroupUserRow = new GroupUser();
                GroupUserRow.GroupId = NewGroupId;
                GroupUserRow.UserId = dbContext.Users.FirstOrDefault(t => t.UserName.Equals(User.Identity.Name)).Id;
                GroupUserRow.Role = 1; //For Admin
                dbContext.TbGroupUser.Add(GroupUserRow);
                dbContext.SaveChanges();

                //Save GroupId in Session
                TempData["GroupId"] = NewGroupId;

                //Save GroupId in Session
                //if (Session["GroupId"] == null)
                //{
                //    Session["GroupId"] = NewGroupId;
                //}

                return RedirectToAction("GroupInfo");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult GroupInfo(int GroupId)
        {
            IList<SelectListItem> categoryList = new List<SelectListItem>();
            categoryList.Add(new SelectListItem(){
                Text = "Food",
                Value = "1",
                Selected = true
            });
            categoryList.Add(new SelectListItem(){
                Text = "Shopping",
                Value = "2"
            });
            categoryList.Add(new SelectListItem(){
                Text = "General",
                Value = "3"
            });

            if (TempData["GroupId"] == null)
            {
                TempData["GroupId"] = GroupId;
            }

            var userIds = dbContext.TbGroupUser.Where(t => t.GroupId.Equals(GroupId)).Select(t => t.UserId);
            List<SelectListItem> usersList = new List<SelectListItem>();

            foreach(string id in userIds)
            {
                usersList.Add(new SelectListItem()
                {
                    Text = dbContext.Users.Find(id).FirstName,
                    Value = id
                });
            }

            GroupInfoViewModel GroupTransactionVM = new GroupInfoViewModel()
            {
                CreateGroupTransactionVM = new GroupTransactionViewModel()
                {
                    CategoryList = categoryList,
                    GroupUsers = usersList
                },
                GroupInvitationForm = new GroupInvitation(),
                GroupUsersList = usersList,
                GroupTransactionHistoryDateVM = new GroupTransactionHistoryDateViewModel()
            };

            return View(GroupTransactionVM);
        }

        [HttpPost]
        public ActionResult SaveGroupTransaction(GroupTransactionViewModel model)
        {
            GroupTransaction transactionData = null;
            int GroupId = Convert.ToInt32(TempData.Peek("GroupId"));
            if (ModelState.IsValid)
            {
                transactionData = new GroupTransaction()
                {
                    Id = Guid.NewGuid(),
                    GroupId = GroupId,
                    Date = Convert.ToDateTime(model.Date),
                    ItemName = model.ItemName,
                    ItemPrice = model.ItemPrice,
                    ItemType = model.ItemType,
                    Quantity = model.Quantity,
                    LastUpdatedDate = DateTime.Now,
                    SpentByUserId = model.SpentUser.ToString(),
                    UpdatedByUserId = dbContext.Users.FirstOrDefault(t => t.UserName.Equals(User.Identity.Name)).Id,
                    Comments = model.Comments,
                    NewTransactionId = String.Empty
                };
                dbContext.TbGroupTransactions.Add(transactionData);
                dbContext.SaveChanges();
            }
            return RedirectToAction("GroupInfo", new { GroupId = GroupId });
        }

        [HttpGet]
        public ActionResult ViewGroupTransactionHistory()
        {
            return View("GroupTransactionHistoryDatePage");
            //GroupTransactionHistoryViewModel ViewGrpTransHistoryVM = null;
            //int groupId = Convert.ToInt32(TempData.Peek("GroupId"));

            ////Getting Data from Transaction Table
            //var transactionHistory = dbContext.TbGroupTransactions.Where(t => t.GroupId.Equals(groupId)).OrderByDescending(t => t.LastUpdatedDate);
            //List<ViewGroupTransactionViewModel> ViewGrpTransVM = new List<ViewGroupTransactionViewModel>();
            //ViewGroupTransactionViewModel viewModel = null;

            ////Adding rows to transaction history view model 
            //foreach (var row in transactionHistory)
            //{
            //    viewModel = new ViewGroupTransactionViewModel();
            //    viewModel.Date = row.Date;
            //    viewModel.Id = row.Id;
            //    viewModel.ItemName = row.ItemName;
            //    viewModel.ItemPrice = row.ItemPrice;
            //    viewModel.Quantity = row.Quantity;
            //    viewModel.Comments = row.Comments;
            //    viewModel.SpentByUser = dbContext.Users.Find(row.SpentByUserId).FirstName + " " + dbContext.Users.Find(row.SpentByUserId).LastName;
            //    ViewGrpTransVM.Add(viewModel);
            //}

            //ViewGrpTransHistoryVM = new GroupTransactionHistoryViewModel()
            //{
            //    GroupName = dbContext.TbGroup.Find(groupId).GroupName,
            //    ViewTransactionHistoryVM = ViewGrpTransVM
            //};

            //return View(ViewGrpTransHistoryVM);
        } 

        public static void SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                   new HttpBasicAuthenticator("api",
                                              "key-6de7390c50abada1b301c00207a3be35");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                "sandboxa39a1e82a3b14174a1eb4c8ccd6b3082.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa39a1e82a3b14174a1eb4c8ccd6b3082.mailgun.org>");
            request.AddParameter("to", "Ambuj Sharma <ambujsharma23@gmail.com>");
            request.AddParameter("subject", "Hello Ambuj Sharma");
            request.AddParameter("text", "Congratulations Ambuj Sharma, you just sent an email with Mailgun!  You are truly awesome!  You can see a record of this email in your logs: https://mailgun.com/cp/log .  You can send up to 300 emails/day from this sandbox server.  Next, you should add your own domain so you can send 10,000 emails/month for free.");
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}