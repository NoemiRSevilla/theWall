using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using theWall.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using theWall;

// Other using statements
namespace theWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                return RedirectToAction("Wall");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("submit")]
        public IActionResult Submit(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.users.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                else
                {
                    DateTime today = DateTime.Today;
                    int years = today.Year - newUser.DOB.Year;
                    if (years > 18)
                    {
                        PasswordHasher<User> Hasher = new PasswordHasher<User>();
                        newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                        dbContext.Add(newUser);
                        dbContext.SaveChanges();
                        HttpContext.Session.SetString("Email", newUser.Email);
                        return Redirect($"/wall");
                    }
                    else
                    {
                        ModelState.AddModelError("DOB", "Chef has to be over 18 to register");
                        return View("Index");
                    }

                }
            }
            else
            {
                TempData["First Name"] = newUser.FirstName;
                TempData["Last Name"] = newUser.LastName;
                TempData["Email"] = newUser.Email;
                return View("Index");
            }
        }

        [HttpPost("submitlogin")]
        public IActionResult submitlogin(LoginUser retrievedUser)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == retrievedUser.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "You could not be logged in");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in dbcopy
                var result = hasher.VerifyHashedPassword(retrievedUser, userInDb.Password, retrievedUser.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "You could not be logged in");
                    return View("Index");
                }
                HttpContext.Session.SetString("Email", retrievedUser.LoginEmail);
                return Redirect("/wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet("wall")]
        public IActionResult Wall()
        {
            string userInSession = HttpContext.Session.GetString("Email");
            if (userInSession != null)
            {
                User retrievedUser = dbContext.users
                .FirstOrDefault(u => u.Email == userInSession);

                List<Message> AllMessages = dbContext.messages
                .Include(u => u.Creator)
                .Include( u => u.theComments)
                .ThenInclude( c => c.Creator)
                .OrderBy(c => c.CreatedAt)
                .ToList();

                ViewBag.current = retrievedUser;
                return View(AllMessages);
            }
            else
            {
                return Redirect("/logout");
            }
        }

        [HttpPost("postmessage")]
        public IActionResult postMessage(Message newMessage)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(newMessage);
                dbContext.SaveChanges();
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Wall");
            }
        }

        [HttpPost("postComment")]
        public IActionResult postComment(Comment newComment)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(newComment);
                dbContext.SaveChanges();
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Wall");
            }
        }
    }
}