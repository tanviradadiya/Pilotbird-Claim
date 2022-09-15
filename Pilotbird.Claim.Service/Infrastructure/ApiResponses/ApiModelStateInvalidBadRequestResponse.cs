using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using static Common.Enums.ApplicationEnums;

namespace Pilotbird.Claim.Service.Infrastructure.ApiResponses
{
    public class ModelValidationErrors
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ApiModelStateInvalidBadRequestResponse : BaseApiResponse
    {
        #region Public Properties

        public IEnumerable<ModelValidationErrors> ErrorMessages { get; }

        #endregion

        #region Constructor

        public ApiModelStateInvalidBadRequestResponse(IEnumerable<ValidationFailure> validationFailure)
            : base(StatusCodes.Status400BadRequest, status: APIResponseStatus.Error)
        {
            ErrorMessages = validationFailure
                .Select(x => new ModelValidationErrors { PropertyName = x.PropertyName, ErrorMessage = x.ErrorMessage })
                .ToArray();
        }

        public ApiModelStateInvalidBadRequestResponse(ModelStateDictionary modelStateDictionary)
           : base(StatusCodes.Status400BadRequest, status: APIResponseStatus.Error)
        {
            if (modelStateDictionary.IsValid)
            {
                throw new ArgumentException("The ModelState is not invalid.", nameof(modelStateDictionary));
            }

            ErrorMessages = modelStateDictionary.Keys
                .SelectMany(key => modelStateDictionary[key].Errors.Select(x => new ModelValidationErrors { PropertyName = key, ErrorMessage = x.ErrorMessage }))
                .ToArray();
        }
        #endregion
    }
}
