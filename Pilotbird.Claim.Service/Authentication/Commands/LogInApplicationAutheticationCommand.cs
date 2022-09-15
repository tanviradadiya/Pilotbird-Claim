using MediatR;
using Pilotbird.Claim.Service.Authentication.Responses;

namespace Pilotbird.Claim.Service.Authentication.Commands
{
    public class LogInApplicationAutheticationCommand : IRequest<LoginResponse>
    {
        public string Email { get; }
        public string Password { get; }
        public string stIPAddress { get; set; }

        public LogInApplicationAutheticationCommand
            (
                string email,
                string password,
                string ipaddress
            )
        {
            Email = email;
            Password = password;
            stIPAddress = ipaddress;
        }
    }
}
