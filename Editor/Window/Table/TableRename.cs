using ExceptionSoftware.TreeViewTemplate;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public partial class Table
    {
        // Rename
        //--------

        protected override bool CanRename(TreeViewItem item)
        {
            if (item is TreeViewItem<TableElement>)
            {
                return (item as TreeViewItem<TableElement>).data.IsGroup || (item as TreeViewItem<TableElement>).data.IsSubGroup;
            }
            return false;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (args.acceptedRename)
            {
                var element = treeModel.Find(args.itemID);
                ExAssets.RenameAsset(element.item, args.newName);
                treeModel.SetData(TableGenerator.GenerateTree());
                Reload();
            }
        }

        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
            CenterRectUsingSingleLineHeight(ref cellRect);
            return base.GetRenameRect(cellRect, row, item);
        }

    }
}
