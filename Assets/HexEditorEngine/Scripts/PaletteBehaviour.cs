using System.Collections.Generic;
using UnityEngine;

namespace HexEditorEngine
{
    public abstract class PaletteBehaviour : ScriptableObject
    {
        public abstract void InvokeCustomBehaviour(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedTilePrevious, MapTile baseTile, MapTile overlappedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost);
        
    }
}