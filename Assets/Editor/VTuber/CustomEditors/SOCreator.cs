using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Editor.Attributes;
using VTuber.Editor.Utils;

public class SOCreator : OdinMenuEditorWindow
{
    private Type SelectedType
    {
        get
        {
            var m = MenuTree.Selection.LastOrDefault(); 
            return m == null ? null : m.Value as Type;
        }
    }
    /// <summary>
    /// 选中的 ScriptableObject（等待创建）
    /// </summary>
    private ScriptableObject previewObject;

    private string saveName;
    private string currentDirectory;
    
    static HashSet<Type> scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
        .Where(t =>
            t.IsClass &&
            typeof(VScriptableObject).IsAssignableFrom(t) &&
            !typeof(EditorWindow).IsAssignableFrom(t) &&
            !typeof(UnityEditor.Editor).IsAssignableFrom(t))
        .ToHashSet();
    
    [MenuItem("CustomEditors/SOCreator")]
    public static void OpenWindow()
    {
        WindowExtension.OpenWindow<SOCreator>(new Vector2(800,600),new GUIContent("SOCreator"));
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(false); //不支持多选
        tree.Config.DrawSearchToolbar = true; //开启搜索状态
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle; //菜单设置成树形模式

        //筛选所有非抽象的类 并获取对应的路径
        tree.AddRange(scriptableObjectTypes.Where(x => !x.IsAbstract), GetMenuPathForType).AddThumbnailIcons();
        tree.Selection.SelectionChanged += e =>
        {
            //每当选择发生更改时发生进行回调2次，一次SelectionCleared 一次是ItemAdded
            if (this.previewObject && !AssetDatabase.Contains(this.previewObject))
            {
                DestroyImmediate(previewObject);
            }

            if (e != SelectionChangedType.ItemAdded)
            {
                return;
            }

            var t = SelectedType;
            if (t != null && !t.IsAbstract)
            {
                previewObject = ScriptableObject.CreateInstance(t) as ScriptableObject;
                saveName = MenuTree.Selection.First().Name;
                
                for (int i = saveName.Length - 1; i > -1 ; --i)
                {
                    if(saveName[i] == ' ')
                    {
                        saveName = saveName.Remove(i, 1);
                        i--;
                    }
                }
                
            }
            OnSelectionChange();
        };
        
        return tree;
    }
    
    protected override IEnumerable<object> GetTargets()
    {
        yield return previewObject;
    }
    
    /// <summary>
    /// 创建 ScriptableObject 时文件存储的目标文件夹
    /// </summary>
    private string targetFolder;
    private Vector2 scroll;
    protected override void DrawEditor(int index)
    {
        //scroll 内容滑动条的XY坐标
        scroll = GUILayout.BeginScrollView(scroll);
        {
            base.DrawEditor(index);
        }
        GUILayout.EndScrollView();

        if (this.previewObject)
        {
            GUILayout.FlexibleSpace(); 
            EditorGUILayout.LabelField($"Current Directory: {currentDirectory}", SirenixGUIStyles.BoldTitle);
            saveName = EditorGUILayout.TextField("Specify Name (Do NOT Add .asset): ", saveName);
            SirenixEditorGUI.HorizontalLineSeparator(5); 
            if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
            {
                CreateAsset();
            }
        }
    }
    
    private void OnSelectionChange()
    {
        currentDirectory = SelectionUtility.GetCurrentPath();
        targetFolder = currentDirectory.Trim('/');
    }
    
    private void CreateAsset()
    {
        if (previewObject)
        {
            bool isValid = CheckValid(SelectedType);
            if (!isValid)
            {
                return;
            }
            var dest = targetFolder + "/" + saveName + ".asset";
            dest = AssetDatabase.GenerateUniqueAssetPath(dest); //创建唯一路径 重名后缀 +1
            Debug.Log($"{previewObject} Created at {dest}");
            AssetDatabase.CreateAsset(previewObject, dest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = previewObject;
            //EditorApplication.delayCall += Close; //如不需要创建后自动关闭可将本行注释
        }
    }
    

    private bool CheckValid(Type type)
    {
        SOCreateLimitAttribute soCreateLimitAttribute = type.GetCustomAttribute<SOCreateLimitAttribute>();
        
        if (!typeof(ScriptableObject).IsAssignableFrom(type))
        {
            Debug.LogWarning($"类型 {type} 不是 ScriptableObject 派生类");
            return false;
        }

        string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");

        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, type);
            count++;
        }

        bool flag = true;
        if (soCreateLimitAttribute != null)
        {
            flag = soCreateLimitAttribute.soCreateCount > count;
        }
        return flag;
    }
    
    private string GetMenuPathForType(Type t)
    {
        if (t != null && scriptableObjectTypes.Contains(t))
        {
            var name = t.Name.Split('`').First().SplitPascalCase();
            return GetMenuPathForType(t.BaseType) + "/" + name;
        }
        return "";
    }
}