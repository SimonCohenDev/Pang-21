using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// this is to easily cast monobehavior components from one type to another
    /// </summary>
    /// <typeparam name="TOut">The type to cast to</typeparam>
    /// <param name="source">the object to cast</param>
    /// <returns>a cast object</returns>
    public static TOut To<TOut>(this MonoBehaviour source) where TOut : class
    {
        return source as TOut;
    }
    /// <summary>
    /// Execute command if platform is one stated in param
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="command"></param>
    /// <returns>True if command was executed</returns>
    
}
public static class Utils
{
    public static bool ExecuteForPlatform(RuntimePlatform platform, System.Action command)
    {
        if (IsPlatform(platform))
        {
            command();
            return true;
        }

        return false;
    }

    public static bool IsAndroid()
    {
        return IsPlatform(RuntimePlatform.Android);
    }

    public static bool IsPlatform(RuntimePlatform platform)
    {
        return Application.platform == platform;
    }
}
