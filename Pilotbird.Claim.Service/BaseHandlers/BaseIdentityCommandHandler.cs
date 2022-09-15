using DataAccess.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.Configuration;
using Pilotbird.Claim.EFCore.DBModels.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using static Common.Enums.ApplicationEnums;

namespace Pilotbird.Claim.Service.BaseHandlers
{
    public abstract class BaseIdentityCommandHandler<TCommand, TCommandResult> : IRequestHandler<TCommand, TCommandResult>
        where TCommand : IRequest<TCommandResult>
    {
        protected UserManager<ApplicationUser> UserManager;
        protected BaseIdentityCommandHandler
           (
               UserManager<ApplicationUser> userManager
           )
        {
            UserManager = userManager;
        }

        protected IMediator _mediator;
        protected BaseIdentityCommandHandler
            (
                UserManager<ApplicationUser> userManager,
                IMediator mediator
            )
        {
            UserManager = userManager;
            _mediator = mediator;
        }

        protected JwtConfiguration _jwtConfiguration;
        protected BaseIdentityCommandHandler
            (
                UserManager<ApplicationUser> userManager,
                IMediator mediator,
                JwtConfiguration jwtConfiguration
            )
        {
            UserManager = userManager;
            _mediator = mediator;
            _jwtConfiguration = jwtConfiguration;
        }

        protected ICommandRepositoryAsync _commandRepositoryAsync;

        protected BaseIdentityCommandHandler
            (
                UserManager<ApplicationUser> userManager,
                IMediator mediator,
                JwtConfiguration jwtConfiguration,
                ICommandRepositoryAsync commandRepositoryAsync
            )
        {
            UserManager = userManager;
            _mediator = mediator;
            _jwtConfiguration = jwtConfiguration;
            _commandRepositoryAsync = commandRepositoryAsync;
        }

        protected IOptionsSnapshot<ApplicationSettings> _applicationSettings;
        protected BaseIdentityCommandHandler
            (
                UserManager<ApplicationUser> userManager,
                IMediator mediator,
                JwtConfiguration jwtConfiguration,
                ICommandRepositoryAsync commandRepositoryAsync,
                IOptionsSnapshot<ApplicationSettings> applicationSettings
            )
        {
            UserManager = userManager;
            _mediator = mediator;
            _jwtConfiguration = jwtConfiguration;
            _commandRepositoryAsync = commandRepositoryAsync;
            _applicationSettings = applicationSettings;
        }

        private static IEnumerable<System.Security.Claims.Claim> GenerateClaimsForApplicationUser(ApplicationUser applicationUser, bool IsAPIUser) => new[]
                {
                    // The "sub" (subject) claim identifies the principal that is the subject of the JWT.
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                    // The "jti" (JWT ID) claim provides a unique identifier for the JWT.
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sid, applicationUser.Id.ToString()),
                    IsAPIUser ? new System.Security.Claims.Claim("CurrentUserAPIAccessToken",  !string.IsNullOrEmpty(applicationUser.APIAccessToken)?  applicationUser.APIAccessToken.ToString(): "") : null,
                    new System.Security.Claims.Claim("IsAPIUserRole", IsAPIUser.ToString())

                };

        protected string GenerateJwtSecurityTokenForApplicationUser(ApplicationUser applicationUser, bool IsAPIUser)
        {
            var token = new JwtSecurityToken
            (
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                expires: DateTime.UtcNow.AddSeconds(_jwtConfiguration.NumberOfSecondsUntilExpiration),
                claims: GenerateClaimsForApplicationUser(applicationUser, IsAPIUser),
                signingCredentials: _jwtConfiguration.GenerateSigningCredentials()
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected async Task<DateTimeOffset?> ExecuteCommandScalarAsync
          (
              string commandText,
              CommandType commandType = CommandType.Text
          )
        {
            return await _commandRepositoryAsync.ExecuteCommandScalarAsync<DateTimeOffset>
            (
                commandText,
                commandType
            );
        }


        protected async Task<ForgotPasswordRequestStatus> ExecuteCreateCommandScalarWithObjectParametersAsync
       (
           string commandText,
           object command,
           CommandType commandType = CommandType.StoredProcedure
       )
        {
            return await _commandRepositoryAsync.ExecuteCommandScalarAsync<ForgotPasswordRequestStatus>
            (
                commandText,
                commandType,
                command
            );
        }

        public abstract Task<TCommandResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
