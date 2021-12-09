using System.Collections.Generic;
using UnityEngine;

//This file can be used as a 2in1 Template&Example

//These are purely game-dependent classes
namespace HexEditorEngine
{
    [CreateAssetMenu(fileName = "DoNothing", menuName = "Hex Editor Engine/Pallete behaviours/DoNothing", order = 5)]
    public class DoNothing : PaletteBehaviour
    {
        public override void InvokeCustomBehaviour(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedTilePrevious, MapTile baseTile, MapTile overlappedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost)
        {
            newGhost = ghost;
        }
    }


}