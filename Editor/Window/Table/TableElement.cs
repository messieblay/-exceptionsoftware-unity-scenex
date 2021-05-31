
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

                if (!name.ToLower().StartsWith("scen"))
                {
                    _type = Type.FolderGroups;
                }
            }
            else
            {
                if (item is SceneInfo)
                {
                    _type = Type.Scene;
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

        [UnityEngine.SerializeField] public bool IsFolder => _type == Type.FolderGroups || _type == Type.FolderScenes;
        [UnityEngine.SerializeField] public bool IsFolderScenes => _type == Type.FolderScenes;
        [UnityEngine.SerializeField] public bool IsFolderGroups => _type == Type.FolderGroups;
        [UnityEngine.SerializeField] public bool IsScene => _type == Type.Scene;
        [UnityEngine.SerializeField] public bool IsLayout => _type == Type.Layout;
        [UnityEngine.SerializeField] public bool IsGroup => _type == Type.Group;
        [UnityEngine.SerializeField] public bool IsSubGroup => _type == Type.SubGroup;

        //[UnityEngine.SerializeField] public bool IsFolder => item == null;
        //[UnityEngine.SerializeField] public bool IsFolderScenes => IsFolder && name.ToLower().StartsWith("scen");
        //[UnityEngine.SerializeField] public bool IsFolderGroups => IsFolder && !name.ToLower().StartsWith("scen");
        //[UnityEngine.SerializeField] public bool IsScene => IsFolder ? false : item is SceneInfo;
        //[UnityEngine.SerializeField] public bool IsLayout => IsFolder ? false : item is Layout;
        //[UnityEngine.SerializeField] public bool IsGroup => IsFolder ? false : item is Group;
        //[UnityEngine.SerializeField] public bool IsSubGroup => IsFolder ? false : item is SubGroup;

        public TableElement(string name, int depth, int id) : base(name, depth, id)
        {
        }

        public override string ToString() => name;


        public enum Type
        {
            Folder, FolderScenes, FolderGroups, Scene, Layout, Group, SubGroup
        }
    }
}
