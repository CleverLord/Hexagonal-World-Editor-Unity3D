using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexEditorEngine
{
    //------------------------------ HEX -----------------------------//
    /// <summary>
    /// Provides structure for algorithms, since algorithms on Cubic coordinates are the easiest
    /// </summary>
    [System.Serializable]
    public struct HexCoords
    {
        public int x;//w prawo
        public int y;//w górê w lewo
        public int z;//w dó³ w lewo
        public HexCoords(int x,int y,int z) { this.x = x; this.y = y; this.z = z; }
        public AxialCoords toAxial()
        {
            return new AxialCoords()
            {
                q = this.x,
                r = this.z
            };
        }
    }
    /// <summary>
    /// This class exist just because why not
    /// </summary>
    [System.Serializable]
    public class HexCoordsFloating
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public HexCoordsFloating() { }
        public AxialCoordsFloating toAxial()
        {
            return new AxialCoordsFloating()
            {
                q = this.x,
                r = this.z
            };
        }
        public HexCoords rounded()
        {
            int rx = Mathf.RoundToInt(this.x);
            int ry = Mathf.RoundToInt(this.y);
            int rz = Mathf.RoundToInt(this.z);

            var x_diff = Mathf.Abs(rx - this.x);
            var y_diff = Mathf.Abs(ry - this.y);
            var z_diff = Mathf.Abs(rz - this.z);

            if (Mathf.Max(x_diff, y_diff, z_diff) == x_diff)
                return new HexCoords() { x = -ry - rz, y = ry, z = rz };
            else if (Mathf.Max(x_diff, y_diff, z_diff) == y_diff)
                return new HexCoords() { x = rx, y = -rx-rz, z = rz };
            else
                return new HexCoords() { x = rx, y = ry, z = -rx-ry };
        }
    }

    //------------------------------ AXIAL -----------------------------//
    /// <summary>
    /// Structure for storing coordinates for hexagons in optimal way
    /// </summary>
    [System.Serializable]
    public struct AxialCoords
    {
        public int q;// = 0; //column / w prawo
        public int r;// = 0; //row / lewo w dó³
        public AxialCoords(int q, int r) { this.q = q; this.r = r; }
        public HexCoords toHex()
        {
            return new HexCoords()
            {
                x = this.q,
                z = this.r,
                y = -this.q - this.r
            };
        }
        public override string ToString()
        {
            return $"(r:{r}, q:{q})";
        }
        public Vector2 worldPosition()
        {
            var x = (1.5f * this.q);
            var y = (HexGeneral.Sqrt3by2 * this.q + HexGeneral.Sqrt3 * this.r);
            return new Vector2(x, y) * HexGeneral.scale;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AxialCoords))
                return false;
            AxialCoords ac = (AxialCoords)obj;
            if (ac.q == this.q && ac.r == this.r)
                return true;
            return false;
        }

        public static bool operator != (AxialCoords ac1, AxialCoords ac2){
            return !ac1.Equals(ac2);
        }
        public static bool operator == (AxialCoords ac1, AxialCoords ac2){
            return ac1.Equals(ac2);
        }

        public static AxialCoords operator +(AxialCoords self,AxialCoords other)
        {
            return new AxialCoords(self.q+other.q,self.r+other.r);
        }
        public static AxialCoords operator -(AxialCoords self, AxialCoords other)
        {
            return new AxialCoords(self.q - other.q, self.r - other.r);
        }
    }
    /// <summary>
    /// Class mainly to convert world position to hexagonal position
    /// </summary>
    [System.Serializable]
    public class AxialCoordsFloating
    {
        public float q = 0;
        public float r = 0;
        public AxialCoordsFloating() { }
        public HexCoordsFloating toHex()
        {
            return new HexCoordsFloating()
            {
                x = this.q,
                z = this.r,
                y = -this.q - this.r
            };
        }
        public AxialCoords round()
        {
            return this.toHex().rounded().toAxial();
        }
    }
    
    public static class HexGeneral
    {
        /// <summary>
        /// distance from the center to verticle, or, base of the triangle
        /// </summary>
        public const float scale = 1 ;
        /// <summary>
        /// distance from the center to edge, or, height of the triangle
        /// </summary>
        public const float vScale = scale * Sqrt3by2 ; 
        public const float Sqrt3 = 1.7320508075688772935274463415059f;
        public const float Sqrt3by2 = 0.86602540378443864676372317075294f;
        public const float Sqrt3by3 = 0.57735026918962576450914878050196f;
        public static AxialCoords fromRaycastHit(Vector2 hitPoint)
        {
            //double size = 1;
            hitPoint /= scale;
            return new AxialCoordsFloating()
            {
                q = (2.0f / 3 * hitPoint.x),
                r = (-1.0f / 3 * hitPoint.x + Sqrt3by3 * hitPoint.y)
            }.round();

        }
    }
}