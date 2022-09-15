using Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pilotbird.Claim.Service.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ApiControllerBase : Controller
    {
        #region Private Properties
        private readonly IMediator _mediator;
        #endregion Private Properties

        #region Protected Properties
        protected string CurrentUserName => HttpContext?.User?.Identity?.Name;

        private Guid? _currentUserIdClaimValue;

        protected Guid? CurrentUserIdClaimValue
        {
            get
            {
                if (_currentUserIdClaimValue == null || _currentUserIdClaimValue == Guid.Empty)
                {
                    var sidClaimValue = HttpContext?.User?.Claims.EmptyIfNull().SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value;

                    Guid.TryParse(sidClaimValue, out Guid sidConvertedClaimValue);

                    if (sidConvertedClaimValue != Guid.Empty)
                    {
                        _currentUserIdClaimValue = sidConvertedClaimValue;
                    }
                }

                return _currentUserIdClaimValue;
            }
        }
        #endregion

        #region Constructor
        public ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException();
        }
        #endregion Constructor

        #region Protected Methods

        protected ActionResult<T> Single<T>(T data)
        {
            if (data == null) return NotFound();
            return Ok(data);
        }

        protected async Task<TResult> Send<TResult>(IRequest<TResult> command)
        {
            return await _mediator.Send(command);
        }

        protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
        {
            return await _mediator.Send(query);
        }
        protected async Task Publish<TRequest>(TRequest request)
        {
            await _mediator.Publish(request);
        }

        protected IActionResult GetOkResult<T>(T result)
        {
            if (result == null)
            {
                return Ok();
            }

            return Ok(result);
        }

        protected IActionResult GetErrorResult<T>(T result)
        {
            if (result == null)
            {
                return new StatusCodeResult(500);
            }

            if (ModelState.IsValid)
            {
                return BadRequest(result);
            }
            return BadRequest(ModelState);
        }

        #endregion
    }
}
