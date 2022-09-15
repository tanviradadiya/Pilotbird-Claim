using Microsoft.AspNetCore.Http;
using static Common.Enums.ApplicationEnums;

namespace Pilotbird.Claim.Service.Infrastructure.ApiResponses
{
    public class ApiBadRequestResponse : BaseApiResponse
    {
        #region Constructors

        public ApiBadRequestResponse(string errorMessage)
            : base(StatusCodes.Status400BadRequest, APIResponseStatus.Error, errorMessage: errorMessage)
        { }

        #endregion
    }

    public class ApiUnauthorizedRequestResponse : BaseApiResponse
    {
        #region Constructors

        public ApiUnauthorizedRequestResponse(string errorMessage)
            : base(StatusCodes.Status401Unauthorized, APIResponseStatus.Error, errorMessage: errorMessage)
        { }

        #endregion
    }
}
