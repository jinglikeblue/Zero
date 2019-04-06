#if UNITY_EDITOR
using UnityEditor;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis()
    {
      
    }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {

    }
}
#endif
