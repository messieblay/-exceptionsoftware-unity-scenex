using ExceptionSoftware.TreeViewTemplate;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ExceptionSoftware.ExScenes
{
    public partial class Table : TableWithModel<TableElement>
    {
        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;



        static Texture _folderIcon = EditorGUIUtility.IconContent("d_Folder Icon").image;
        static Texture _LoadingSceneIcon = EditorGUIUtility.IconContent("d_StreamingController Icon").image;
        static Texture _SceneIcon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image;
        static Texture _GroupIcon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image;
        static Texture _subGroupIcon = EditorGUIUtility.IconContent("d_ScriptableObject On Icon").image;

        // All columns
        enum ScenexTableColumns
        {
            //Id = 0,
            Name = 0,
            //priority,
            mainScene,
            waitForInput,
            canvasOrder,
            description
        }

        public enum SortOption
        {
            Name = 0,
            //priority,
            mainScene,
            waitForInput,
            canvasOrder,
            description
        }

        // Sort options per column
        SortOption[] m_SortOptions = {
            SortOption.Name,
            //SortOption.priority,
            SortOption.mainScene,
            SortOption.waitForInput,
            SortOption.canvasOrder,
            SortOption.description
        };

        public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
                throw new NullReferenceException("root");
            if (result == null)
                throw new NullReferenceException("result");

            result.Clear();

            if (root.children == null)
                return;

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (int i = root.children.Count - 1; i >= 0; i--)
                stack.Push(root.children[i]);

            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        public Table(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<TableElement> model) : base(state, multicolumnHeader, model)
        {
            Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(ScenexTableColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kToggleWidth;

            Reload();
        }


        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<TableElement>)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (ScenexTableColumns)args.GetColumn(i), ref args);
            }
        }

        int _priority;
        void CellGUI(Rect cellRect, TreeViewItem<TableElement> item, ScenexTableColumns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case ScenexTableColumns.Name:
                    {
                        Rect toggleRect = cellRect;
                        toggleRect.x += GetContentIndent(item);
                        toggleRect.width = kToggleWidth;

                        if (toggleRect.xMax < cellRect.xMax)
                        {
                            //GUI.DrawTexture(toggleRect, EditorGUIUtility.FindTexture("d_cs Script Icon"), ScaleMode.ScaleToFit);
                            //GUI.DrawTexture(toggleRect, EditorGUIUtility.FindTexture("d_ScriptableObject On Icon"), ScaleMode.ScaleToFit);

                            if (item.data.IsFolder)
                                GUI.DrawTexture(toggleRect, _folderIcon, ScaleMode.ScaleToFit);
                            else if (item.data.IsScene)
                                GUI.DrawTexture(toggleRect, _SceneIcon, ScaleMode.ScaleToFit);
                            else if (item.data.IsLoadingScene)
                                GUI.DrawTexture(toggleRect, _LoadingSceneIcon, ScaleMode.ScaleToFit);
                            else if (item.data.IsGroup)
                                GUI.DrawTexture(toggleRect, _GroupIcon, ScaleMode.ScaleToFit);
                            else if (item.data.IsSubGroup)
                                GUI.DrawTexture(toggleRect, _subGroupIcon, ScaleMode.ScaleToFit);
                        }

                        //Default icon and label
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    }
                    break;
            }

            if (item.data.IsFolder || item.data.item == null)
                return;

            switch (column)
            {
                //case ScenexTableColumns.priority:

                //    EditorGUI.BeginChangeCheck();
                //    _priority = EditorGUI.IntField(cellRect, item.data.item.priority);
                //    if (EditorGUI.EndChangeCheck())
                //    {
                //        item.data.item.priority = _priority;
                //    }
                //    break;
                case ScenexTableColumns.mainScene:
                    if (item.data.IsScene)
                    {
                        EditorGUI.BeginChangeCheck();
                        var waitForInput = EditorGUI.Toggle(cellRect, (item.data.item as SceneInfo).isMainScene);
                        if (EditorGUI.EndChangeCheck())
                        {
                            (item.data.item as SceneInfo).isMainScene = waitForInput;
                            EditorUtility.SetDirty(item.data.item);
                        }
                    }
                    break;
                case ScenexTableColumns.canvasOrder:

                    if (item.data.IsScene)
                    {

                        EditorGUI.BeginChangeCheck();
                        var order = EditorGUI.IntField(cellRect, (item.data.item as SceneInfo).canvasSortOrder);
                        if (EditorGUI.EndChangeCheck())
                        {
                            (item.data.item as SceneInfo).canvasSortOrder = order;
                            EditorUtility.SetDirty(item.data.item);
                        }
                    }

                    break;
                case ScenexTableColumns.description:
                    EditorGUI.BeginChangeCheck();
                    var description = EditorGUI.DelayedTextField(cellRect, item.data.item.description);
                    if (EditorGUI.EndChangeCheck())
                    {
                        item.data.item.description = description;
                        EditorUtility.SetDirty(item.data.item);
                    }
                    break;
                case ScenexTableColumns.waitForInput:
                    if (item.data.IsGroup)
                    {
                        EditorGUI.BeginChangeCheck();
                        var waitForInput = EditorGUI.Toggle(cellRect, (item.data.item as Group).waitForInput);
                        if (EditorGUI.EndChangeCheck())
                        {
                            (item.data.item as Group).waitForInput = waitForInput;
                            EditorUtility.SetDirty(item.data.item);
                        }
                    }
                    break;
            }



        }


        // Misc
        //--------

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        //static float 
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[] {
                 
                //NAME
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent (ScenexTableColumns.Name.ToString()),
                    contextMenuText = "Name",
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    minWidth = 200,
                    width = 200,
                    autoResize = true,
                    allowToggleVisibility = true
                },
                //Priority
                //new MultiColumnHeaderState.Column {
                //    headerContent = new GUIContent (ScenexTableColumns.priority.ToString()),
                //    contextMenuText = "Priority",
                //    headerTextAlignment = TextAlignment.Left,
                //    sortedAscending = false,
                //    sortingArrowAlignment = TextAlignment.Right,
                //    minWidth = 50,
                //    autoResize = false,
                //    allowToggleVisibility = true
                //},
                //keylist
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent ("Main Scene"),
                    contextMenuText = "Main Scene",
                    headerTextAlignment = TextAlignment.Left,
                    canSort=false,
                    width = 100,
                    minWidth = 50,
                    maxWidth=100,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                
                //STATE
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent("Wait For Input"),
                    contextMenuText = "waitForInput",
                    headerTextAlignment = TextAlignment.Left,
                    canSort=false,
                    width = 100,
                    minWidth = 50,
                    maxWidth=100,
                    autoResize = false,
                },
                //TYPEfILE
                new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent ("Canvas Order"),
                    contextMenuText = "canvasOrder",
                    headerTextAlignment = TextAlignment.Left,
                    canSort=false,
                    width = 100,
                    minWidth = 50,
                    maxWidth=100,
                    autoResize = false,
                    allowToggleVisibility = false
                },
              new MultiColumnHeaderState.Column {
                    headerContent = new GUIContent ("Description"),
                    contextMenuText = "description",
                    headerTextAlignment = TextAlignment.Left,
                    canSort=false,
                    minWidth = 100,
                    width = 150,
                    autoResize = true,
                    allowToggleVisibility = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(ScenexTableColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }


        internal class MyMultiColumnHeader : MultiColumnHeader
        {
            Mode m_Mode;

            public enum Mode
            {
                LargeHeader,
                DefaultHeader,
                MinimumHeaderWithoutSorting
            }

            public MyMultiColumnHeader(MultiColumnHeaderState state) : base(state)
            {
                mode = Mode.DefaultHeader;
            }

            public Mode mode
            {
                get { return m_Mode; }
                set
                {
                    m_Mode = value;
                    switch (m_Mode)
                    {
                        case Mode.LargeHeader:
                            canSort = true;
                            height = 37f;
                            break;
                        case Mode.DefaultHeader:
                            canSort = true;
                            height = DefaultGUI.defaultHeight;
                            break;
                        case Mode.MinimumHeaderWithoutSorting:
                            canSort = false;
                            height = DefaultGUI.minimumHeight;
                            break;
                    }
                }
            }

            protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
            {
                // Default column header gui
                base.ColumnHeaderGUI(column, headerRect, columnIndex);

                // Add additional info for large header
                if (mode == Mode.LargeHeader)
                {
                    // Show example overlay stuff on some of the columns
                    if (columnIndex > 2)
                    {
                        headerRect.xMax -= 3f;
                        var oldAlignment = EditorStyles.largeLabel.alignment;
                        EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                        GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
                        EditorStyles.largeLabel.alignment = oldAlignment;
                    }
                }
            }
        }
    }


}
