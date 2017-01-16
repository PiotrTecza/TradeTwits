using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TradeTwits.Data.Models;
using TradeTwits.Models;
using System.Linq.Expressions;
using TradeTwits.Data.Services;

namespace TradeTwits.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly IQueryable<ApplicationUser> users;
        private readonly IQueryable<TwitModel> twits;
        private readonly IQueryable<StockDataModel> stocksData;

        public HomeController(
            IUserService userService, 
            IQueryable<ApplicationUser> users, 
            IQueryable<TwitModel> twits, 
            IQueryable<StockDataModel> stocksData)
        {
            this.userService = userService;
            this.users = users;
            this.twits = twits;
            this.stocksData = stocksData;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Blocked()
        {
            List<string> blockedIds = userService.GetUser(User.Identity.Name).BlockedIds;
            Expression<Func<ApplicationUser, bool>> whereCriteria = u => blockedIds.Contains(u.UserName);
            var blocked = users.Where(whereCriteria);
            var result = blocked.Select(u => new BlockedViewModel(u.UserName, u.GravatarHash));
            return View(result);
        }

        [HttpGet]
        public ActionResult PersonalInfo()
        {
            ApplicationUser user = userService.GetUser(User.Identity.Name);
            PersonalInfoViewModel model = new PersonalInfoViewModel()
            {
                FullName = user.FullName,
                Description = user.Description,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PersonalInfo(PersonalInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = userService.GetUser(User.Identity.Name);
                user.FullName = model.FullName;
                user.Description = model.Description;
                userService.Save(user);
            }
            
            return View(model);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            ApplicationUser user = userService.GetUser(id);
            int followedUsers = users.Where(u => user.FollowedIds.Contains(u.UserName) && u.IsUser == true).Count();
            bool isFollowed = users.Where(u => u.UserName == User.Identity.Name && u.FollowedIds.Contains(id)).Any();
            int followersCount = users.Where(u => u.FollowedIds.Contains(id)).Count();
            int twitCount = twits.Where(t => t.Tags.Contains(id)).Count();
            int followedStocks = user.FollowedIds.Count - followedUsers;
            bool isBlocked = users.Where(u => u.UserName == User.Identity.Name && u.BlockedIds.Contains(id)).Any();

            if (user.IsUser)
            {
                var userVM = new UserViewModel(user.UserName, user.EmailAddress, isFollowed, isBlocked, 
                    followedUsers, followedStocks, followersCount, twitCount, user.ReputationPoints);
                return View(userVM);
            }
            else
            {
                StockDataModel stockData = stocksData.Where(t => t.Symbol.Contains(id)).FirstOrDefault();
                if (stockData != null)
                {
                    StockViewModel stockDTO = new StockViewModel(user.Category, user.Name, user.UserName, user.FullName, isFollowed, followersCount, twitCount, stockData.Price, stockData.Change, stockData.ChangeAmount);
                    return View("StockDetails", stockDTO);
                }
                else
                {
                    StockViewModel stockDTO = new StockViewModel(user.Category, user.Name, user.UserName, user.FullName, isFollowed, followersCount, twitCount);
                    return View("StockDetails", stockDTO);
                }
            }

        }
    }
}