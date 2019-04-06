using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Threading_Interlocked_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            Zero_ASingletonMonoBehaviour_1_ILBridge_Binding.Register(app);
            Zero_ILBridge_Binding.Register(app);
            Zero_ComponentUtil_Binding.Register(app);
            Zero_ZeroView_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            System_Action_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            Zero_ObjectBindingData_Binding.Register(app);
            Zero_DoubleBindingData_Binding.Register(app);
            Zero_FloatBindingData_Binding.Register(app);
            Zero_IntBindingData_Binding.Register(app);
            Zero_LongBindingData_Binding.Register(app);
            Zero_StringBindingData_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Activator_Binding.Register(app);
            Zero_ResMgr_Binding.Register(app);
            UnityEngine_CanvasGroup_Binding.Register(app);
            UnityEngine_WaitForEndOfFrame_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            System_Collections_Generic_HashSet_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_ILTypeInstance_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Type_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            Zero_Runtime_Binding.Register(app);
            Zero_RuntimeVO_Binding.Register(app);
            Zero_RuntimeVO_Binding_MainPrefabCfgVO_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            Zero_GUIDeviceInfo_Binding.Register(app);
            Zero_ASingletonMonoBehaviour_1_AudioPlayer_Binding.Register(app);
            Zero_AudioPlayer_Binding.Register(app);
            AEventListener_1_Physics2DEventListener_Binding.Register(app);
            Zero_Physics2DEventListener_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            UnityEngine_Rigidbody2D_Binding.Register(app);
            Zero_UIEventListener_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            Zero_Texture2DUtil_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Action_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
