using Pilotbird.Claim.EFCore.DBModels.DB;
using static Common.Enums.IdentityEnums;

namespace Pilotbird.Claim.Service.Authentication.Responses
{
    public class LoginResponse
    {
        #region Public Properties

        public LoginStatus LoginStatus { get; }

        public string AccessToken { get; }

        public ApplicationUser ApplicationUser { get; set; }

        #endregion

        #region Constructor

        public LoginResponse
            (
                LoginStatus loginStatus,
                string accessToken,
                ApplicationUser applicationUser
            )
        {
            LoginStatus = loginStatus;

            AccessToken = accessToken;

            ApplicationUser = applicationUser;
        }
        #endregion
    }
}
