using Jing;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Zero.Edit
{
    class LinkXMLEditorWin : AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<LinkXMLEditorWin>("Useless Assets Find", true);
            win.minSize = new Vector2(800, 700);
            win.maxSize = new Vector2(1000, 700);
            win.Show();
        }

        [SerializeField]
        TreeViewState _tvs;

        LinkAssemblyTreeView _latv;

        SearchField _searchField;

        string _selectDir;
        List<CreateLinkXMLCommand.AssemblyNodeVO> _nodeList;
        CreateLinkXMLCommand _cmd;

        private void OnEnable()
        {
            if(_tvs == null)
            {
                _tvs = new TreeViewState();
            }

            _latv = new LinkAssemblyTreeView(_tvs);
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _latv.SetFocusAndEnsureSelectedItem;
        }

        Vector2 _pos = Vector2.zero;

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("选择DLL所在目录", GUILayout.Height(30)))
            {
                string dir = EditorUtility.OpenFolderPanel("Dll文件目录", Application.dataPath, "");
                if("" == dir)
                {
                    return;
                }
                _selectDir = dir;
                _cmd = new CreateLinkXMLCommand(dir);
                _cmd.onCreated += OnCreated;
                _cmd.Excute();
            }
           
            if (null != _nodeList)
            {
                _pos = GUILayout.BeginScrollView(_pos);

                DoToolbar();
                DoTreeView();
                DoSaveView();

                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

        private void OnCreated(CreateLinkXMLCommand cmd)
        {
            _nodeList = _cmd.NodeList;
            _cmd.onCreated -= OnCreated;
            _latv.UpdateData(_nodeList);
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("search",GUILayout.Width(50));
            //GUILayout.Space(100);
            //GUILayout.FlexibleSpace();
            _latv.searchString = _searchField.OnToolbarGUI(_latv.searchString);
            GUILayout.EndHorizontal();
        }

        void DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            _latv.OnGUI(rect);
        }

        void DoSaveView()
        {
            if (GUILayout.Button("保存 link.xml", GUILayout.Height(30)))
            {
                string saveDir = EditorUtility.OpenFolderPanel("选择保存位置", Application.dataPath, "");
                if ("" == saveDir)
                {
                    return;
                }
                var savePath = FileSystem.CombinePaths(saveDir, "link.xml");
                File.WriteAllText(savePath, _cmd.LinkXMLString);
                //打开目录
                EditorMenu.OpenDirectory(saveDir);
                Debug.Log("创建成功");
            }
        }
    }
}
