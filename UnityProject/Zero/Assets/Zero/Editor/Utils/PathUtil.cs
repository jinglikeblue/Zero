using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    public class PathUtil
    {
        public static string PlatformDirName
        {
            get
            {
                string name;
#if UNITY_STANDALONE
                name = "pc/";
#elif UNITY_IPHONE
        name = "ios/";
#elif UNITY_ANDROID
                name = "android/";
#endif
                return name;
            }
        }

    }
}