using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using VTuber.Editor.Utils;

namespace Editor
{
    public class SOEditor : OdinMenuEditorWindow
    {
        [MenuItem("CustomEditors/SO/SOEditor")]
        public static void OpenWindow()
        {
            WindowExtension.OpenWindow<SOEditor>(new Vector2(800,600),new GUIContent("SOEditor"));
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false);
            tree.AddAssetAtPath("UI Diaplay Config","Assets/Resources/so_preview cfg.asset");
            tree.Config.DrawSearchToolbar = true;
            
            foreach (var dataList in SO_PreviewCfg.Instance.displayDatas)
            {
                foreach (var data in dataList.value)
                {
                    if (data.so != null)
                    {
                        tree.Add(dataList.key+"/"+data.name, data.so);
                    }
                    else
                    {
                        tree.AddAssetAtPath(dataList.key+"/"+data.name, data.path);
                    }
                }
            }
            return tree;
        }
        

        protected override void OnBeginDrawEditors()
        {
            try
            {
                var selected = MenuTree.Selection.FirstOrDefault();
                var toolbarHeight = MenuTree.Config.SearchToolbarHeight;
                SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
                {
                    if (selected != null)
                    {
                        GUILayout.Label(selected.Name);
                    }

                    if (MenuTree.Selection.SelectedValue == SO_PreviewCfg.Instance)
                    {
                        if (GUILayout.Button("Refresh"))
                        {
                            ForceMenuTreeRebuild();
                        }
                    }
                }
                SirenixEditorGUI.EndHorizontalToolbar();
            }
            catch (Exception e)
            {
            }

        }
    }

}