using Newtonsoft.Json;
using static Common.Enums.ApplicationEnums;

namespace Pilotbird.Claim.Service.Infrastructure.ApiResponses
{
    public class BaseApiResponse
    {
        #region Public Properties

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; }

        public int StatusCode { get; }

        public string Status { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; }

        #endregion

        #region Private Static Methods

        private static string GenerateDefaultResponseMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resource not found.";
                case 500:
                    return "An unhandled error occurred.";
                default:
                    return null;
            }
        }

        private static object GenerateDefaultResponseDataForStatusCode(int statusCode, object data)
        {
            switch (statusCode)
            {
                case 404:
                    return null;
                case 500:
                    return null;
                default:
                    return data;
            }
        }

        #endregion

        #region Constructors

        public BaseApiResponse(int statusCode, APIResponseStatus status, string message = null, string errorMessage = null, object data = null)
        {
            Data = GenerateDefaultResponseDataForStatusCode(statusCode, data);

            StatusCode = statusCode;

            Status = status.ToString();

            Message = message ?? GenerateDefaultResponseMessageForStatusCode(statusCode);

            ErrorMessage = errorMessage;
        }

        #endregion
    }
}
