using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vive.Plugin.SR.Experience
{
    public class ViveSR_Experience_SemanticDrawer : MonoBehaviour
    {
        public enum RaycastMode
        {
            ValidHit,
            InvalidHit,
            NoHit
        }
        [SerializeField] GameObject Player;
        [SerializeField] GameObject RaycastStartPoint;
        [SerializeField] LineRenderer lineRenderer;
        public GameObject boundingBox;

        ViveSR_Experience_StaticMesh StaticMeshScript;
        RaycastMode raycastMode = RaycastMode.NoHit;
        ViveSR_StaticColliderInfo hitCldInfo = null;
        RaycastHit hit;

        public bool enablePlacer { get; set; }
        private GameObject _defaultPlacedObject;
        private List<GameObject> _placedObjects = new List<GameObject>();
        public float placedObjScale = 0.03f;

        bool isTriggerDown;
        Color lightRed, lightGreen;

        private void Awake()
        {
            StaticMeshScript = FindObjectOfType<ViveSR_Experience_StaticMesh>();
            lightRed = new Color(1f, 0.5f, 0.5f, 1f);
            lightGreen = new Color(0.5f, 1f, 0.5f, 1f);
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                Debug.Log("TEST");

                _defaultPlacedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _defaultPlacedObject.hideFlags = HideFlags.HideInHierarchy;
                Destroy(_defaultPlacedObject.GetComponent<Collider>());
                MeshRenderer rend = _defaultPlacedObject.GetComponent<MeshRenderer>();
                rend.material.shader = Shader.Find("Unlit/Color");
                rend.material.SetColor("_Color", Color.red);
                rend.enabled = false;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (isTriggerDown)
            {
                Debug.Log("TriggerDown");
                UpdateRaycastMode();
                UpdateRaycastLine();
            }
            //UpdateIconLookAtPlayer();
        }
        //private void OnEnable()
        //{
        //    ImportSceneObjects();
        //}

        //private void OnDisable()
        //{
        //    ClearAllObjects();
        //}

        //private void OnDestroy()
        //{
        //    if (_defaultPlacedObject)
        //        Destroy(_defaultPlacedObject);

        //    DestroyAllObjects();
        //}

        void UpdateRaycastMode()
        {
            Vector3 forward = RaycastStartPoint.transform.forward;
            Vector3 startPos = RaycastStartPoint.transform.position;

            Physics.Raycast(startPos, forward, out hit);
            //if (hit.collider)
            //{
            //    ViveSR_StaticColliderInfo cldInfo = hit.collider.gameObject.GetComponent<ViveSR_StaticColliderInfo>();
            //    if (CheckValidHit(hit, cldInfo))
            //    {
            //        raycastMode = RaycastMode.ValidHit;
            //        hitCldInfo = cldInfo;
            //    }
            //    else
            //    {
            //        raycastMode = RaycastMode.InvalidHit;
            //    }
            //}
            //else
            //{
            //    raycastMode = RaycastMode.NoHit;
            //}
        }

        void UpdateRaycastLine()
        {
            lineRenderer.SetPosition(0, RaycastStartPoint.transform.position);
            Vector3 targetPos;

            //if (raycastMode == RaycastMode.NoHit)
            //{
            //    lineRenderer.startColor = lightRed;
            //    lineRenderer.endColor = Color.red;
            targetPos = RaycastStartPoint.transform.position + RaycastStartPoint.transform.forward * 10.0f;
            //}
            //else if (raycastMode == RaycastMode.InvalidHit)
            //{
            //    lineRenderer.startColor = lightRed;
            //    lineRenderer.endColor = Color.red;
            //    targetPos = hit.point;
            //}
            //else //validHit
            //{
            //    lineRenderer.startColor = lightGreen;
            //    lineRenderer.endColor = Color.green;
            //    targetPos = hit.point;
            //}
            lineRenderer.SetPosition(1, targetPos);
            boundingBox.transform.position = targetPos;
            boundingBox.transform.forward = RaycastStartPoint.transform.forward;
        }

        //private void UpdateIconLookAtPlayer()
        //{
        //    ViveSR_SceneUnderstanding.SetIconLookAtPlayer(Player.transform);
        //}

        public void TriggerPressDown()
        {
            //ClearAllObjects();
            isTriggerDown = true; // to update raycast mode at runtime
            lineRenderer.enabled = true;
            boundingBox.SetActive(true);
        }

        public void TriggerPressUp()
        //public SceneUnderstandingObjectType TriggerPressUp()
        {
            SceneUnderstandingObjectType type = SceneUnderstandingObjectType.NONE;
            isTriggerDown = false;
            lineRenderer.enabled = false;
            boundingBox.SetActive(false);
            //if (raycastMode == RaycastMode.ValidHit)
            //{
            //    ViveSR_StaticColliderPool cldPool = hitCldInfo.gameObject.transform.root.GetComponent<ViveSR_StaticColliderPool>();
            //    type = hitCldInfo.SemanticType;

            //    if (enablePlacer) PlaceObjects(ViveSR_SceneUnderstanding.GetPlacedPositionsByID((int)type, hitCldInfo.SceneObjectID));

            //    cldPool.ShowAllColliderWithPropsAndCondition(new uint[] { (uint)ColliderShapeType.MESH_SHAPE });
            //    ViveSR_SceneUnderstanding.ShowSemanticBoundingBoxAndIconWithId((int)type, hitCldInfo.SceneObjectID, true, true);
            //}
            //return type;
        }

    //    public bool ShowBoundingBoxAndIconByType(int objType, bool boxIsVisible, bool iconIsVisible)
    //    {
    //        bool found = ViveSR_SceneUnderstanding.ShowSemanticBoundingBoxAndIconWithType(objType, boxIsVisible, iconIsVisible);
    //        if (enablePlacer) PlaceObjects(ViveSR_SceneUnderstanding.GetPlacedPositionsByType(objType));

    //        return found;
    //    }

    //    private void PlaceObjects(List<Vector3> placed_positions)
    //    {
    //        for (int i = 0; i < placed_positions.Count; i++)
    //        {
    //            GameObject placedObj = GameObject.Instantiate(_defaultPlacedObject);
    //            placedObj.GetComponent<MeshRenderer>().enabled = true;
    //            placedObj.transform.localScale = new Vector3(placedObjScale, placedObjScale, placedObjScale);
    //            placedObj.transform.position = placed_positions[i];
    //            _placedObjects.Add(placedObj);
    //        }
    //    }

    //    private void DestroyPlacedObjects()
    //    {
    //        if (_placedObjects.Count > 0)
    //        {
    //            foreach (GameObject go in _placedObjects) Destroy(go);
    //            _placedObjects.Clear();
    //        }
    //    }

    //    private void ImportSceneObjects()
    //    {
    //        ViveSR_SceneUnderstanding.ImportSceneObjects("Recons3DAsset/SceneUnderstanding");
    //    }

    //    public void DestroyAllObjects()
    //    {
    //        ViveSR_SceneUnderstanding.DestroySceneObjects();
    //    }

    //    public void ClearAllObjects()
    //    {
    //        DestroyPlacedObjects();
    //        ViveSR_SceneUnderstanding.HideAllSemanticBoundingBoxAndIcon();
    //        StaticMeshScript.HideAllSemanticCollider();
    //    }

    //    public void ShowAllObjects()
    //    {
    //        ViveSR_SceneUnderstanding.ShowAllSemanticBoundingBoxAndIcon();
    //        StaticMeshScript.ShowAllSemanticCollider();
    //        if (enablePlacer) PlaceObjects(ViveSR_SceneUnderstanding.GetAllPlacedPositions());
    //    }

    //    bool CheckValidHit(RaycastHit hitInfo, ViveSR_StaticColliderInfo cldInfo)
    //    {
    //        if (hitInfo.collider != null && cldInfo != null && cldInfo.SemanticType != SceneUnderstandingObjectType.NONE)
    //            return true;

    //        return false;
    //    }
    }
}