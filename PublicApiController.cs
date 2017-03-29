using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using adr.Web.Domain;
using adr.Web.Enums;
using adr.Web.Models;
using adr.Web.Models.Requests;
using adr.Web.Models.Requests.company;
using adr.Web.Models.Requests.Uploads;
using adr.Web.Models.Requests.User;
using adr.Web.Models.Responses;
using adr.Web.Services;
using adr.Web.Services.Interfaces;
using adr.Web.Services.S3Service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace adr.Web.Controllers.Api
{
    [RoutePrefix("api/public")]
    public class PublicApiController : ApiController
    {
        //....// ====================================== DEPENDENCY INJECTION START ===================================


        [Dependency]
        public ITokenService _TokenService { get; set; }
        public ICompanyService _CompanyOptionService { get; set; }

        [Dependency]
        public IUserEmailService _UserEmailService { get; set; }

        [Dependency]
        public IAdminService _AdminService { get; set; }

        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }


        //....// ====================================== DEPENDENCY INJECTION END ====================================


        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // User Registration
        [HttpPost]
        [Route("registration")]
        public HttpResponseMessage Register(UserAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            bool success = false;

            IdentityUser newUser = null;

            // Use model input to build user Account, company, and profile in DB
            try
            {
                // Create User Account
                newUser = UserService.CreateUser(model.Email, model.Password);

                // Determine company role
                CompanyRoleType companyType;
                switch (model.CompanyRole)
                {
                    case "Buyer":
                        companyType = CompanyRoleType.Buyer;
                        break;
                    case "Supplier":
                        companyType = CompanyRoleType.Supplier;
                        break;
                    case "Shipper":
                        companyType = CompanyRoleType.Shipper;
                        break;
                    default:
                        throw new Exception("Unrecognized Company Role: " + model.CompanyRole);
                }

                // Create Company Account
                CompanyInsertRequest company = new CompanyInsertRequest
                {
                    Name = model.Company,
                    OwnerId = newUser.Id,
                    TypeId = companyType
                };

                int companyId = _CompanyOptionService.InsertCompany(company);


                // Set User Role
                _AdminService.SetUserRole(newUser.Id, _AdminService.GetCompanyOwnerRoleId());

                //- Create Blank User Profile
                CreateBlankProfileRequest newUserProfile = new CreateBlankProfileRequest
                {
                    UserId = newUser.Id,
                    UserEmail = newUser.Email,
                    CompanyId = companyId
                };

                _UserProfileService.InsertBlankProfile(newUserProfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (newUser != null)
            {
                // Use model input to create confirmation email
                InsertTokenRequest insertToken = new InsertTokenRequest
                {
                    Email = model.Email,
                    UserId = newUser.Id,
                    TokenType = Enums.TokenType.Registration
                };

                EmailRequest ToSend = new EmailRequest();
                try
                {
                    ToSend = _UserEmailService.BuildAccountConfirmEmail(insertToken);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                _UserEmailService.SendEmail(ToSend);

                success = true;
            }

            response.Item = success;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // User Login [Route("login")]
        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login(UserLogInRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<bool> response = new ItemResponse<bool>();

            // create temp variable, use conditional, return createErrorResponse(don't return ModelState)
            response.Item = UserService.Signin(model.Email, model.Password);

            if (response.Item)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Login Error"));
            }


        }



        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // Forgot Password [Route("forgotPassword")]
        [HttpPost]
        [Route("forgotPassword")]
        public HttpResponseMessage ForgotPassword(InsertTokenRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bool success = false;

            ApplicationUser thisUser = UserService.GetUser(model.Email);

            UserDomain smsText = new UserDomain();
            smsText = _UserProfileService.GetPhoneNumberByEmail(model.Email);

            if (thisUser != null)
            {

                //- Create Token 

                model.TokenType = Enums.TokenType.ForgotPassword;

                model.UserId = thisUser.Id;

                Guid token = _TokenService.RegisterToken(model);

                //- Create the link 
                string link = "/public/resetpassword/" + token.ToString();

                link = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + link;

                //- Send Link to user email 
                EmailRequest confirm = new EmailRequest();

                confirm.UserEmail = thisUser.Email;
                confirm.Subject = "Reset Your Password";
                confirm.Content = link;

                _UserEmailService.SendEmail(confirm);

                // TEXTING SERVICE BUILT BY RAVID YOEUN
                NotifySMSRequest userModel = new NotifySMSRequest();

                userModel.Phone = smsText.PhoneNumber;
                userModel.firstName = smsText.FirstName;
                userModel.lastName = smsText.LastName;
                userModel.url_link = link;

                NotifySMSService.SendConfirmText(userModel);
                success = true;
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = success;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }



        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // Reset Password [Route("resetpassword")]
        [HttpPut]
        [Route("resetpassword")]
        public HttpResponseMessage ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            TokenDomain token = _TokenService.GetToken(model.Token);

            bool isSuccessful = UserService.ChangePassWord(token.UserId, model.NewPassword);

            bool testBool = false;


            if (isSuccessful)
            {
                if (_UserProfileService.ConfirmUserEmail(model.Token) || testBool)
                {
                    testBool = true;
                }

                _TokenService.MarkTokenAsUsed(model.Token);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // Reset Password [Route("resetpassword")]
        [HttpPut]
        [Route("setpassword")]
        public HttpResponseMessage SetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            TokenDomain token = _TokenService.GetToken(model.Token);

            bool isSuccessful = UserService.ChangePassWord(token.UserId, model.NewPassword);

            if (isSuccessful)
            {
                _TokenService.MarkTokenAsUsed(model.Token);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }


        // ++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        [Route("token")]
        public HttpResponseMessage CreateToken(InsertTokenRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<Guid> response = new ItemResponse<Guid>();
            response.Item = _TokenService.RegisterToken(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // Confirm User Email
        [HttpGet]
        [Route("registration")]
        public HttpResponseMessage ConfirmEmail([FromUri]Guid token)
        {
            //- Check for valid model
            if (!ModelState.IsValid)
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

            //- Create response<bool>. Set to false
            ItemResponse<bool> response = new ItemResponse<bool>();
            response.Item = false;

            //- Use Token to confirm User email
            response.Item = _UserProfileService.ConfirmUserEmail(token);

            //- Return response
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }





        // Email verification [Route("emailVerification")]

        // Social login (dunno if we'll actually use this)


        // ////////////////////////////////////////////////////////
        // Most of this should be moved to the Media services
        // File upload
        //[HttpPost]
        //[Route("upload")]
        public void UploadFile()
        {
            /*This method works and creates a fully-hydrated request model*/
            try
            {
                // get variables first
                NameValueCollection nvc = HttpContext.Current.Request.Form;
                var model = new UploadRequest();
                var request = HttpContext.Current.Request;
                // iterate through and map user values to strongly typed model
                foreach (string kvp in nvc.AllKeys)
                {
                    PropertyInfo propInfo = model.GetType().GetProperty(kvp, BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        propInfo.SetValue(model, nvc[kvp], null);
                    }
                }
                // Attach uploaded file itself to the model
                model.File = new HttpPostedFileWrapper(HttpContext.Current.Request.Files["file"]);

                // set filename
                model.FileName = Guid.NewGuid() + model.File.FileName;

                // Save file to project uploads
                string filePath = HttpContext.Current.Server.MapPath("~/App_Data/uploads/" + model.FileName);

                model.File.SaveAs(filePath);

                // Send the file to AWS file storage and capture the resource URL
                //S3Handler s3 = new S3Handler();
                //model.Location = s3.UploadFile(filePath, model.FileName, true);
                // Tie up loose ends on the model
                if (model.FolderName == null)
                {
                    model.FolderName = "No Folder";
                }
                // Save file information in DB
                //UploadService.InsertUpload(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage Test()
        {
            var context = HttpContext.Current;
            //string test = model.FileName;

            FileService(context);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public void FileService(HttpContext context)
        {

            var File = new HttpPostedFileWrapper(HttpContext.Current.Request.Files["file"]);

            string MIME = MimeMapping.GetMimeMapping(File.FileName);

            string FileName = Guid.NewGuid() + File.FileName;

            string filePath = context.Server.MapPath("~/App_Data/uploads/" + FileName);

            File.SaveAs(filePath);

            //S3Handler s3service = new S3Handler();
            //string url = s3service.UploadFile(filePath, FileName, true);
        }
        // /////////////////////////////////////////////////////////////////////////////////////////
        // ++++++++++++++++++++++++++++++++++++++++++++++++
        // User Invite
        [HttpPost]
        [Route("invite")]
        public HttpResponseMessage inviteUser(InviteUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bool success = false;

            ApplicationUser thisRole = UserService.GetUser(model.Email);

            if (thisRole == null)
            {

                //- Create the link 
                string link = "/Account/Register/";

                link = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + link;

                //- Send Link to user email 
                EmailRequest confirm = new EmailRequest();

                confirm.UserEmail = thisRole.Email;
                confirm.Subject = "Hello, You've been invited to join QuoteMule! Please sign up now";
                confirm.Content = link;
                // confirm.html = 

                _UserEmailService.SendEmail(confirm);

                success = true;
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = success;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


    }



}
