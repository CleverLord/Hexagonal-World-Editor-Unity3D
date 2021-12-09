using System.Collections;
using UnityEngine;

namespace HexEditorEngine
{
    [System.Serializable]
    public class MapTileData
    {
        /// <summary>
        /// 2D top down coordinates
        /// </summary>
        public AxialCoords coords = new AxialCoords();
        /// <summary>
        /// vertical offset, for terrain variety
        /// </summary>
        public float vOffset;
        public int layer;
        public int rotation;
        public PlaceableHexObject placeableHexObjectPrefabData;
        public static MapTileData clonePosition(MapTileData from)
        {
            return new MapTileData()
            {
                coords = from.coords,
                vOffset = from.vOffset,
                layer = from.layer,
                rotation = from.rotation,
            };
        }
        public bool sameTransform(MapTileData other)
        {
            if (coords != other.coords)
                return false;
            if (vOffset != other.vOffset)
                return false;
            if (layer != other.layer)
                return false;
            if (rotation != other.rotation)
                return false;
            return true;
        }
        public void copyTransform(MapTileData other)
        {
            coords = other.coords;
            vOffset = other.vOffset;
            layer = other.layer;
            rotation = other.rotation;
        }
        public bool sameTransformSkipVOffset(MapTileData other)
        {
            if (coords != other.coords)
                return false;
            if (layer != other.layer)
                return false;
            if (rotation != other.rotation)
                return false;
            return true;
        }
        public bool samePositionAndLayer(MapTileData other)
        {
            if (coords != other.coords)
                return false;
            if (layer != other.layer)
                return false;
            return true;
        }

        public override string ToString()
        {
            return "{" + coords.ToString() + " ,layer: " + layer + " ,rotation: " + rotation + "}";
        }
    }
    public enum HexEdge { Top, TopRight, BottomRight, Bottom, BottomLeft, TopLeft }

    public static class Extensions
    {
        public static Vector3 xz(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }
    }
}