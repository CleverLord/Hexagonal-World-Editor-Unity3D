using System.Collections.Generic;
using UnityEngine;

//This file can be used as a 2in1 Template&Example

//These are purely game-dependent classes
namespace HexEditorEngine
{
    [CreateAssetMenu(fileName = "DestroyWhenOverlapped", menuName = "Hex Editor Engine/Pallete behaviours/DestroyWhenOverlapped", order = 2)]
    public class DestroyWhenOverlapped : PaletteBehaviour
    {
        public override void InvokeCustomBehaviour(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedTilePrevious, MapTile baseTile, MapTile overlappedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost)
        {
            newGhost = ghost;
            if (overlappedTile)
            {
                if (ghost)
                    newGhost = ghost.Delete();
            }
            else
            {
                if (!ghost || ghost.data.coords != newPosition.coords || ghost.data.layer != newPosition.layer || focusedTile != focusedTilePrevious)
                    newPosition.vOffset = baseTile ? baseTile.data.vOffset : focusedTile ? focusedTile.data.vOffset : 0;

                if (!ghost)
                    ghost = newGhost = currentPalette.CreateGhost(newPosition);
                ghost.data.copyTransform(newPosition);
            }
        }
    }
}