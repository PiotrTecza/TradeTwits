using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TradeTwits.Data.Models;
using TradeTwits.Utils;
using System.Linq.Expressions;
using TradeTwits.Models;
using TradeTwits.Data.Services;

namespace TradeTwits.Controllers
{
    public class SearchController : ApiController
    {
        private readonly IUserService userService;
        private readonly IQueryable<ApplicationUser> users;

        public SearchController(
            IUserService userService, 
            IQueryable<ApplicationUser> users)
        {
            this.userService = userService;
            this.users = users;
        }

        // GET: Stocks  and users
        [HttpGet]
        public IEnumerable<ApplicationUser> Get(string q)
        {
            return userService.GetByNameMatches(q);
        }

        [HttpGet]
        public IEnumerable<SearchViewModel> GetSearchResults(string name)
        {
            var users = userService.GetByNameMatches(name).Where(u => u.IsUser == true).OrderBy(u => u.UserName).Take(5);
            var stocks = userService.GetByNameMatches(name).Where(u => u.IsUser != true).OrderBy(u => u.UserName).Take(5);
            IEnumerable<SearchViewModel> result = PrepareSearchVM(users, stocks);
            return result;
        }

        [HttpGet]
        public dynamic GetFollowedUsers(string id, string lastName = "")
        {
            return GetFollowed(id, lastName, true);
        }

        [HttpGet]
        public dynamic GetFollowedStocks(string id, string lastName = "")
        {
            return GetFollowed(id, lastName, false);
        }

        [HttpGet]
        public dynamic GetFollowers(string id, string lastName = "")
        {
            var query = users.Where(u => u.FollowedIds.Contains(id));
            var followers = query.AsEnumerable();
            var count = query.Count();

            if (!string.IsNullOrEmpty(lastName))
            {
                followers = GetNextUsers(lastName, u => u.FollowedIds.Contains(id));
            }
            var followedResult = FormatUsers(followers);
            return new { data = followedResult, count = count };
        }

        private dynamic GetFollowed(string id, string lastName, bool isUser)
        {
            var followedIds = userService.GetUser(id).FollowedIds;
            if (!followedIds.Any())
                return new { allLoaded = true };

            Expression<Func<ApplicationUser, bool>> whereCriteria;
            if (isUser)
                whereCriteria = u => followedIds.Contains(u.UserName) && u.IsUser == true;
            else
                whereCriteria = u => followedIds.Contains(u.UserName) && u.IsUser != true;

            var query = users.Where(whereCriteria);
            var followed = query.AsEnumerable();
            int count = followed.Count();

            if (!string.IsNullOrEmpty(lastName))
            {
                followed = GetNextUsers(lastName, whereCriteria);
            }
            var followedResult = FormatUsers(followed);
            return new { data = followedResult, count = count };
        }

        private IEnumerable<ApplicationUser> GetNextUsers(string lastName, Expression<Func<ApplicationUser, bool>> whereCriteria)
        {
            return userService.GetNextUsers(lastName, whereCriteria);
        }

        private object FormatUsers(IEnumerable<ApplicationUser> users)
        {
            return users.Select(x => new { UserName = x.UserName, GravatarHash = GravatarGenerator.GenerateHash(x.EmailAddress) });
        }

        private IEnumerable<SearchViewModel> PrepareSearchVM(IEnumerable<ApplicationUser> users, IEnumerable<ApplicationUser> stocks)
        {
            var usersVM = users.Select(u => new SearchViewModel(u.UserName, u.FullName, u.GravatarHash)).ToList();
            var stockVM = stocks.Select(s => new SearchViewModel(s.UserName, s.FullName)).ToList();

            if (stockVM.Any())
                stockVM[0].IsFirstStock = true;

            return usersVM.Concat(stockVM);
        }
    }
}