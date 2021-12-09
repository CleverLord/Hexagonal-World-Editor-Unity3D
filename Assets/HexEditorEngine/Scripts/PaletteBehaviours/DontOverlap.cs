using System.Collections.Generic;
using UnityEngine;

//This file can be used as a 2in1 Template&Example

//These are purely game-dependent classes
namespace HexEditorEngine
{
    [CreateAssetMenu(fileName = "DontOverlap", menuName = "Hex Editor Engine/Pallete behaviours/DontOverlap", order = 50)]
    public class DontOverlap : PaletteBehaviour
    {
        public override void InvokeCustomBehaviour(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedTilePrevious, MapTile baseTile, MapTile overlappedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost)
        {
            newGhost = ghost;

            if (!ghost || !ghost.data.samePositionAndLayer(newPosition) || focusedTile != focusedTilePrevious)
                newPosition.vOffset = baseTile ? baseTile.data.vOffset : focusedTile ? focusedTile.data.vOffset : 0;

            if (!ghost)
                ghost = newGhost = currentPalette.CreateGhost(newPosition);

            if (!overlappedTile)
            {
                ghost.data.copyTransform(newPosition);
            }
        }
    }
}