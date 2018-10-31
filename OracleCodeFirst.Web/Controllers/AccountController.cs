using DATA;
using Domain;
using Newtonsoft.Json;
using OracleCodeFirst.Web.CustomAuthentication;
using OracleCodeFirst.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OracleCodeFirst.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView loginView, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(loginView.UserName, loginView.Password))
                {
                    var user = (CustomMembershipUser)Membership.GetUser(loginView.UserName, false);
                    if (user != null)
                    {
                        CustomSerializeModel userModel = new Models.CustomSerializeModel()
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            RoleName = user.Roles.Select(r => r.RoleName).ToList()
                        };

                        string userData = JsonConvert.SerializeObject(userModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                            (
                            1, loginView.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
                            );

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("Cookie1", enTicket);
                        Response.Cookies.Add(faCookie);
                    }

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            ModelState.AddModelError("", "Something Wrong : Username or Password invalid ^_^ ");
            return View(loginView);
        }


        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationView registrationView)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;
            string activationCode = null;
            if (ModelState.IsValid)
            {
                // Email Verification
                string userName = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
                    return View(registrationView);
                }

                //Save User Data 
                using (OeContext dbContext = new OeContext())
                {
                    var user = new User()
                    {
                        Username = registrationView.Username,
                        FirstName = registrationView.FirstName,
                        LastName = registrationView.LastName,
                        Email = registrationView.Email,
                        Password = registrationView.Password,
                        ActivationCode = Guid.NewGuid().ToString(),                        
                    };

                    activationCode = user.ActivationCode.ToString();
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }

                //Verification Email
                VerificationEmail(registrationView.Email, activationCode);
                messageRegistration = "Your account has been created successfully. ^_^";
                statusRegistration = true;
            }
            else
            {
                messageRegistration = "Something Wrong!";
            }
            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;

            return View(registrationView);
        }
        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;
            using (OeContext dbContext = new OeContext())
            {
                //var usrAcc = dbContext.Users.FirstOrDefault(u => u.ActivationCode == "ecd089bc-edd3-40a7-8b73-d708be9267fb");
                var userAccount = dbContext.Users.FirstOrDefault(u => u.ActivationCode.Equals(id));

                if (userAccount != null)
                {
                    userAccount.IsActive = true;
                    dbContext.SaveChanges();
                    statusAccount = true;
                }
                else
                {
                    ViewBag.Message = "Something Wrong !!";
                }

            }
            ViewBag.Status = statusAccount;
            return View();
        }
        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie("Cookie1", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }






        //[NonAction]
        //public void VerificationEmail1(string email, string activationCode)
        //{
        //    var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
        //    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

        //    var fromEmail = new MailAddress("nksolutions.ltd@gmail.com", "Activation Account - AKKA");
        //    var toEmail = new MailAddress(email);

        //    var fromEmailPassword = "09115438SHARDUL";
        //    string subject = "Activation Account !";

        //    string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        //    };

        //    using (var message = new MailMessage(fromEmail, toEmail)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true

        //    })
        //        smtp.Send(message);
        //}


        //[NonAction]
        //public void VerificationEmail2(string email, string activationCode)
        //{
        //    var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
        //    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);
                        
        //    string subject = "Activation Account !";

        //    string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";


        //    var client = new SmtpClient("smtp.gmail.com", 587)
        //    {
        //        Credentials = new NetworkCredential("nksolutionsltd@gmail.com", "09115438SHARDUL"),
        //        EnableSsl = true
        //    };

        //    client.Send("nksolutionsltd@gmail.com", email, subject, body);
        //}


        //Email Service
        [NonAction]
        public void VerificationEmail(string email, string activationCode)
        {
            using (MailMessage mm = new MailMessage("bbs.govt.bd@gmail.com", email))
            {
                mm.Subject = "Account Activation Link from BBS for " + email;
                string body = "Hello " + email + ",";
                body += "<br /><br />Please click the following link to activate your account";
                body += "<br /><a href = '" + string.Format("{0}://{1}/Account/ActivationAccount/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />Thanks";
                mm.Body = body;

                SendMail(mm);
            }
        }

        [NonAction]
        private void SendMail(MailMessage mm)
        {
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("nksolutionsltd@gmail.com", "09115438SHARDUL");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mm);
        }

    }
}