// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
    using System;

    public class LaunchInvitePanelFlowResult
    {
        public readonly UserList InvitedUsers;


        public LaunchInvitePanelFlowResult(IntPtr o)
        {
            InvitedUsers = new UserList(CAPI.ovr_LaunchInvitePanelFlowResult_GetInvitedUsers(o));
        }
    }

}
