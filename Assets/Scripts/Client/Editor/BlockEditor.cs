using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

namespace MC.Configurations
{
    public class BlockEditor : OdinMenuEditorWindow
    {
        [MenuItem("MineCraft/BlockEditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<BlockEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            BlockTypeOverview.Instance.UpdateBlockTypeOverview();
            tree.Add("Blocks", new BlockTable(BlockTypeOverview.Instance.AllBlockTypes));

            tree.AddAllAssetsAtPath("Blocks", "Assets/_minecraft/Block/Block", typeof(BlockType), true, true);

            tree.AddAllAssetsAtPath("", "Assets/_minecraft/Block/", typeof(BlockMeshData), true)
                .ForEach(this.AddDragHandles);
            tree.AddAllAssetsAtPath("", "Assets/_minecraft/Block/", typeof(Texture2D), true)
                .ForEach(this.AddDragHandles);

            tree.EnumerateTree().Where(x => x.Value as BlockType).ForEach(AddDragHandles);

            tree.EnumerateTree().AddIcons<BlockType>(x => x.blockProperties.Icon);
            tree.EnumerateTree().AddIcons<Texture2D>(x => x);

            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Block")))
                {
                    ScriptableObjectCreator.ShowDialog<BlockType>("Assets/_minecraft/Block/Block", obj =>
                    {
                        obj.blockProperties.Block_Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
            base.OnBeginDrawEditors();
        }
    }


    #region OBLockOverview
    [GlobalConfig("Assets/_minecraft/Block/Blocks")]
    public class BlockTypeOverview : GlobalConfig<BlockTypeOverview>
    {
        [ReadOnly]
        [ListDrawerSettings(ShowFoldout = true)]
        public BlockType[] AllBlockTypes;
#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateBlockTypeOverview()
        {
            this.AllBlockTypes = AssetDatabase.FindAssets("t:BlockType")
            .Select(guid => AssetDatabase.LoadAssetAtPath<BlockType>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
        }
#endif
    }
    #endregion


    #region BlockTable

    public class BlockTable
    {
        [FormerlySerializedAs("allBlocks")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private readonly List<BlockWrapper> allBlocks;

        public BlockType this[int index]
        {
            get { return this.allBlocks[index].BlockType; }
        }

        public BlockTable(IEnumerable<BlockType> blocks)
        {
            this.allBlocks = blocks.Select(x => new BlockWrapper(x)).ToList();
        }

        #region BlockWrapper
        private class BlockWrapper
        {
            private BlockType soBlock;

            public BlockType BlockType
            {
                get { return this.soBlock; }
            }

            public BlockWrapper(BlockType soBlock)
            {
                this.soBlock = soBlock;
            }

            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
            public Texture Icon { get { return this.soBlock.blockProperties.Icon; } set { this.soBlock.blockProperties.Icon = value; EditorUtility.SetDirty(this.soBlock); } }

            [TableColumnWidth(120)]
            [ShowInInspector]
            public string Name { get { return this.soBlock.blockProperties.Block_Name; } set { this.soBlock.blockProperties.Block_Name = value; EditorUtility.SetDirty(this.soBlock); } }

            [ShowInInspector]
            public BlockFlags BlockFlags { get { return this.soBlock.blockProperties.BlockFlags; } set { this.soBlock.blockProperties.BlockFlags = value; EditorUtility.SetDirty(this.soBlock); } }

            [ShowInInspector, ProgressBar(0, 1)]
            public float MoveResistance { get { return this.soBlock.blockProperties.MoveResistance; } set { this.soBlock.blockProperties.MoveResistance = value; EditorUtility.SetDirty(this.soBlock); } }
            [ShowInInspector, ProgressBar(0, 1)]
            public float LightOpacity { get { return this.soBlock.blockProperties.LightOpacity; } set { this.soBlock.blockProperties.LightOpacity = value; EditorUtility.SetDirty(this.soBlock); } }
            [ShowInInspector, ProgressBar(0, 10, 0, 1, 0, Segmented = true)]
            public int LightValue { get { return this.soBlock.blockProperties.LightValue; } set { this.soBlock.blockProperties.LightValue = value; EditorUtility.SetDirty(this.soBlock); } }
            [ShowInInspector, ProgressBar(-1, 10, 0, 1, 0, Segmented = true)]
            public int Hardness { get { return this.soBlock.blockProperties.Hardness; } set { this.soBlock.blockProperties.Hardness = value; EditorUtility.SetDirty(this.soBlock); } }
        }
        #endregion
    }

    #endregion
}