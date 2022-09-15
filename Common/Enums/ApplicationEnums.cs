using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Enums
{
    public static class ApplicationEnums
    {
        public enum APIResponseStatus
        {
            Success,
            Error,
            Fail
        }

        public enum ApplicationRoles
        {
            SuperAdmin,
            ClientAdmin,
            User
        }


        public enum Gender
        {
            Male,
            Female,
            Other
        }

        public enum CreationStatus
        {
            Success,
            Error,
            VerificationFailed,
            AlreadyExists,
            NoAllowed
        }

        public enum ForgotPasswordRequestStatus : int
        {
            Success = 1,
            Error = 0,
            InactiveUser = -1,
            RecordDoesNotExists = -2,
            APIUserRole = -3

        }

        public enum ResetPasswordRequestStatus : int
        {
            Success = 1,
            Error = 0,
            TokenExpired = -1,
            RecordDoesNotExists = -2
        }

        public enum RecordCreationStatus : int
        {
            Success = 1,
            Error = 0,
            RecordAlreadyExists = -1,
            UserAlreadyFriend = -2,
            Unauthorized = 401
        }

        public enum RecordUpdationStatus : int
        {
            Success = 1,
            Error = 0,
            VerificationFailed = 2,
            Unauthorized = 401
        }

        public enum RecordDeletionStatus : int
        {
            Success = 1,
            Error = 0,
            Unauthorized = 401
        }

        public enum ChangePasswordStatus
        {
            Success,
            Error,
            RecordDoesNotExists,
            ExistingPasswordNotValid
        }

        public enum AccountSettingsUpdateStatus
        {
            Success,
            Error,
            RecordDoesNotExists,
            ExistingPasswordNotValid
        }

        public enum WebApplicationArea
        {
            API,
            Web
        }

    }
}
