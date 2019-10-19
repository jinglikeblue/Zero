using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PostprocessBuildMsg : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return int.MaxValue; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("Build Complete");
    }
}
