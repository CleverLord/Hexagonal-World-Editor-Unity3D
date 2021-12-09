using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace HexEditorEngine
{
    [System.Serializable]
    //[CreateAssetMenu(fileName = "New PlaceableHexObject", menuName = "HexEditorEngine/PlaceableHexObject", order = 1)]
    /// <summary>
    /// Provides info for building system
    /// </summary>
    public class PlaceableHexObject : SerializedMonoBehaviour
    {
        public Dictionary<HexEdge, string> feed=new Dictionary<HexEdge, string>();
    }
}   