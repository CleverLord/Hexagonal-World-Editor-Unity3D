using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace HexEditorEngine
{
    public partial class CameraController
    {
        MapTileData hitPointData = new MapTileData() { layer = -1 };
        MapTileData hitPointDataPrevious = new MapTileData() { layer = -1 }; //pozycja na ghost'ie nie zawsze jest aktualizowana - szczególnie kiedy ignorujemy ruch podczas overlappingu
        MapTile focusedTile;
        MapTile focusedTilePrevious;
        MapTile overlappedTile;
        MapTile baseTile;
        public Map mapa;
        public List<Palette> palettes = new List<Palette>();
        Palette currentPalette;

        //public Dictionary<HexEdge, string> debugFeed;
        public MapTile ghost;

        bool initialized = false;
        // Start is called before the first frame update
        void StartHexEditor()
        {
            mapa = ScriptableObject.CreateInstance(typeof(Map)) as Map;
            palettes = palettes.Where(x => x != null).ToList();
            palettes.ForEach(p=>p.RemoveImporoperBrushes());
            if (palettes.Count == 0)
                return;
            currentPalette = palettes[0];
            currentPalette.currentPlaceableHexObject = currentPalette.placeableHexObjects[0];
            Palette.mapTileCorePrefab = Resources.Load<GameObject>("MapTileCorePrefab");
            initialized = true;
        }

        private void UpdateHexEditor()
        {
            if (!initialized)
                return;

            //focusedTile = null;
            overlappedTile = null;
            baseTile = null;

            if (!Input.GetMouseButton(1))
            {
                HandleRaycast();
                HandleInput();
            }
            if (Input.GetKeyDown(KeyCode.K))
                SaveMap();
            if (Input.GetKeyDown(KeyCode.L))
                LoadMap();
            ProcessRaycastAndInput();
            focusedTilePrevious = focusedTile;
            hitPointDataPrevious.copyTransform(hitPointData);
        }

        private void HandleRaycast()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, int.MaxValue, int.MaxValue, QueryTriggerInteraction.Ignore))
            {
                Vector3 hit3d = hit.point + hit.normal * 0.01f; // dodajê offset, ¿eby mo¿na by³o na œcianê hexa na jechaæ i siê nie psu³o
                Vector2 flatHit = new Vector2(hit3d.x, hit3d.z);
                AxialCoords hitPointCoords = HexGeneral.fromRaycastHit(flatHit);
                Vector3 vDirection = Vector3.Dot(Vector3.up, hit.normal) >= 0 ? Vector3.up : Vector3.down;

                GameObject tempHitObject = hit.collider.gameObject;

                focusedTile = tempHitObject.GetComponent<MapTile>();
                hitPointData.coords = hitPointCoords;

                while (focusedTile == null && tempHitObject.transform.parent != null)
                {
                    tempHitObject = tempHitObject.transform.parent.gameObject;
                    focusedTile = tempHitObject.GetComponent<MapTile>();
                }

                if (focusedTile != null)
                {
                    //map tile was hit
                    if (focusedTile.data.coords == hitPointCoords)
                    {
                        //flat surface was hit
                        if (vDirection == Vector3.up)
                            hitPointData.layer = focusedTile.data.layer + 1;
                        else
                            hitPointData.layer = focusedTile.data.layer - 1;
                    }
                    else
                    {
                        //vertical sufrace was hit
                        hitPointData.layer = focusedTile.data.layer;
                    }
                    overlappedTile = mapa.GetTile(hitPointData.coords, hitPointData.layer);
                    baseTile = mapa.GetTile(hitPointData.coords, hitPointData.layer - 1);
                }
                else
                    hitPointData.layer = 0;

            }
            else
            {
                focusedTile = null;
            }
        }

        private void HandleInput()
        {
            if (Input.mouseScrollDelta.y != 0 && !Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftShift))
                swapBrush(Input.mouseScrollDelta.y > 0 ? 1 : -1);
            if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.LeftAlt))
            {
                int delta = Input.mouseScrollDelta.y > 0 ? 1 : -1;
                hitPointData.vOffset = Mathf.Clamp(hitPointData.vOffset + delta * 0.1f, 0, 0.9f);
                if (ghost)
                    ghost.data.copyTransform(hitPointData);
            }
            if (Input.mouseScrollDelta.y != 0 && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt))
            {
                int delta = Input.mouseScrollDelta.y < 0 ? 1 : -1;
                hitPointData.rotation += 60 * delta + 360;
                hitPointData.rotation %= 360;
                if (ghost)
                    ghost.data.copyTransform(hitPointData);
            }

        }

        private void ProcessRaycastAndInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (focusedTile)
                    focusedTile.Hologrify();
                if (ghost)
                    ghost = ghost.Delete();
            }
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftControl))
            {
                //Delete mode
                if (focusedTile != focusedTilePrevious)
                {
                    //Update who is holo
                    if (focusedTilePrevious)
                        focusedTilePrevious.Dehologrify();
                    if (focusedTile)
                        focusedTile.Hologrify();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    //Delete target
                    /*if (focusedTile)
                    {
                        focusedTile.Delete();
                        focusedTile = focusedTilePrevious = null;
                    }*/
                    PickHex();
                    if (ghost)
                        ghost = ghost.Delete();
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (focusedTile)
                    focusedTile.Dehologrify();
                if (focusedTilePrevious)
                    focusedTilePrevious.Dehologrify();
            }
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if ( focusedTile != focusedTilePrevious || (ghost && !hitPointData.samePositionAndLayer(hitPointDataPrevious)) || !ghost)
                {
                    //Debug.Log("Update"+ hitPointData); //I'll keep this just in case i needed to check how many times sth is updated
                    if (currentPalette.paletteBehaviours.Count > 0)
                        currentPalette.InvokeCustomBehaviours(hitPointData, ghost, focusedTile, focusedTilePrevious, baseTile, overlappedTile, mapa, palettes, currentPalette, out ghost);
                    else
                        DefaultBehaviour();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (ghost)
                        PlaceHex();
                }
            }
        }

        public void DefaultBehaviour()
        {
            if (overlappedTile) return;
            else
            {
                if (!ghost)
                    CreateGhost();
                if (!hitPointData.samePositionAndLayer(hitPointDataPrevious) || focusedTilePrevious != focusedTile)
                    hitPointData.vOffset = baseTile ? baseTile.data.vOffset : focusedTile ? focusedTile.data.vOffset : 0;
                ghost.data.copyTransform(hitPointData);
            }
        }

        void CreateGhost()
        {
            if (ghost) return;
            ghost = currentPalette.CreateGhost(hitPointData);
        }
        /*void ReplaceGhost(MapTile newGhost)
        {
            ghost.Delete();
            newGhost = ghost;
            currentPalette.currentPlaceableHexObject = currentPalette.placeableHexObjects.First(pho => pho == newGhost.data.placeableHexObjectPrefabData.gameObject);
        }*/
       /* void RecalculatePosition(AxialCoords newCoords, Vector3 vDir)
        {
            //Ustawianie layera
            if (focusedTile != null)
                if (focusedTile.data.coords != newCoords)
                    hitPointData.layer = focusedTile.data.layer;
                else
                {
                    if (vDir == Vector3.up)
                        hitPointData.layer = focusedTile.data.layer + 1;
                    else
                        hitPointData.layer = focusedTile.data.layer - 1;
                }

            else
                hitPointData.layer = 0;

            //Ustawienie vOffsetu
            if (baseTile != null)
                hitPointData.vOffset = baseTile.data.vOffset;
            else if (overlappedTile != null)
                hitPointData.vOffset = overlappedTile.data.vOffset;
            else if (focusedTile != null)
                hitPointData.vOffset = focusedTile.data.vOffset;

            //Ustawienie wspó³rzêdnych
            hitPointData.coords = newCoords;
        }*/
        void swapBrush(int delta)
        {
            int idx = currentPalette.placeableHexObjects.IndexOf(currentPalette.currentPlaceableHexObject);
            idx += delta;
            while (idx < 0)
                idx += currentPalette.placeableHexObjects.Count();
            idx %= currentPalette.placeableHexObjects.Count();
            currentPalette.currentPlaceableHexObject = currentPalette.placeableHexObjects[idx];
            if (ghost)
                ghost = ghost.Delete();
            CreateGhost();
        }
        void PlaceHex()
        {
            if (!ghost) return;
            mapa.AddMapTile(ghost);
            ghost = ghost.Place();
        }
        void PickHex()
        {
            if (!focusedTile) return;
            mapa.RemoveMapTile(focusedTile);
            ghost = focusedTile.Pick();
        }
        void SaveMap()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Mapy"))
                AssetDatabase.CreateFolder("Assets/Resources", "Mapy");
            string mapIndexString = "001";
            int firstFreeMap = 1;
            while (AssetDatabase.FindAssets($"mapa_{firstFreeMap}", new string[] { "Assets/Resources/Mapy" }).Length != 0)
            {
                firstFreeMap++;
                mapIndexString = firstFreeMap.ToString();
                if (firstFreeMap < 100)
                    mapIndexString = "0" + mapIndexString;
                if (firstFreeMap < 10)
                    mapIndexString = "0" + mapIndexString;
            }
            AssetDatabase.CreateAsset(Instantiate(mapa), $"Assets/Resources/Mapy/mapa_{firstFreeMap}.asset");
            AssetDatabase.SaveAssets();
        }
        void LoadMap()
        {
            StartCoroutine(MapLoader());
        }
        IEnumerator MapLoader()
        {
            initialized = false;
            if (ghost != null)
                ghost.Delete();
            yield return new WaitForEndOfFrame();
            if (mapa)
                mapa.Delete();
            yield return new WaitForSeconds(1f);
            mapa = null;
            mapa = Resources.Load<Map>("Mapy/mapa");
            if (mapa != null)
            {
                mapa = Instantiate(mapa);
                mapa.Regenerate();
            }
            else
            {
                mapa = ScriptableObject.CreateInstance(typeof(Map)) as Map;
                Debug.Log("Mapa could not be loaded");
            }
            initialized = true;
        }
        private void OnDrawGizmos()
        {
            if (focusedTile != null)
            {
                Vector3 shift = focusedTile.globalPosition;
                Gizmos.color = Color.red;
                Vector3 pointOne = Vector3.right * HexGeneral.scale * 1.05f;
                Vector3 pointTwo = Quaternion.Euler(0, -60, 0) * pointOne;
                for (int i = 0; i < 6; i++)
                {
                    Gizmos.DrawLine(pointOne + shift, pointTwo + shift);
                    Gizmos.DrawLine(pointOne + shift, pointOne + shift + Vector3.up * HexGeneral.vScale / 2);
                    pointOne = Quaternion.Euler(0, -60, 0) * pointOne;
                    pointTwo = Quaternion.Euler(0, -60, 0) * pointTwo;
                }
            }
            if (baseTile != null)
            {
                Vector3 shift = baseTile.globalPosition;
                Gizmos.color = Color.green;
                Vector3 pointOne = Vector3.right * HexGeneral.scale * 0.99f;
                Vector3 pointTwo = Quaternion.Euler(0, -60, 0) * pointOne;
                for (int i = 0; i < 6; i++)
                {
                    Gizmos.DrawLine(pointOne + shift, pointTwo + shift);
                    Gizmos.DrawLine(pointOne + shift, pointOne + shift + Vector3.up * HexGeneral.vScale / 2);
                    pointOne = Quaternion.Euler(0, -60, 0) * pointOne;
                    pointTwo = Quaternion.Euler(0, -60, 0) * pointTwo;
                }
            }
            if (overlappedTile != null)
            {
                Vector3 shift = overlappedTile.globalPosition;
                Gizmos.color = Color.cyan;
                Vector3 pointOne = Vector3.right * HexGeneral.scale * 0.95f;
                Vector3 pointTwo = Quaternion.Euler(0, -60, 0) * pointOne;
                for (int i = 0; i < 6; i++)
                {
                    Gizmos.DrawLine(pointOne + shift, pointTwo + shift);
                    Gizmos.DrawLine(pointOne + shift, pointOne + shift + Vector3.up * HexGeneral.vScale / 2);
                    pointOne = Quaternion.Euler(0, -60, 0) * pointOne;
                    pointTwo = Quaternion.Euler(0, -60, 0) * pointTwo;
                }
            }
        }
    }
}