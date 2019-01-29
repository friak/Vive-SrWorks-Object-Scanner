# Vive-SrWorks-Object-Scanner

How to scan static objects in SRWorks using the Unity game engine

  -Gabriel Chapel and Farjana Ria Khan

## Downloading SRWorks and Steam VR:

## 1 ) SRWorks SDK v0.8.0.2

You need to download the latest version of SRWorks SDK (as of Jan. 29, 2019) on the Vive Developers website at:
https://developer.vive.com/resources/knowledgebase/vive-srworks-sdk/

You'll be downloading the zip files shown below for Unity
![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/unity.png)

## 2) Steam VR 1.2.3

The latest version of SRWorks SDK is not compatible with the most recent iteration of Steam VR. Instead, Steam VR v1.2.3 needs to be downloaded from:
https://github.com/ValveSoftware/steamvr_unity_plugin/releases/tag/1.2.3

## Opening SRWorks in Unity
When opening Unity, you'll need to import three asset packages:
-Steam VR 1.2.3
-Vive-SRWorks-0.8.0.2-Unity-Plugin
-Vive-SRWorks-0.8.0.2-Unity-Experience

You can do this in Unity by going to Assets > Import Package > Custom Package

## ViveSR Experience Example 9

All of the modding for this function will be happening in The Semantic Segmentation Example provided by SRWorks
![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/2.png)

This example shows the static mesh scanner and saver function. We're going to be modifying this example in several ways.

## 1) Creating a single submesh in RigidReconstructionRenderer
Forewarning, these script names will be unnesscarily long.

The first modifcation to the script will be in RigidReconstructionRenderer.cs , a script that can found in the game object RigidReconstructor under [Vive SR] in the lefthand GUI
![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/4.png)

We'll be then opening up the script and changing just two lines in the code; setting LastEnableSector and BackupSectorValue from true to false
![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/1.png)

We're doing this because we want to create a single child to call back in a new game object that we'll be making later in this walkthrough. When these booliens are set to false, the static mesh we create becomes a singular submesh ( submesh(0) ).

## 2) BoundingBox game object

Example 9 includes a beam pointer or RayCast when the trigger is pulled down on the right-hand controller. We're going to be heavily commenting out lines from the two scripts Sample9_SemanticSegmentation.cs and ViveSR_Experience_SemanticDrawer.cs in this example so that the RayCast can be used as a pointer instead of its original functionality, which was to segment items within the static scan (this funtion is not important to what we're doing here). 

Lastly, we'll be attaching the game object BoundingBox to the end point of the RayCast. To do this we'll be adding our own lines of code in the 
![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/6.png)

Refer to the modified scripts in this respitory to see what has been commented out.

## 3) ScanCopy game object

We'll be creating our own custom script for this last set of instructions. Once again, right-click on the left hand GUI to create a new game object.

We're going to then add component > new script and name our script ScanCopy.cs 

To make RigidReconstructor globally available to this new game, while viewing ScanCopy, drag RigidReconstructor into the ScanCopy component as the Scan Parent.

![Figure Images](https://github.com/friak/Vive-SrWorks-Object-Scanner/blob/master/3.png)

Lastly, we'll be adding the code to ScanCopy.cs (refer to ScanCopy.cs in this repository, detaile comments will also be included within code).

## And that's it

Run the game and witht the right controller, holled down the trigger to scan objects within the bounding box for a tighter static mesh.

