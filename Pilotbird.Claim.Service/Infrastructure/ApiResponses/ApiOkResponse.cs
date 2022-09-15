using Microsoft.AspNetCore.Http;
using static Common.Enums.ApplicationEnums;

namespace Pilotbird.Claim.Service.Infrastructure.ApiResponses
{
    public class ApiOkResponse : BaseApiResponse
    {
        #region Public Properties

        public object Data { get; }

        #endregion

        #region Constructors

        public ApiOkResponse(object data, string message = null) : base(StatusCodes.Status200OK, APIResponseStatus.Success, message: message) => Data = data;

        #endregion
    }
}
