using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class EditorMenuItemView
    {
        struct MenuItemInfo
        {
            public string Name;
            public string[] ItemNames;
            public Action<int> OnSelected;
            public float Width;
        }

        private List<MenuItemInfo> _menuItems = new List<MenuItemInfo>();

        public EditorMenuItemView SetMenuItem(string name, string[] itemNames, Action<int> onSelected,float width=60)
        {
            _menuItems.Add(new MenuItemInfo() { Name=name,ItemNames=itemNames,OnSelected=onSelected,Width=width});
            return this;
        }

        public void OnDrawLayout()
        {
            GUILayout.BeginHorizontal();
            foreach (var item in _menuItems)
            {
                if (GUILayout.Button(item.Name, EditorStyles.toolbarDropDown,GUILayout.Width(item.Width)))
                {
                    if (item.ItemNames != null && item.ItemNames.Length > 0)
                    {
                        GenericMenu gm = new GenericMenu();
                        for (int i = 0; i < item.ItemNames.Length; i++)
                        {
                            int index = i;
                            gm.AddItem(new GUIContent(item.ItemNames[i]), false, () => {
                                item.OnSelected?.Invoke(index);
                            });
                        }
                        gm.ShowAsContext();
                    }
                   
                   
                }
            }
            GUILayout.EndHorizontal();
        }

    }
}
