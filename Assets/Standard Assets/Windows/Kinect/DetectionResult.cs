#if !(UNITY_WSA_10_0 && NETFX_CORE)
using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Windows.Kinect
{
    //
    // Windows.Kinect.DetectionResult
    //
    public enum DetectionResult : int
    {
        Unknown                                  =0,
        No                                       =1,
        Maybe                                    =2,
        Yes                                      =3,
    }

}
#endif
