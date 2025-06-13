# Hex Tile Placement Snapper

This is a tiny script that can help you place the hexagon tiles into perfect positions if you are going to manually craft a level or world with hexagon tiles. With the tool enabled, when you change the positions  of objects in Unity editor, the object would automatically snap to the closest hex grid center. You can also adjust the grid size, number of grids, and the helper grid lines color to best fit your need.

To start to use it, add the "HexTilePlacementTool.prefab" into your scene, and make sure "Is Snapping Enabled" is on. Configure the grid size to be useful for you, usually it's either 1 or 1.732050807568877f, depending on how your hex grid model was modelled. (The two provided examples are only different in hex grid size.) Now, when you drag any game object(s) in the scene view, if the object is close enough to the grid, then it automatically snaps to the closest grid center. When you don't need this tool anymore, just simply disable this HexTilePlacementTool object, or delete it.

(Note that some actions may not trigger the snapping. At least moving the selected objects in the scene view will definitely trigger the snapping. Changing positions from the inspector will not trigger the snapping. )

Hope this can provide a little help to your development. If you have any questions, feel free to reach out to me :) qlgamedev@gmail.com 
