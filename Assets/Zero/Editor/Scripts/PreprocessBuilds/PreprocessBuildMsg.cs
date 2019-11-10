using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreprocessBuildMsg : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return int.MinValue; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Build Will Start");
    }
}
