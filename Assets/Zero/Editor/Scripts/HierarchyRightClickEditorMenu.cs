using Jing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class HierarchyRightClickEditorMenu
    {
        [MenuItem("GameObject/Zero/Copy Full Path", priority = -999)]
        static void CopyPath()
        {
            var objs = Selection.gameObjects;
            if(objs.Length == 1)
            {
                List<string> list = new List<string>();
                var obj = objs[0];
                while(obj != null)
                {
                    list.Add(obj.name);
                    if(obj.transform.parent != null)
                    {
                        obj = obj.transform.parent.gameObject;                        
                    }
                    else
                    {
                        obj = null;
                    }                    
                }

                list.Reverse();

                var path = "";
                path = FileUtility.CombinePaths(list.ToArray());
                GUIUtility.systemCopyBuffer = path;
            }
            else
            {
                Debug.Log("选中单独一个GameObject时，此功能有效");
            }
        }
    }
}