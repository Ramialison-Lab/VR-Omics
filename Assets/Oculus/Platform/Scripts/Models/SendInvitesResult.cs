// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
    using System;

    public class SendInvitesResult
    {
        public readonly ApplicationInviteList Invites;


        public SendInvitesResult(IntPtr o)
        {
            Invites = new ApplicationInviteList(CAPI.ovr_SendInvitesResult_GetInvites(o));
        }
    }

}