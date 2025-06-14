using UnityEngine;
using UnityEditor;

namespace HexTilePlacementSnapper
{
    // some customizations for HexTileSnapper in inspector
    #if UNITY_EDITOR
    [CustomEditor(typeof(HexTileSnapper))]
    public class HexGridDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            HexTileSnapper hexGridSnapper = (HexTileSnapper)target;

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck() ||
                GUILayout.Button(new GUIContent("Refresh Grid visual", 
                "If in some cases, the grid visual does not update, click this to refresh." +
                "For example, changing the object that holds HexTileSnapper script position in the " +
                "inspector would need a manual refresh.")))
            {
                hexGridSnapper.ForceUpdate();
                SceneView.RepaintAll();
            }
        }
    }
    #endif
}