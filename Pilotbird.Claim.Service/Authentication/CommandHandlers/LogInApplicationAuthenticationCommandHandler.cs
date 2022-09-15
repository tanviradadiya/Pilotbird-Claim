using DataAccess.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models.Configuration;
using Pilotbird.Claim.EFCore.DBModels.DB;
using Pilotbird.Claim.Service.Authentication.Commands;
using Pilotbird.Claim.Service.Authentication.Responses;
using Pilotbird.Claim.Service.BaseHandlers;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums.IdentityEnums;

namespace Pilotbird.Claim.Service.Authentication.CommandHandlers
{
    public class LogInApplicationAuthenticationCommandHandler : BaseIdentityCommandHandler<LogInApplicationAutheticationCommand, LoginResponse>
    {
        #region Private Static Fields

        private static readonly string _createUserLoginHistory = "createUserLoginHistory";

        #endregion
        public LogInApplicationAuthenticationCommandHandler
       (
           UserManager<ApplicationUser> userManager,
           IMediator mediator,
           JwtConfiguration jwtConfiguration,
           ICommandRepositoryAsync commandRepositoryAsync
       ) : base(userManager, mediator, jwtConfiguration, commandRepositoryAsync) { }

        public async Task<LoginResponse> AuthenticateApplicationUserAsync(LogInApplicationAutheticationCommand command)
        {
            var user = await UserManager.FindByNameAsync(command.Email);

            if (user != null && (user.flgIsActive == false || user.flgIsArchived == true))
            {
                return new LoginResponse(!user.flgIsActive ? LoginStatus.InActiveUser : LoginStatus.InArchivedUser, null, null);
            }
            if (user != null && await UserManager.CheckPasswordAsync(user, command.Password))
            {
                await ExecuteCreateCommandScalarWithObjectParametersAsync(
                  _createUserLoginHistory,
                 new
                 {
                     uiUserId = user.Id,
                     stIPAddress = command.stIPAddress
                 });

                return new LoginResponse(LoginStatus.Success, GenerateJwtSecurityTokenForApplicationUser(user, false), user);

            }
            else
            {
                return new LoginResponse(LoginStatus.Unauthorized, null, null);
            }
        }

        public override async Task<LoginResponse> Handle
        (
            LogInApplicationAutheticationCommand command,
            CancellationToken cancellationToken
        ) => await AuthenticateApplicationUserAsync(command);
    }
}
