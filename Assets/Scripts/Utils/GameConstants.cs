using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public static class GameConstants
{
    static float tapDelay = 0.3f;
    static float nodeBaseSize = 1.2f;
    static float mobileNodeBaseSize = 2.4f;

    [DllImport("__Internal")]
    private static extern bool IsMobile();


    public static float NodeBaseSize 
    {
        get { return nodeBaseSize; }
    }


    public static float TapDelay
    {
        get { return tapDelay; }
    }


    public static void Initialize()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            if(IsMobile()){
                nodeBaseSize = mobileNodeBaseSize;
            }
        #elif UNITY_ANDROID
            nodeBaseSize = mobileNodeBaseSize;
        #elif UNITY_IOS
            nodeBaseSize = mobileNodeBaseSize;
        #endif
    }

}
