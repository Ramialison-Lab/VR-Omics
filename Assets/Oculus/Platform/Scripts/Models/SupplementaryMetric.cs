// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
    using System;

    public class SupplementaryMetric
    {
        public readonly UInt64 ID;
        public readonly long Metric;


        public SupplementaryMetric(IntPtr o)
        {
            ID = CAPI.ovr_SupplementaryMetric_GetID(o);
            Metric = CAPI.ovr_SupplementaryMetric_GetMetric(o);
        }
    }

}
