using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Test
{
    [PostProcessScene(0)]
    public static void OnPost()
    {
        Debug.Log("PostProcessSceneAttribute");
    }
}
