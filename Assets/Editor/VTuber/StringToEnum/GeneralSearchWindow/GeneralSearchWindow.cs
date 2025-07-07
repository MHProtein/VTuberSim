using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.VTuber.StringToEnum.GeneralSearchWindow
{
    public class GeneralSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private List<string> m_menus;
        private Action<int> m_actions;

        public void Init(List<string> menus, Action<int> actions)
        {
            m_menus = menus;
            m_actions = actions;
        }
    
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(), 0));
            List<SearchMenuItem> mainMenu = new List<SearchMenuItem>();

            foreach (string type in m_menus)
            {
                string nodePath = type;
                if (nodePath == null) continue;
                string[] menus = nodePath.Split('/');
                List<SearchMenuItem> currentFloor = mainMenu;

                for (int i = 0; i < menus.Length; i++)
                {
                    string currentName = menus[i];
                    bool exist = currentFloor.Exists(item => item.Name.Equals(currentName));
                
                    if (!exist)
                    {
                        SearchMenuItem item = new SearchMenuItem() { Name = currentName };
                        currentFloor.Add(item);
                    }
                }
            }
            MakeSearchTree(mainMenu, 1, ref searchTreeEntries);
            return searchTreeEntries;
        }

        private void MakeSearchTree(List<SearchMenuItem> floor, int floorIndex, ref List<SearchTreeEntry> treeEntries)
        {
            foreach (var item in floor)
            {
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(item.Name))
                {
                    userData = item.Name,
                    level = floorIndex
                };
                treeEntries.Add(entry);
            }
        }
    
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            int index = m_menus.FindIndex((i) => i.Equals(SearchTreeEntry.userData));
            m_actions?.Invoke(index);
            return true;
        }
    }
}
