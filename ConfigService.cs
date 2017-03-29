using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace adr.Web
{
	public class ConfigService
	{

		public static string GetFromConfig(string key)
		{
			return WebConfigurationManager.AppSettings[key];
			
		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetSupportEmailAddress()
		{
			return GetFromConfig("SupportEmailAddressKey");

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetSendGridKey()
		{
			return GetFromConfig("EmailSendGridKey");

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetAWS_S3Bucket()
		{
			return GetFromConfig("AWS_S3Bucket");

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetAWS_ServiceUrl()
		{
			return GetFromConfig("AWS_ServiceUrl");

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetAWS_AccessKey()
		{
			return GetFromConfig("AWS_AccessKey");

		}


		// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		public static string GetAWS_SecretKey()
		{
			return GetFromConfig("AWS_SecretKey");

		}

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string GetBT_MerchantId()
        {
            return GetFromConfig("BT_MerchantId");

        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string GetBT_PublicKey()
        {
            return GetFromConfig("BT_PublicKey");

        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string GetBT_PrivateKey()
        {
            return GetFromConfig("BT_PrivateKey");

        }


    }

}
