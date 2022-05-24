// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
    using System;

    public class RejoinDialogResult
    {
        public readonly bool RejoinSelected;


        public RejoinDialogResult(IntPtr o)
        {
            RejoinSelected = CAPI.ovr_RejoinDialogResult_GetRejoinSelected(o);
        }
    }

}
