using System.Collections.Generic;
using UnityEngine;

//This file can be used as a 2in1 Template&Example

//These are purely game-dependent classes
namespace HexEditorEngine
{
    [CreateAssetMenu(fileName = "GroundAndRoadBehaviour", menuName = "Hex Editor Engine/Pallete behaviours/GroundAndRoadBehaviour", order = 4)]
    public class GroundAndRoadBehaviour : PaletteBehaviour
    {
        public override void InvokeCustomBehaviour(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedTilePrevious, MapTile baseTile, MapTile overlappedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost)
        {
            newGhost = ghost;
            if (!ghost || ghost.data.coords != newPosition.coords || ghost.data.layer != newPosition.layer)
                newPosition.vOffset = baseTile ? baseTile.data.vOffset : focusedTile ? focusedTile.data.vOffset : 0;

            if (!ghost)
                ghost = newGhost = currentPalette.CreateGhost(newPosition);
            ghost.data.copyTransform(newPosition);
        }
    }


}