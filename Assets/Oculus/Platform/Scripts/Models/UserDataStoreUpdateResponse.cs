// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
    using System;

    public class UserDataStoreUpdateResponse
    {
        public readonly bool Success;


        public UserDataStoreUpdateResponse(IntPtr o)
        {
            Success = CAPI.ovr_UserDataStoreUpdateResponse_GetSuccess(o);
        }
    }

}
