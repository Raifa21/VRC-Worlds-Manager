using System;

namespace VRC_Favourite_Manager.Common
{
    public class VRCNotLoggedInException: Exception
    {
        
    }
    public class VRCIncorrectCredentialsException: Exception
    {
        
    }
    public class VRCRequiresTwoFactorAuthException: Exception
    {
        public string TwoFactorAuthType { get; private set; }

        public VRCRequiresTwoFactorAuthException(string twoFactorAuthType)
        {
            TwoFactorAuthType = twoFactorAuthType;
        }
    }
    public class VRCAPIException: Exception
    {
        
    }
    public class VRCFailedToCreateInviteException: Exception
    {
        
    }
    public class VRCFailedToCreateInstanceException: Exception
    {
        
    }

    public class VRCServiceUnavailableException : Exception
    {
        public string Message { get; private set; }

        public VRCServiceUnavailableException(string message)
        {
            Message = message;
        }
}
}
