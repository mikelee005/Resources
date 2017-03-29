using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using System.Web.Http;
using adr.Web.Services.Interfaces;
using adr.Web.Services;

namespace adr.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            UnityContainer container = new UnityContainer();

			// register all your components with the container here
			// it is NOT necessary to register your controllers
			container.RegisterType<IQuoteRequestService, QuoteRequestService>();
			container.RegisterType<IQuoteRequestItemService, QuoteRequestItemService>();

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<ICompanyRatingService, CompanyRatingService>();

            container.RegisterType<ICompanyAdminService, CompanyAdminService >();

            container.RegisterType<ITemplateEmailService, TemplateEmailService>();

            container.RegisterType<IProductService, ProductService>();

            container.RegisterType<ITokenService, TokenService>();

            container.RegisterType<IActivityService, ActivityService>();

            container.RegisterType<IAddressService, AddressService>();

            container.RegisterType<IResourcesService, ResourcesService>();

            container.RegisterType<IMediaService, MediaService>();

            container.RegisterType<IConversationService, ConversationService>();

            container.RegisterType<ICompanyService, CompanyService>();

            container.RegisterType<INotificationService, NotificationService>();

            container.RegisterType<IMapService, MapService>();

            container.RegisterType<IMarketPlaceService, MarketPlaceService>();

            container.RegisterType<IUserEmailService, UserEmailService>();

            container.RegisterType<IProductMarketPlaceService, ProductMarketPlaceService>();

            container.RegisterType<IAdminService, AdminService>();

            container.RegisterType<IAdminSPgBService, AdminSPgBService>();

            container.RegisterType<IBidService, BidService>();

            container.RegisterType<IFaqService, FaqService>();

            container.RegisterType<IUserProfileService, UserProfileService>();

            container.RegisterType<ISupplierSettingsService, SupplierSettingsService>();

            container.RegisterType<IPdfService, PdfService>();

            container.RegisterType<IContractService, ContractService>();

            container.RegisterType<IQuoteService, QuoteService>();

			container.RegisterType<IshippingQuoteRequestService, ShippingQuoteRequestService>();


			DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            //  this line is needed so that the resolver can be used by api controllers 
            config.DependencyResolver = new adr.Web.Core.Injection.UnityResolver(container);



        }
    }
}