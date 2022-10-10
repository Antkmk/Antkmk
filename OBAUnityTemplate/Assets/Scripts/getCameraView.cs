// -------------------------------------------------------------------------------
//
// getCameraView.cs
// Description:   Display the device's camera view over the scene's attached
//                raw image object.
// Created:       07 / 25 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getCameraView : MonoBehaviour
{

    static WebCamTexture deviceCam;
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);
    public RawImage cameraView;

    public Quaternion baseRotation;


    // Start is called before the first frame update
    void Start()
    {

        // Get the device's camera:
        // For the Unity Editor use the front facing camera, for an Android or iOS device use
        // the camera facing away from the person (back camera)
        // -------------------------------------------------------------------------------
        if (deviceCam == null)
        {
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
#if UNITY_EDITOR
                if (WebCamTexture.devices[i].isFrontFacing)
                {
                    deviceCam = new WebCamTexture(WebCamTexture.devices[i].name);
                    break;
                }
#else
                if (!WebCamTexture.devices[i].isFrontFacing)
                {
                    deviceCam = new WebCamTexture(WebCamTexture.devices[i].name);
                    break;
                }
#endif
            }
        }

        // Display the camera view on the scene, stream and orient the view
        // -------------------------------------------------------------------------------
        deviceCam.filterMode = FilterMode.Trilinear;
        cameraView.texture = deviceCam;
        cameraView.material.mainTexture = deviceCam;

        // Start the camera
        if (!deviceCam.isPlaying)
        {
            deviceCam.Play();
        }

        // Make sure the camera's orientation is correct (top is up!)
        rotationVector.z = -deviceCam.videoRotationAngle;
        cameraView.rectTransform.localEulerAngles = rotationVector;

        // Compensate for camera view being flipped horizontally on iOS
#if UNITY_IOS          
        cameraView.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
#endif

        Debug.Log("CAMERA ROTATION " + -deviceCam.videoRotationAngle + " DEGREES");
        Debug.Log("VERTICALLY MIRRORED: " + deviceCam.videoVerticallyMirrored);
    } // Start
}