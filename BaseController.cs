using Microsoft.Practices.Unity;
using adr.Web.Domain;
using adr.Web.Models.ViewModels;
using adr.Web.Services;
using adr.Web.Services.Interfaces;
using adr.Web.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adr.Web.Controllers
{
    public class BaseController : Controller
    {
        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }

        public new ViewResult View()
        {
            BaseViewModel BaseModel = GetViewModel<BaseViewModel>(); // create a new instance

            BaseModel = DecorateViewModel<BaseViewModel>(BaseModel); // add user id to new instance

            return base.View(BaseModel); // make sure to pass basemodel in otherwise it will do nothing
        }

        public new ViewResult View(string viewString)
        {
            BaseViewModel BaseModel = GetViewModel<BaseViewModel>(); // create a new instance

            BaseModel = DecorateViewModel<BaseViewModel>(BaseModel); // add user id to new instance

            return base.View(viewString, BaseModel); // call the base class controller and return the ID to it
        }

        public ViewResult View(string ViewStrings, BaseViewModel BaseVMS)
        {

            BaseVMS = DecorateViewModel<BaseViewModel>(BaseVMS); // add user ID to BaseVMS

            return base.View(ViewStrings, BaseVMS); // call the base class controller and return the ID to it
        }

        public ViewResult View(BaseViewModel baseVM)
        {
            baseVM = DecorateViewModel<BaseViewModel>(baseVM); // decorate baseVM with the string id

            return base.View(baseVM); // call the base class controller and return the ID to it
        }

        protected T GetViewModel<T>() where T : BaseViewModel, new() //creates a new instance of the object in the base view model
        {
            //don't add anything to this function "ever"

            T viewModel = new T(); // create a new variable using T 
            return viewModel; //return the id to the variable
        }

        protected T DecorateViewModel<T>(T model) where T : BaseViewModel // T is extended with the baseviewmodel
        {
            if (UserService.IsLoggedIn())
            {
                UserPageDomain upd = UserPageService.GetModel();
                string thisUserId = UserService.GetCurrentUserId();
                model._IsLoggedIn = true;
                model._AccountId = thisUserId;// attach the user id to the model (string value)
                Console.WriteLine("Right");

                model._UserName = upd.UserName;

                model._IsEmailConfirmed = upd.IsEmailConfirmed;

                model._UserEmail = upd.UserEmail;

                model._FirstName = upd.FirstName;

                model._LastName = upd.LastName;

                model._CompanyId = upd.CompanyId;

                model._UserPhone = upd.UserPhone;

                model._UserPhotoUrl = upd.UserPhotoUrl;

                model._CompanyOwnerId = upd.CompanyOwnerId;

                model._DateCreated = upd.DateCreated;

                model._CompanyName = upd.CompanyName;

                model._CompanyType = upd.CompanyType;

                model._CompanyEmail = upd.CompanyEmail;

                model._CompanyAddressString = upd.CompanyAddressString;

                model._CompanyPhotoUrl = upd.CompanyPhotoUrl;

                if(thisUserId != null)
                {
                    model._Auth = true;
                }

                if(_UserProfileService.IsCompanyAdmin(thisUserId))
                {
                    model._Admin = true;
                }

                if(_UserProfileService.IsCompanyOwner(thisUserId))
                {
                    model._Owner = true;
                    model._Admin = true;
                }
    }
            else
            {
                model._IsLoggedIn = false;
                Console.WriteLine("Wrong");
            }


            return model; // return the value

        }
    }
}