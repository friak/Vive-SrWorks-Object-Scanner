using UnityEngine;
using UnityEngine.UI;

namespace Vive.Plugin.SR.Experience
{
    [RequireComponent(typeof(ViveSR_Experience))]
    public class Sample9_SemanticSegmentation : MonoBehaviour
    {
        enum ActionMode
        {
            MeshControl,
            SemanticObjectControl,
            MaxNum
        }

        ActionMode actionMode = ActionMode.MeshControl;
        SceneUnderstandingObjectType currentSemanticType = SceneUnderstandingObjectType.NONE;

        ViveSR_Experience_SemanticDrawer SemanticDrawer;

        public Text ScanText, StopText, SaveText, LoadText, HintText, DartText, GripText, MidText;

        bool isTriggerDown;

        [ReadOnly] public ViveSR_Experience_StaticMesh StaticMeshScript;
        ViveSR_Experience_SwitchMode SwitchModeScript;

        int ActionModeNum = (int)ActionMode.MaxNum;

        public bool EnablePlacer = false;

        private void Awake()
        {
            StaticMeshScript = FindObjectOfType<ViveSR_Experience_StaticMesh>();
            SwitchModeScript = StaticMeshScript.SwitchModeScript;
            SemanticDrawer = FindObjectOfType<ViveSR_Experience_SemanticDrawer>();
        }

        private void Start()
        {
            LoadText.color = StaticMeshScript.CheckModelExist() ? Color.white : Color.gray;
            SemanticDrawer.enabled = true;
            ViveSR_Experience.instance.CheckHandStatus(() =>
            {
                ViveSR_RigidReconstructionRenderer.LiveMeshDisplayMode = ReconstructionDisplayMode.ADAPTIVE_MESH;
                ViveSR_Experience_ControllerDelegate.touchpadDelegate += HandleTouchpad_MeshOperation;
                ViveSR_Experience_ControllerDelegate.triggerDelegate += HandleTrigger_DisplayRay;
            });
        }

        //public void HandleGrip_SwitchMode(ButtonStage buttonStage, Vector2 axis)
        //{
        //    if (!isTriggerDown)
        //    {
        //        switch (buttonStage)
        //        {
        //            case ButtonStage.PressDown:
        //                if (!ViveSR_RigidReconstruction.IsExportingMesh && !ViveSR_RigidReconstruction.IsScanning)
        //                {
        //                    MoveToNextActionMode();
        //                }
        //                break;
        //        }
        //    }
        //}

        //void MoveToNextActionMode()
        //{
        //    int mode_int = (int)actionMode;
        //    ActionMode mode = (ActionMode)((++mode_int) % ActionModeNum);
        //    SwitchActionMode(mode);
        //}

        //void SwitchActionMode(ActionMode mode)
        //{
        //    actionMode = mode;

        //    if (mode == ActionMode.MeshControl) // 0
        //    {
        //        SemanticDrawer.enabled = false;

        //        //ViveSR_Experience_ControllerDelegate.triggerDelegate -= HandleTrigger_SelectSemanticObject;
        //        //ViveSR_Experience_ControllerDelegate.touchpadDelegate -= HandleTouchpad_SemanticObjOperation;
        //        ViveSR_Experience_ControllerDelegate.touchpadDelegate += HandleTouchpad_MeshOperation;
        //        SwitchModeScript.SwithMode(DualCameraDisplayMode.MIX);
        //        StaticMeshScript.SwitchShowCollider(ShowMode.None);
        //        SetStaticMeshUI();
        //    }
        //    else if (mode == ActionMode.SemanticObjectControl) // 1
        //    {
        //        SemanticDrawer.enabled = true;

        //        //ViveSR_Experience_ControllerDelegate.triggerDelegate += HandleTrigger_SelectSemanticObject;
        //        ViveSR_Experience_ControllerDelegate.touchpadDelegate -= HandleTouchpad_MeshOperation;
        //        //ViveSR_Experience_ControllerDelegate.touchpadDelegate += HandleTouchpad_SemanticObjOperation;
        //        StaticMeshScript.SwitchShowCollider(ShowMode.None);
        //        SetSemanticMeshUI();
        //    }
        //}

        /*------------------------------mesh-------------------------------------*/

        void UpdateModelPercentage(int percentage)
        {
            HintText.text = "Saving Scene...\n" + percentage + "%";
        }

        void UpdateSegmentationPercentage(int percentage)
        {
            HintText.text = "Saving Objects...\n" + percentage + "%";
        }

        private void HandleTouchpad_MeshOperation(ButtonStage buttonStage, Vector2 axis)
        {
            if (!isTriggerDown)
            {
                switch (buttonStage)
                {
                    case ButtonStage.PressDown:

                        TouchpadDirection touchpadDirection = ViveSR_Experience_ControllerDelegate.GetTouchpadDirection(axis, false);

                        if (touchpadDirection == TouchpadDirection.Up)//[Scan]
                        {
                            if (!ViveSR_RigidReconstruction.IsScanning && !StaticMeshScript.ModelIsLoading && !StaticMeshScript.SemanticMeshIsLoading)
                            {
                                //SemanticDrawer.DestroyAllObjects();
                                if (StaticMeshScript.CheckModelLoaded()) StaticMeshScript.LoadMesh(false);
                                StaticMeshScript.ActivateSemanticMesh(false);

                                DartText.text = "";
                                HintText.text = "";
                                GripText.text = "";
                                //ViveSR_Experience_ControllerDelegate.gripDelegate -= HandleGrip_SwitchMode;

                                StaticMeshScript.SetScanning(true);
                                StaticMeshScript.SetSegmentation(true);

                                LoadText.color = Color.gray;
                                ScanText.color = Color.gray;
                                SaveText.color = Color.white;
                                StopText.color = Color.white;
                            }
                        }
                        else if (touchpadDirection == TouchpadDirection.Left)//[Stop]
                        {
                            if (ViveSR_RigidReconstruction.IsScanning)
                            {
                                StaticMeshScript.SetScanning(false);
                                StaticMeshScript.SetSegmentation(false);
                                if (StaticMeshScript.CheckModelLoaded()) StaticMeshScript.LoadMesh(true);
                                if (StaticMeshScript.SemanticMeshIsLoaded) StaticMeshScript.ActivateSemanticMesh(true);

                                SetStaticMeshUI();
                                //ViveSR_Experience_ControllerDelegate.gripDelegate += HandleGrip_SwitchMode;
                            }
                        }
                        else if (touchpadDirection == TouchpadDirection.Right)// [Save]
                        {
                            if (ViveSR_RigidReconstruction.IsScanning)
                            {
                                StaticMeshScript.UnloadSemanticMesh();

                                LoadText.color = Color.grey;
                                ScanText.color = Color.grey;
                                StopText.color = Color.grey;
                                SaveText.color = Color.grey;
                                ViveSR_Experience_ControllerDelegate.touchpadDelegate -= HandleTouchpad_MeshOperation;

                                ViveSR_SceneUnderstanding.SetAllCustomSceneUnderstandingConfig(10, true);

                                StaticMeshScript.SetSegmentation(false);
                                
                                StaticMeshScript.ExportSemanticMesh( UpdateSegmentationPercentage,
                                    ()=>
                                    {
                                        StaticMeshScript.ExportModel( UpdateModelPercentage, 
                                            () =>
                                            {
                                                DartText.text = "";
                                                HintText.text = "Mesh Saved!";
                                                ScanText.color = Color.white;
                                                LoadText.color = Color.white;
                                                ViveSR_Experience_ControllerDelegate.touchpadDelegate += HandleTouchpad_MeshOperation;
                                            }
                                        );
                                    }
                                );
                            }
                        }
                        else if (touchpadDirection == TouchpadDirection.Down)//[Load]
                        {
                            if (!ViveSR_RigidReconstruction.IsScanning && StaticMeshScript.CheckModelExist() && !StaticMeshScript.CheckModelLoaded())
                            {
                                //ViveSR_Experience_ControllerDelegate.gripDelegate -= HandleGrip_SwitchMode;
                                StaticMeshScript.LoadMesh(
                                    true,
                                    () =>
                                    { // step 1
                                        ScanText.color = Color.grey;
                                        LoadText.color = Color.grey;
                                        HintText.text = "Loading Scene...";
                                        DartText.text = "";

                                    },
                                    () =>
                                    { // step 2
                                        if (StaticMeshScript.CheckSemanticMeshDirExist())
                                        {
                                            LoadSemanticMesh();
                                            StaticMeshScript.collisionMesh.SetActive(false);
                                        }
                                        else
                                        {
                                            ScanText.color = Color.white;
                                            HintText.text = "No Object is Found.\nPlease Rescan!";
                                        }
                                    }
                                );
                            }
                        }
                        break;
                }
            }
        }

        private void LoadSemanticMesh()
        {
            StaticMeshScript.LoadSemanticMesh(
                () =>
                {
                    HintText.text = "Loading Objects...";
                },
                () =>
                {
                    StaticMeshScript.collisionMesh.SetActive(false);

                    HintText.text = "Mesh Loaded!";
                    //vivesr_experience_controllerdelegate.gripdelegate += handlegrip_switchmode;
                    //switchactionmode(actionmode.semanticobjectcontrol);
                }
            );
        }

        //private void HandleTouchpad_SemanticObjOperation(ButtonStage buttonStage, Vector2 axis)
        //{
        //    if (!isTriggerDown)
        //    {
        //        switch (buttonStage)
        //        {
        //            case ButtonStage.PressDown:

        //                TouchpadDirection touchpadDirection = ViveSR_Experience_ControllerDelegate.GetTouchpadDirection(axis, false);

        //                if (touchpadDirection == TouchpadDirection.Up)
        //                {
        //                    SemanticDrawer.ShowAllObjects();
        //                    HintText.text = "Show All";
        //                }
        //                else if (touchpadDirection == TouchpadDirection.Left)
        //                {
        //                    SemanticDrawer.ClearAllObjects();
        //                    ShowPreviousSemanticType();
        //                }
        //                else if (touchpadDirection == TouchpadDirection.Right)
        //                {
        //                    SemanticDrawer.ClearAllObjects();
        //                    ShowNextSemanticType();
        //                }
        //                else if (touchpadDirection == TouchpadDirection.Down)
        //                {
        //                    SemanticDrawer.ClearAllObjects();
        //                    HintText.text = "Show Scene Objects";
        //                }
        //                break;
        //        }
        //    }
        //}

        //private void ShowNextSemanticType()
        //{
        //    int type, input_type_int;
        //    type = input_type_int = (int)currentSemanticType;
        //    do {
        //        type = ++type % (int)SceneUnderstandingObjectType.NumOfTypes;
        //        if (type == input_type_int)
        //        {
        //            SemanticDrawer.ShowBoundingBoxAndIconByType(type, true, true);
        //            break;
        //        }
        //    } while (type == (int)SceneUnderstandingObjectType.NONE || !SemanticDrawer.ShowBoundingBoxAndIconByType(type, true, true));

        //    StaticMeshScript.ShowSemanticColliderByType((SceneUnderstandingObjectType)type);
        //    currentSemanticType = (SceneUnderstandingObjectType)type;
        //    HintText.text = "Show "+ ViveSR_SceneUnderstanding.SemanticTypeToString(currentSemanticType);
        //}

        //private void ShowPreviousSemanticType()
        //{
        //    int type, input_type_int;
        //    type = input_type_int = (int)currentSemanticType;
        //    do
        //    {
        //        if (--type < 0) type += (int)SceneUnderstandingObjectType.NumOfTypes;
        //        if (type == input_type_int)
        //        {
        //            SemanticDrawer.ShowBoundingBoxAndIconByType(type, true, true);
        //            break;
        //        }
        //    } while (type == (int)SceneUnderstandingObjectType.NONE || !SemanticDrawer.ShowBoundingBoxAndIconByType(type, true, true));

        //    StaticMeshScript.ShowSemanticColliderByType((SceneUnderstandingObjectType)type);
        //    currentSemanticType = (SceneUnderstandingObjectType)type;
        //    HintText.text = "Show " + ViveSR_SceneUnderstanding.SemanticTypeToString(currentSemanticType);
        //}

        //void HandleTrigger_SelectSemanticObject(ButtonStage buttonStage, Vector2 axis)
        //{
        //    switch (buttonStage)
        //    {
        //        case ButtonStage.PressDown:
        //            SemanticDrawer.enablePlacer = EnablePlacer;
        //            SemanticDrawer.TriggerPressDown();
        //            isTriggerDown = true;
        //            break;

        //        case ButtonStage.PressUp:
        //            isTriggerDown = false;
        //            SceneUnderstandingObjectType type = SemanticDrawer.TriggerPressUp();
        //            //HintText.text = "Show " + ViveSR_SceneUnderstanding.SemanticTypeToString(type);
        //            break;
        //    }
        //}

        void HandleTrigger_DisplayRay(ButtonStage buttonStage, Vector2 axis)
        {
            switch (buttonStage)
            {
                case ButtonStage.PressDown:
                    SemanticDrawer.TriggerPressDown();
                    StaticMeshScript.SetScanning(true);
                    isTriggerDown = true;
                    break;
                case ButtonStage.PressUp:
                    SemanticDrawer.TriggerPressUp();
                    StaticMeshScript.SetScanning(false);
                    isTriggerDown = false;
                    break;
            }
        }

        void SetStaticMeshUI()
        {
            StopText.color = Color.grey;
            SaveText.color = Color.grey;
            LoadText.color = StaticMeshScript.SemanticMeshIsLoaded ? Color.grey : Color.white;
            ScanText.color = StaticMeshScript.ModelIsLoading ? Color.grey : Color.white;

            HintText.text = "Static Mesh";
            StopText.text = "[Stop]";
            SaveText.text = "[Save]";
            LoadText.text = "[Load]";
            ScanText.text = "[Scan]";
            MidText.text = "";
            DartText.text = "";
            GripText.text = "Show Scene Objects";
        }

        void SetSemanticMeshUI()
        {
            StopText.color = Color.white;
            SaveText.color = Color.white;
            LoadText.color = Color.white;
            ScanText.color = Color.white;

            HintText.text = "Show Scene Objects";
            StopText.text = "<";
            SaveText.text = ">";
            LoadText.text = "[Clear]";
            ScanText.text = "[Show All]";
            MidText.text = "";
            DartText.text = "Select And Show Object";
            GripText.text = "Switch Control";
        }
    }
}