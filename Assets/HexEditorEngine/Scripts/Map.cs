using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace HexEditorEngine
{
    /// <summary>
    /// Provides a saveable structure
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Map", menuName = "Hex Editor Engine/Map", order = 1)]
    public class Map : ScriptableObject
    {
        public List<MapTileData> mapTilesDatas = new List<MapTileData>();
        [System.NonSerialized] public List<MapTile> mapTiles = new List<MapTile>();
        
        public void AddMapTile(MapTile mt)
        {
            mapTilesDatas.Add(mt.data);
            mapTiles.Add(mt);
        }
        public void RemoveMapTile(MapTile mt)
        {
            if (mt != null)
            {
                if (mapTiles.Contains(mt)) {
                    mapTiles.Remove(mt);
                    mapTilesDatas.Remove(mt.data);
                }
            }
        }
        public void Delete(bool instant = false)
        {
            foreach(MapTile mt in mapTiles)
            {
                if (instant)
                    Destroy(mt.gameObject);
                else
                    mt.Delete();
            }
        }
        public MapTile GetTile(AxialCoords coords, int layer)
        {
            return mapTiles.FirstOrDefault(mt => mt.data.coords == coords && mt.data.layer==layer);
        }
        public Tuple<Dictionary<HexEdge, string>,string> getFeed(AxialCoords coords, int layer)
        {
            Dictionary<HexEdge, string> answer = new Dictionary<HexEdge, string>();

            Dictionary<AxialCoords, MapTileData> neighbors = new Dictionary<AxialCoords, MapTileData>();
            //Ustalić na jakich współrzędnych są sąsiedzi
            Dictionary<AxialCoords, HexEdge> neighbordCoordsEdgePairs = new Dictionary<AxialCoords, HexEdge>();
            for (int i = 0; i < 6; i++)
                neighbordCoordsEdgePairs.Add(axialOffsetToNeighbourByEdge((HexEdge)i)+coords, (HexEdge)i);

            //Optymalizacja przeszukiwania hex tile'i;
            List<AxialCoords> neighbordCoords = neighbordCoordsEdgePairs.Keys.ToList();

            foreach (MapTileData mtd in mapTilesDatas)
            {
                if (neighbordCoords.Contains(mtd.coords))
                {
                    neighbors.Add(mtd.coords, mtd);
                }
            }
            //Tu już wiem jacy są sąsiedzi i wiem gdzie są, ale nie wiem gdzie są względem mnie

            //Spięcie krawędzi z sąsiadami
            Dictionary<HexEdge, MapTileData> neighbordEdgePairs = new Dictionary<HexEdge, MapTileData>();
            foreach (var kvp in neighbors)
            {
                HexEdge he = neighbordCoordsEdgePairs[kvp.Key];
                neighbordEdgePairs.Add(he, kvp.Value);
            }
            //Przekodowanie na feed
            //Obrót jest przeciwnie do ruchu wskazówek zegara
            foreach(KeyValuePair<HexEdge, MapTileData> kvp in neighbordEdgePairs)
            {
                MapTileData neighbor = kvp.Value;
                //Kolejne wartości HexEdge są obrócone o 60 stopni zgodnie z r.w.z 
                //Kolejne 60tki rotation obracają obiekt o 60 stopni przeciwnie do r.w.z
                //Więc jak mamy probrać coś z elementu na górze, który ma rotation 60
                //To bierzemy patrzymy się na output tego jest w lokalnej górze, która globalnie jest góraLewo
                //Następnie dodajemy 60/60 co daje nam lokalnie góraPrawo, co globalnie jest górą
                //A na końcu przechodzimy na przeciwną stronę, bo jak chcemy pobrać coś z góry
                //To pobieramy to co na górze jest globalnym dołem, czyli lokalnie to jest bottom left
                HexEdge relativeEdge = (HexEdge)(((int)kvp.Key + neighbor.rotation / 60 + 3) % 6);
                //A następnie do klucza "z góry" wpisujemy feed z wyliczonego kierunku
                if(kvp.Value.placeableHexObjectPrefabData.feed.ContainsKey(relativeEdge))
                    answer[kvp.Key] = kvp.Value.placeableHexObjectPrefabData.feed[relativeEdge];
            }
            MapTile bottomTile = GetTile(coords, layer - 1);
            return new Tuple<Dictionary<HexEdge, string>, string> ( answer,"xd" );
        }
        public static AxialCoords axialOffsetToNeighbourByEdge(HexEdge edge)
        {
            switch (edge)
            {
                case HexEdge.Top:           return new AxialCoords(0, 1);
                case HexEdge.TopRight:      return new AxialCoords(1, 0);
                case HexEdge.TopLeft:       return new AxialCoords(-1, 1);
                case HexEdge.Bottom:        return new AxialCoords(0, -1);
                case HexEdge.BottomRight:   return new AxialCoords(1, -1);
                case HexEdge.BottomLeft:    return new AxialCoords(-1, 0);
                default:                    return new AxialCoords();
            }
        }
        public void Regenerate()
        {
            GameObject mapTileDataCore= Resources.Load<GameObject>("MapTileCorePrefab");
            foreach (MapTileData mtd in mapTilesDatas)
            {
                MapTile mt = Instantiate(mapTileDataCore).GetComponent<MapTile>();
                mt.data = mtd;
                mt.placed = true;
                mapTiles.Add(mt);
            }

        }
    }
}   