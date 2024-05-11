using Recipe_Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace Recipe_Site.Controllers
{
    public class UsersController : Controller
    {
        ProjectContext db;
        IWebHostEnvironment env;
        static IConfiguration configuration;
        private const string EncryptionKey = "0123456789abcdef";

        public UsersController(ProjectContext _db, IWebHostEnvironment _env, IConfiguration _configuration)
        {
            db = _db;
            env = _env;
            configuration = _configuration;

        }
        [HttpGet]
        public IActionResult Login()
        {
            TempData["EmailID"] = null;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Users users)
        {
            try
            {
                var Lresult = await db.tblUsers.FirstOrDefaultAsync(p=>p.EmailId.Equals(users.EmailId));
                if (Lresult == null)
                {
                    ViewBag.Message = "EmailID doesn't exist!";
                    return View();
                }
                else if (Decrypt(Lresult.Password) != users.Password)
                {
                    ViewBag.Message = "Invalid User!Password didn't match";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetString("EmailID", Lresult.EmailId);
                    return RedirectToAction("Recipes","Recipe");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string emailid)
        {
            try
            {
                var user = await db.tblUsers.FirstOrDefaultAsync(p => p.EmailId.Equals(emailid));
                return View(user);
            }catch(Exception ex)
            {
                ViewBag.Exception = ex;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Users users)
        {
            try
            {
                var passwrd = await db.tblUsers.Where(p => p.EmailId.Equals(users.EmailId)).Select(p=>p.Password).FirstOrDefaultAsync();
                db.Entry(users).State = EntityState.Modified;
                if (users.ImageFile != null)
                {
                    var Imgpath = Path.Combine(env.WebRootPath, "ProfileImages", users.ProfilePic);
                    if (System.IO.File.Exists(Imgpath))
                    {
                        System.IO.File.Delete(Imgpath);
                    }
                    users.ProfilePic = UploadFile(users.ImageFile);
                }
                users.Password = passwrd;
                await db.SaveChangesAsync();
                return RedirectToAction("Welcome");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string emailid, int OTP)
        {
            var verifyemail = await db.tblUsers.Where(p => p.EmailId == emailid && p.OTP == OTP).FirstOrDefaultAsync();
            if (verifyemail != null)
            {
                TempData["EmailID"] = emailid;
                return RedirectToAction("ResetPassword");
            }
            TempData["Error"] = "Email/OTP didn't match!Please check again";
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Password, string Confirmpassword)
        {
            if (Password != Confirmpassword)
            {
                ModelState.AddModelError("Password", "Password and ConfirmPassword didn't match! Please try again");
                ViewBag.Message = "Password and ConfirmPassword didn't match! Please try again";
                return View();
            }
            var emailid = TempData["EmailID"];
            var verifypasswrd = await db.tblUsers.FirstOrDefaultAsync(p => p.EmailId == emailid);
            verifypasswrd.Password = Encrypt(Password);
            db.Update(verifypasswrd);
            await db.SaveChangesAsync();
            TempData["Success"] = "Password has been reset successfully!";
            TempData["EmailID"] = null;
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> SendOTP(string emailid)
        {
            try
            {
                var email = await db.tblUsers.FirstOrDefaultAsync(p => p.EmailId == emailid);
                if (emailid == null)
                {
                    ModelState.AddModelError("EmailId", "Email Cannot be empty!!");
                    TempData["Error"] = "Email Cannot be empty!!";
                    return RedirectToAction("ForgetPassword");
                }
                else if (email == null)
                {
                    ModelState.AddModelError("EmailId", "Email doesn't exist in our database!!");
                    TempData["Error"] = "Email doesn't exist in our database!!";
                    return RedirectToAction("ForgetPassword");
                }
                int OTP = GenerateOTP();
                SendMail(emailid, OTP);
                var updateotp = await db.tblUsers.FirstOrDefaultAsync(p => p.EmailId == emailid);
                updateotp.OTP = OTP;
                db.Update(updateotp);
                await db.SaveChangesAsync();
                TempData["OTP"] = OTP;
                TempData["EmailID"] = emailid;
                TempData["Success"] = "Mail has been successfully sent to your registered email!";
                return RedirectToAction("ForgetPassword");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("ForgetPassword");
            }
        }
        public async Task<IActionResult> Welcome()
        {
            try
            {
                var emailid = HttpContext.Session.GetString("EmailID");
                ViewBag.EmailId = emailid;
                if (!string.IsNullOrEmpty(emailid))
                {
                    var details = await db.tblUsers.FirstOrDefaultAsync(p => p.EmailId == emailid);
                    return View(details);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(Users users)
        {
            try
            {
                var emailcheck = await db.tblUsers.Where(p => p.EmailId == users.EmailId).FirstOrDefaultAsync();
                if (emailcheck != null)
                {
                    ModelState.AddModelError("EmailId", "Email ID already exists");
                }
                if (users != null)
                {
                    db.Entry(users).State = EntityState.Modified;
                    if (users.ImageFile != null)
                    {
                        users.ProfilePic = UploadFile(users.ImageFile);
                    }
                    users.Password = Encrypt(users.Password);
                    await db.tblUsers.AddAsync(users);
                    await db.SaveChangesAsync();
                    HttpContext.Session.SetString("EmailID", users.EmailId);
                    return RedirectToAction("Recipes", "Recipe");
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                return View();
            }
        }
        string UploadFile(IFormFile obj)
        {
            string fileName = "";
            if (obj != null && obj.ContentType.ToLower().StartsWith("image"))
            {
                string uploadDir = Path.Combine(env.WebRootPath, "ProfileImages");
                fileName = Guid.NewGuid().ToString() + "_" + obj.FileName;
                string path = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    obj.CopyTo(fileStream);
                }
                return fileName;
            }
            return fileName;
        }
        protected int GenerateOTP()
        {
            string characters = "1234567890";
            string otp = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return Convert.ToInt32(otp);
        }
        protected void SendMail(string ToMailAddress, int OTP)
        {
            string? FromMailAddress = configuration["KeySettings:FromEmail"];
            string? passkey = configuration["KeySettings:EmailPass"];
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(FromMailAddress);
            message.To.Add(new MailAddress(ToMailAddress));
            message.Subject = "OTP Activation for Forget Password";
            message.IsBodyHtml = true; //to make message body as html

            StringBuilder str = new StringBuilder();
            str.Append("Hi, \n");
            str.Append("Your Activation OTP is:" + OTP);

            message.Body = Convert.ToString(str);
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com"; //for gmail host
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(FromMailAddress, passkey);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                smtp.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }
        protected static string Encrypt(string text)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; // IV is typically randomly generated for security, but for simplicity, we use a fixed IV here

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }
                    //var x = Convert.ToBase64String(msEncrypt.ToArray());
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        protected static string Decrypt(string text)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; // IV is typically randomly generated for security, but for simplicity, we use a fixed IV here

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            //var x = srDecrypt.ReadToEnd();
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
