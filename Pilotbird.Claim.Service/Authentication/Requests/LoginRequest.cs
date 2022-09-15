using System.ComponentModel.DataAnnotations;

namespace Pilotbird.Claim.Service.Authentication.Requests
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IPAddress { get; set; }
        public string TimeZone { get; set; }
    }
}
