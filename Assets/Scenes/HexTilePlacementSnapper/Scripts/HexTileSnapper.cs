using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HexTilePlacementSnapper
{
    /// <summary>
    /// This is a small helper tool to help you place hexagon tiles in 3D view to the correct locations.
    /// It will snap your objects to the closest grid center positions, unless too far away.
    /// </summary>
    [ExecuteInEditMode]
    public class HexTileSnapper : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The size (length in x axis) of each hex grid tile. " +
            "Try with 1.732050807568877f if 1 does not work for you. It depends on the model.")]
        private float hexSize = 1f;

        [SerializeField]
        [Tooltip("The number of tiles the hex tile map contains in x axis.")]
        private int hexMapWidth = 10;

        [SerializeField]
        [Tooltip("The number of tiles the hex tile map contains in z axis.")]
        private int hexMapHeight = 10;

        [SerializeField]
        [Tooltip("The hexagon tiles color")]
        private Color gridTileColor = Color.white;

        [SerializeField]
        [Tooltip("You can disable snapping here")]
        private bool isSnappingEnabled = true;

        private const float HORIZONTAL_MULTIPLIER = 1.732050807568877f;
        private const float VERTICAL_MULTIPLIER = 1.5f;
        private float horizontalSpacing;
        private float verticalSpacing;
        private float hexLogicalSize;
        // Cache for performance
        private Vector3[] hexVertices = new Vector3[6];
        private List<Vector3> allLineVertices;
        private bool needsUpdate = true;

        /// <summary>
        /// Force the update
        /// </summary>
        public void ForceUpdate()
        {
            needsUpdate = true;
        }

        // Parameters change
        private void OnValidate()
        {
            needsUpdate = true;
            hexLogicalSize = hexSize / HORIZONTAL_MULTIPLIER;
            horizontalSpacing = hexSize;
            verticalSpacing = VERTICAL_MULTIPLIER * hexLogicalSize;
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            // disallow picking current object (so that no mis-selection and movement of this 
            // object if doing multi selections in the scene view.)
            SceneVisibilityManager.instance.DisablePicking(gameObject, true);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        // listen to event to trigger snapping
        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isSnappingEnabled) 
                return;

            Event currentEvent = Event.current;

            // Check something was being dragged and if mouse is up (so the dragging finishes)
            if (currentEvent.type == EventType.MouseUp &&
                Selection.gameObjects.Length > 0 &&
                Tools.current == Tool.Move)
            {
                SnapSelectedObjectsToGrid();
            }
        }

        // snap the selected objects to hex grid centers
        private void SnapSelectedObjectsToGrid()
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject == gameObject)
                {
                    needsUpdate = true; // need to update the hex tile visualization
                    continue; // Skip the grid itself
                }

                if (!selectedObject.activeSelf) 
                    continue;   // skip non-active ones

                Vector3 objectPos = selectedObject.transform.position;
                Vector3 localPos = objectPos - transform.position;

                // Convert position to hex coordinates
                float row = Mathf.Round(localPos.z / verticalSpacing);
                float col = Mathf.Round((localPos.x - (row % 2) * horizontalSpacing * 0.5f) / horizontalSpacing);

                // check if the object is too far away, if so, skip it
                if (row < -1 || row > hexMapHeight || col < -1 || col > hexMapWidth)
                {
                    continue;
                }

                // Clamp to grid bounds
                row = Mathf.Clamp(row, 0, hexMapHeight - 1);
                col = Mathf.Clamp(col, 0, hexMapWidth - 1);

                // Convert back to world position
                float offset = (row % 2) * (horizontalSpacing * 0.5f);
                Vector3 newPosition = transform.position + new Vector3(
                    col * horizontalSpacing + offset,
                    objectPos.y, // Preserve Y position
                    row * verticalSpacing
                );

                selectedObject.transform.position = newPosition;
            }
        }

        #region The drawer
        // calcualte hex vertices for each hex tile
        private void CalculateHexVertices(Vector3 center, List<Vector3> vertices)
        {
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                hexVertices[i] = center + new Vector3(
                    hexLogicalSize * Mathf.Sin(angle),
                    0f,
                    hexLogicalSize * Mathf.Cos(angle)
                );
            }

            // Add lines for this hex
            for (int i = 0; i < 6; i++)
            {
                vertices.Add(hexVertices[i]);
                vertices.Add(hexVertices[(i + 1) % 6]);
            }
        }

        // Avoid unnecessary calculations if nothing is changed
        private void CacheGridData()
        {
            if (!needsUpdate)
                return;

            allLineVertices = new List<Vector3>(hexMapWidth * hexMapHeight * 12); // 12 vertices per hex (6 lines * 2 points)

            for (int z = 0; z < hexMapHeight; z++)
            {
                for (int x = 0; x < hexMapWidth; x++)
                {
                    float offset = (z % 2) * (horizontalSpacing * 0.5f);
                    Vector3 center = new Vector3(
                        x * horizontalSpacing + offset,
                        0f,
                        z * verticalSpacing
                    ) + transform.position;

                    CalculateHexVertices(center, allLineVertices);
                }
            }

            needsUpdate = false;
        }

        // Called each frame. Draw the hexagons tiles
        private void OnDrawGizmos()
        {
            CacheGridData();

            // Draw all lines
            Gizmos.color = gridTileColor;
            for (int i = 0; i < allLineVertices.Count; i += 2)
            {
                Gizmos.DrawLine(allLineVertices[i], allLineVertices[i + 1]);
            }
        }
        #endregion
    }
}