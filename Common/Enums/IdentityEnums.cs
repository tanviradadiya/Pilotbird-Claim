using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Enums
{
    public static class IdentityEnums
    {
        public enum LoginStatus
        {
            Success,
            Unauthorized,
            InActiveUser,
            InArchivedUser,
        }
    }
}
