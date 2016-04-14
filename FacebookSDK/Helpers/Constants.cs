using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookSDK.Helpers
{
    public class Constants
    {
        public static string RedirectUriLogin
        {
            get { return "http://www.facebook.com/connect/login_success.html"; }
        }
        public static string RedirectUriFeed
        {
            get { return "https://m.facebook.com/?post_id="; }// "https://m.facebook.com/v2.0/dialog/app_requests/submit"; }//https://m.facebook.com/v2.0/dialog/app_requests/submit
        }
        public static string RedirectUriAppRequest
        {
            get { return "https://m.facebook.com/"; }
        }

        public static string RedirectUriRequestError
        {
            get { return "https://m.facebook.com/?error_code"; }
        }

        public static string UriNavigateBlankPage
        {
            get { return "about:blank"; }
        }

        public static string CookiesSettingKey
        {
            get { return "cookies"; }
        }

        public static string PublishActionsPermission
        {
            get { return "publish_actions"; }
        }
    }

    public static class GetUserInfoFields
    {
        public static string Id { get { return "id"; } }
        public static string About { get { return "about"; } }
        public static string LastName { get { return "last_name"; } }
        public static string Location { get { return "location"; } }
        public static string MiddleName { get { return "middle_name"; } }
        public static string Name { get { return "name"; } }
        public static string NameFormat { get { return "name_format"; } }
        public static string Bio { get { return "bio"; } }
        public static string Birthday { get { return "birthday"; } }
        public static string Email { get { return "email"; } }
        public static string FirstName { get { return "first_name"; } }
        public static string Gender { get { return "gender"; } }
        public static string IsVerified { get { return "is_verified"; } }
        public static string Verified { get { return "verified"; } }
        public static string Quotes { get { return "quotes"; } }
        ////Các phần bị khóa lại là các phần chưa được hộ trợ trong FBUser
        //public static string Link { get { return "link"; } }
        //public static string MeetingFor { get { return "meeting_for"; } }
        //public static string PaymentPricepoints { get { return "payment_pricepoints"; } }
        //public static string Religion { get { return "religion"; } }
        //public static string SecuritySettings { get { return "security_settings"; } }
        //public static string SignificantOther { get { return "significant_other"; } }
        //public static string ViewerCanSendGift { get { return "viewer_can_send_gift"; } }
        //public static string AgeRange { get { return "age_range"; } }
        //public static string Website { get { return "website"; } }
        //public static string Work { get { return "work"; } }
        //public static string Cover { get { return "cover"; } }
        //public static string Context { get { return "context"; } }
        //public static string Currency { get { return "currency"; } }
        //public static string Devices { get { return "devices"; } }
        //public static string Education { get { return "education"; } }
        //public static string FavoriteAthletes { get { return "favorite_athletes"; } }
        //public static string FavoriteTeams { get { return "favorite_teams"; } }
        //public static string Hometown { get { return "hometown"; } }
        //public static string InspirationalPeople { get { return "inspirational_people"; } }
        //public static string InstallType { get { return "install_type"; } }
        //public static string Installed { get { return "installed"; } }
        //public static string InterestedIn { get { return "interested_in"; } }
        //public static string IsSharedLogin { get { return "is_shared_login"; } }
        //public static string Languages { get { return "languages"; } }
        //public static string PublicKey { get { return "public_key"; } }
        //public static string SharedLoginUpgradeRequiredBy { get { return "shared_login_upgrade_required_by"; } }
        //public static string UpdatedTime { get { return "updated_time"; } }
        //public static string TokenForBusiness { get { return "token_for_business"; } }
        //public static string Timezone { get { return "timezone"; } }
        //public static string VideoUploadLimits { get { return "video_upload_limits"; } }
        //public static string ThirdPartyId { get { return "third_party_id"; } }
        //public static string Locale { get { return "locale"; } }
        //public static string Sports { get { return "sports"; } }
        //public static string TestGroup { get { return "test_group"; } }
        //public static string Political { get { return "political"; } }

    }
}
