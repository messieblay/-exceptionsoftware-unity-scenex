using ExceptionSoftware.ExEditor;
using ExceptionSoftware.TreeViewTemplate;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static ExceptionSoftware.ExScenes.Table;

namespace ExceptionSoftware.ExScenes
{
    public class ScenexWindow : ExWindow<ScenexWindow>
    {
        [MenuItem("Tools/Scenes/Manager", priority = ExConstants.MENU_ITEM_PRIORITY)]
        public static ScenexWindow GetWindow()
        {

            //SceneListWindow currentWindow = (SceneListWindow)SceneListWindow.GetWindow<SceneListWindow>(new System.Type[] { wtype });

            var window = EditorWindow.GetWindow<ScenexWindow>();
            window.titleContent = new GUIContent("Scenex");
            window.Focus();
            window.Repaint();
            return window;
        }

        ScenexSettings _db = null;
        Rect[] _rectLayout = null;

        [SerializeField] TreeViewState m_TreeViewState;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        UnityEditor.IMGUI.Controls.SearchField m_SearchField;
        Table m_TreeView;

        protected override void DoEnable()
        {
            Rect rpos = base.position.CopyToZero();
            _rectLayout = rpos.Split(SplitMode.Vertical, 20, 41, -1);
            m_Initialized = false;

            ScenexUtilityEditor.onDataChanged -= ReupdateTable;
            ScenexUtilityEditor.onDataChanged += ReupdateTable;
        }

        protected override void DoDisable()
        {
            ScenexUtilityEditor.onDataChanged -= ReupdateTable;
        }

        protected override void DoResize()
        {
            Rect rpos = base.position.CopyToZero();
            _rectLayout = rpos.Split(SplitMode.Vertical, 20, 41, -1);
        }
        protected override void DoRecompile() => DoEnable();
        //private void OnFocus() => ReupdateTable();

        [System.NonSerialized] bool m_Initialized = false;
        void InitIfNeedIt()
        {
            if (_db == null)
            {
                _db = ScenexUtilityEditor.Settings;
            }

            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = Table.CreateDefaultMultiColumnHeaderState(_rectLayout[2].width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MyMultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<TableElement>(TableGenerator.GenerateTree());

                m_TreeView = new Table(m_TreeViewState, multiColumnHeader, treeModel);

                m_SearchField = new UnityEditor.IMGUI.Controls.SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

                m_TreeView.searchString = "";
                m_Initialized = true;
            }
            Undo.undoRedoPerformed -= ReupdateTable;
            Undo.undoRedoPerformed += ReupdateTable;
        }

        public void ReupdateTable()
        {
            if (m_TreeView == null) return;
            m_TreeView.SetSelection(new List<int>());
            m_TreeView.treeModel.SetData(TableGenerator.GenerateTree());
            m_TreeView.Reload();

        }

        public override void DoGUI()
        {
            InitIfNeedIt();
            DrawToolbar(_rectLayout[0]);
            DragBar(_rectLayout[1]);
            DoTreeView(_rectLayout[2]);
            DragAndDropScenes();
        }


        void DrawToolbar(Rect r)
        {
            GUILayout.BeginArea(r, EditorStyles.toolbar);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    ReupdateTable();
                }
                GUILayout.FlexibleSpace();
                if (Application.isPlaying)
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button("Stop", EditorStyles.toolbarButton))
                    {
                        ScenexUtilityEditor.StopEditor();
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("Play", EditorStyles.toolbarButton))
                    {
                        ScenexUtilityEditor.PlayEditor();
                    }
                    GUI.color = Color.white;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    ScenexUtilityEditor.PublishToBuildSettings();
                    ScenexUtilityEditor.CreateScenexEnumToLoadScenes();
                    EditorUtility.SetDirty(_db);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void DragBar(Rect rect)
        {
            GUILayout.BeginArea(rect, "D&D Area: Drag here your game scenes", (GUIStyle)"SelectionRect");
            {

            }
            GUILayout.EndArea();
            //m_TreeView.Filter.textFilter = m_TreeView.searchString = m_SearchField.OnGUI(rect, m_TreeView.searchString).Trim();
        }
        void DragAndDropScenes()
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                // To consume drag data.
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
                {
                    Debug.Log("UnityAsset");
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        string path = DragAndDrop.paths[i];

                        if (obj is SceneAsset)
                        {
                            ScenexUtilityEditor.CreateScene(obj as SceneAsset);
                        }
                    }
                }
            }
        }
        void DoTreeView(Rect rect)
        {
            m_TreeView.OnGUI(rect);
        }
    }
}
