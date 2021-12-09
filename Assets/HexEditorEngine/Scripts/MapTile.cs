using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace HexEditorEngine
{
    /// <summary>
    /// A single map object
    /// </summary>    
    public class MapTile : MonoBehaviour
    {
        public MapTileData _data;
        public MapTileData data { get { dataChanged = true; return _data; } set { _data = value; dataChanged = true; } }
        bool dataChanged = false;
        Vector3 targetPosition=Vector3.zero;
        public Material holoMaterial;
        public bool placed = false;
        public Dictionary<MeshRenderer,List<Material>> meshRendererMaterials=new Dictionary<MeshRenderer, List<Material>>();

        public Vector3 globalPosition { get { return _data.coords.worldPosition().xz() + Vector3.up * HexGeneral.vScale * (_data.layer+_data.vOffset); } }

        [System.NonSerialized] public GameObject placeableHexObjectInstance;

        //dodać później wsparcie wysokości
        public void Start()
        {
            if (placeableHexObjectInstance != null)
            {
                Debug.LogWarning("Load called on existing object");
                return;
            }
            placeableHexObjectInstance = GameObject.Instantiate(_data.placeableHexObjectPrefabData.gameObject, this.transform.GetChild(0));
            //localRefToPHO = placeableHexObjectInstance.GetComponent<PlaceableHexObject>();
            Recalculate();
            transform.position = targetPosition;
            if (GetComponentInChildren<MeshCollider>() != null)
            {
                GetComponentInChildren<MeshCollider>().convex = true;
                GetComponentInChildren<MeshCollider>().isTrigger = true;
            }
            else
            {
                Debug.LogWarning("You might want to have some Collider on the hexObject. Automated Generation...");
                foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>().ToList())
                {
                    var mc=mr.gameObject.AddComponent<MeshCollider>();
                    mc.convex = true;
                    mc.isTrigger = true;
                }
            }
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>().ToList())
            {
                List<Material> materials = new List<Material>();
                mr.GetMaterials(materials);
                meshRendererMaterials.Add(mr, materials);
            }
            if (placed)
                Place();
            
        }
        //Runs delete animation, then deletes gameobject;
        //Returns null for making oneliners possible
        //  ex. ghost = ghost.Delete();
        public MapTile Delete()
        {
            GetComponentsInChildren<MeshCollider>().ToList().ForEach((mc) => { mc.isTrigger = true; });
            if (GetComponentInChildren<Animator>())
            {
                GetComponentInChildren<Animator>().SetTrigger($"Delete{Random.Range(1,5)}");
                GameObject.Destroy(this.gameObject, 2);
            }
            else
                GameObject.Destroy(this.gameObject);
            return null;
        }
        //For return type look Delete()
        public MapTile Place()
        {
            placed = true;
            GetComponentsInChildren<MeshCollider>().ToList().ForEach((mc)=> { mc.isTrigger = false; });
            /*if (GetComponentInChildren<MeshCollider>() != null)
                GetComponentInChildren<MeshCollider>().isTrigger = false;*/
            return null;
        }
        //For return type look Delete()
        public MapTile Pick()
        {
            placed = false;
            GetComponentsInChildren<MeshCollider>().ToList().ForEach((mc) => { mc.isTrigger = false; });
            /*if (GetComponentInChildren<MeshCollider>() != null)
                GetComponentInChildren<MeshCollider>().isTrigger = false;*/
            return this;
        }
        public void Recalculate()
        {
            targetPosition = globalPosition;
        }
        public void Update()
        {
            if (dataChanged)
                Recalculate();
            transform.position = Vector3.Lerp(transform.position, targetPosition, BuildingSettings.positionInterpolator);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, _data.rotation, 0), BuildingSettings.rotationInterpolator);
        }
        public void Hologrify()
        {
            foreach(var x in meshRendererMaterials)
            {
                List<Material> holoList = new List<Material>();
                foreach (var y in x.Value)
                    holoList.Add(holoMaterial);
                x.Key.materials = holoList.ToArray();
                x.Key.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        public void Dehologrify()
        {
            foreach (var x in meshRendererMaterials)
            {
                x.Key.materials = x.Value.ToArray();
                x.Key.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;   
            }
        }
    }
}