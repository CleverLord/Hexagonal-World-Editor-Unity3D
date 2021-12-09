using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HexEditorEngine
{
    /// <summary>
    /// Collection of PlaceableHexObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Pallete", menuName = "Hex Editor Engine/Pallete", order = 2)]
    public class Palette : ScriptableObject
    {
        public List<GameObject> placeableHexObjects = new List<GameObject>();
        public List<PaletteBehaviour> paletteBehaviours = new List<PaletteBehaviour>();
        public GameObject currentPlaceableHexObject;
        public static GameObject mapTileCorePrefab;
        public void RemoveImporoperBrushes()
        {
            placeableHexObjects = placeableHexObjects.Where(b => b != null).ToList();
            paletteBehaviours = paletteBehaviours.Where(b => b != null).ToList();
        }
        public MapTile CreateGhost(MapTileData hitPointData)
        {
            MapTile ghost = Instantiate(mapTileCorePrefab).GetComponent<MapTile>();
            ghost.data = MapTileData.clonePosition(hitPointData);
            ghost.data.placeableHexObjectPrefabData = currentPlaceableHexObject.GetComponent<PlaceableHexObject>();
            ghost.name = $"MTCP_#{Random.Range(100, 999)}";
            return ghost;
        }
        public void InvokeCustomBehaviours(MapTileData newPosition, MapTile ghost, MapTile focusedTile, MapTile focusedPrevious, MapTile baseTile, MapTile obverlapedTile, Map map, List<Palette> palettes, Palette currentPalette, out MapTile newGhost)
        {
            MapTile localNewGhost = ghost;
            if (paletteBehaviours != null)
            {
                for (int i = 0; i < paletteBehaviours.Count; i++)
                {
                    paletteBehaviours[i].InvokeCustomBehaviour(newPosition, localNewGhost, focusedTile, focusedPrevious, baseTile, obverlapedTile, map, palettes, currentPalette, out localNewGhost);
                }
            }
            newGhost = localNewGhost;
        }
        //TODO (krzysiek): zrobić listę obiektów które można postawić
        //Skonfigurować je
        //Zrobić z nich nowy pendzel
        //Podłączyć pendzel

        //Dodać REGEX'a
    }
}