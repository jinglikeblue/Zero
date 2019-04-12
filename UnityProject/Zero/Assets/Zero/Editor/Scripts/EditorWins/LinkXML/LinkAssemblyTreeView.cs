using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Zero.Edit
{
    public class LinkAssemblyTreeView : TreeView
    {
        TreeViewItem _root;

        public LinkAssemblyTreeView(TreeViewState state) : base(state)
        {
        }

        protected override TreeViewItem BuildRoot()
        {
            return _root;
        }

        //protected override void RowGUI(RowGUIArgs args)
        //{
        //    Event evt = Event.current;
        //    extraSpaceBeforeIconAndLabel = 18f;

        //    Rect toggleRect = args.rowRect;
        //    toggleRect.x += GetContentIndent(args.item);
        //    toggleRect.width = 16f;

        //    if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
        //        SelectionClick(args.item, false);

        //    EditorGUI.BeginChangeCheck();

        //    bool isSelected = EditorGUI.Toggle(toggleRect, true);

        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Debug.Log(args.item.displayName);
        //    }

        //    base.RowGUI(args);
        //}

        public void UpdateData(List<CreateLinkXMLCommand.AssemblyNodeVO> list)
        {
            int ASSEMBLY_DEPTH = 0;
            int NAMESPACE_DEPTH = 1;
            int TYPE_DEPTH = 2;

            int id = 0;
            var root = new TreeViewItem(id++, -1, "root");
            var allItems = new List<TreeViewItem>();
            for(int i = 0; i < list.Count; i++)
            {
                var assembly = list[i];
                TreeViewItem assemblyItem = new TreeViewItem(id++, ASSEMBLY_DEPTH, assembly.name+ "[dll]");
                allItems.Add(assemblyItem);
                for(int j = 0; j < assembly.nsNodeList.Count; j++)
                {
                    var ns = assembly.nsNodeList[j];
                    TreeViewItem nsItem = new TreeViewItem(id++, NAMESPACE_DEPTH, ns.name + "[namespace]");                    
                    allItems.Add(nsItem);
                    for(int k = 0; k < ns.typeNameList.Count; k++)
                    {
                        var typeName = ns.typeNameList[k];
                        TreeViewItem typeItem = new TreeViewItem(id++, TYPE_DEPTH, typeName + "[class]");
                        allItems.Add(typeItem);
                    }
                }
            }

            SetupParentsAndChildrenFromDepths(root, allItems);
            _root = root;
            Reload();
        }
    }
}