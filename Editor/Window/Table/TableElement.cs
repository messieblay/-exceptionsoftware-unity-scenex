
using ExceptionSoftware.TreeViewTemplate;
using System;


namespace ExceptionSoftware.ExScenes
{

    [Serializable]
    public class TableElement : TreeElement
    {
        [UnityEngine.SerializeField] Item _item = null;
        [UnityEngine.SerializeField]
        public Item item
        {
            get => _item;
            set
            {
                _item = value;
                CheckType();
            }
        }
        public Layout AsLayout => item as Layout;
        void CheckType()
        {
            if (item == null)
            {
                if (name.ToLower().StartsWith("scen"))
                {
                    _type = Type.FolderScenes;
                }

                if (name.ToLower().StartsWith("grou"))
                {
                    _type = Type.FolderGroups;
                }
                if (name.ToLower().StartsWith("load"))
                {
                    _type = Type.FolderLoading;
                }

            }
            else
            {
                if (item is SceneInfo)
                {
                    if ((item as SceneInfo).isLoadingScreen)
                    {
                        _type = Type.LoadingScreen;
                    }
                    else
                    {
                        _type = Type.Scene;
                    }
                }
                if (item is Layout)
                {
                    _type = Type.Layout;
                }
                if (item is Group)
                {
                    _type = Type.Group;
                }
                if (item is SubGroup)
                {
                    _type = Type.SubGroup;
                }
            }
        }

        [UnityEngine.SerializeField] Type _type = Type.Folder;
        public Type ItemType => _type;

        [UnityEngine.SerializeField] public bool IsFolder => _type == Type.FolderGroups || _type == Type.FolderScenes || _type == Type.FolderLoading;
        [UnityEngine.SerializeField] public bool IsFolderScenes => _type == Type.FolderScenes;
        [UnityEngine.SerializeField] public bool IsFolderGroups => _type == Type.FolderGroups;
        [UnityEngine.SerializeField] public bool IsFolderLoading => _type == Type.FolderLoading;
        [UnityEngine.SerializeField] public bool IsScene => _type == Type.Scene;
        [UnityEngine.SerializeField] public bool IsLoadingScene => _type == Type.LoadingScreen;
        [UnityEngine.SerializeField] public bool IsGroup => _type == Type.Group;
        [UnityEngine.SerializeField] public bool IsSubGroup => _type == Type.SubGroup;
        [UnityEngine.SerializeField] public bool IsLayout => IsSubGroup || IsGroup;

        public TableElement(string name, int depth, int id) : base(name, depth, id)
        {
        }

        public override string ToString() => name;


        public enum Type
        {
            Folder, FolderScenes, FolderGroups, FolderLoading, Scene, Layout, Group, SubGroup, LoadingScreen
        }
    }
}
