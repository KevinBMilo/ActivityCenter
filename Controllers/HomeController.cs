using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Exam.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSessionData
        {
            get { return HttpContext.Session.GetInt32("UserId");}
            set { HttpContext.Session.SetInt32("UserId", (int)value);}
        }
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet("new")]
        public IActionResult NewAct()
        {
            return View("New");
        }

        [HttpGet("home")]
        public IActionResult GetAll()
        {
            var SessionId = (int) UserSessionData;
            ViewModel ActGroup = new ViewModel()
            {
                AllActs = dbContext.Acts
                .OrderBy(a => a.ActDate)
                .ToList(),
                AllUsers = dbContext.Users.ToList(),
                AllGoings = dbContext.Goings.ToList(),
                SessionData = SessionId
            };
            User oneUser = dbContext.Users
            .OrderBy(u => u.CreatedAt)
            .FirstOrDefault(User => User.UserId == (int) UserSessionData);
            ViewBag.UserId = oneUser;
            return View("Dashboard", ActGroup);
        }




        [HttpGet("activity/{ActId}")]
        public IActionResult ShowAct(int ActId)
        {
            var ActWithPeople = dbContext.Acts
                .Include(act => act.Goings)
                .ThenInclude(go => go.User)
                .Include(act => act.Creator)
                .FirstOrDefault(act => act.ActId == ActId);
            User oneUser = dbContext.Users
            .OrderBy(u => u.CreatedAt)
            .FirstOrDefault(User => User.UserId == (int) UserSessionData);
            ViewBag.UserId = oneUser;
            return View("Show", ActWithPeople);
        }

        [HttpPost("plan")]
        public IActionResult Plan(Act newAct)
        {
            if(ModelState.IsValid)
            {
                Act submittedAct = newAct;
                newAct.UserId = (int) UserSessionData;
                dbContext.Add(newAct);
                dbContext.SaveChanges();
                return RedirectToAction("ShowAct", newAct);
            }
            return View("New");
        }

        [HttpGet("go/{ActId}")]
        public IActionResult Attend(int ActId, Going newGoing)
        {
            Going subGoing = newGoing;
            newGoing.UserId = (int) UserSessionData;
            newGoing.ActId = (int) ActId;
            dbContext.Add(newGoing);
            dbContext.SaveChanges();
            return RedirectToAction("GetAll");
        }



        [HttpGet("leaving/{ActId}")]
        public IActionResult Leave(int ActId)
        {
            var SessionId = UserSessionData;
            Going notGoing = dbContext.Goings
                .SingleOrDefault(go => go.UserId == SessionId && go.ActId == ActId);
            dbContext.Goings.Remove(notGoing);
            dbContext.SaveChanges();
            return RedirectToAction ("GetAll");
        }

        [HttpGet("delete/{ActId}")]
        public IActionResult Delete(int ActId)
        {
            Act RetrievedAct = dbContext.Acts.FirstOrDefault(Act => Act.ActId == ActId);
            dbContext.Acts.Remove(RetrievedAct);
            dbContext.SaveChanges();
            return RedirectToAction ("GetAll");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            User submittedUser = newUser;
            if(dbContext.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "That email has been taken.");
                return View("Index");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                UserSessionData = newUser.UserId;
                return RedirectToAction("GetAll");
            }
            return View("Index");
        }

        [HttpPost("verify")]
        public IActionResult Verify(LoginUser userSubmission)
        {
            LoginUser submittedUser = userSubmission;
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email.");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Your current information does not match anything in our database.");
                    return View("Index");
                }
                UserSessionData = userInDb.UserId;
                return RedirectToAction("GetAll");
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
