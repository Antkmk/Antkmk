// -------------------------------------------------------------------------------
//
// selectPhotoFromGallery.cs
// Description:   Display the list of the device's photos from it's photo gallery
//                allowing the user to pick one, display it one the screen and
//                save it to a file.
// Created:       09 / 28 / 22 © One Bad Ant
// Modifications: 
//
// -------------------------------------------------------------------------------

// Basic library requirements
// -------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class selectPhotoFromGallery : MonoBehaviour
{
    public RawImage photoView;
    Texture2D selectedPhotoTexture;
    int maxPhotoSize = 1632; // downsize image if width or height is greater than this
    //string photoFileName = "unityTestPhoto.jpg"; // this can be anything or set dynamically by your app.
    string photoPathFileName = string.Format("{0}/{1}", SceneParametersHandler.PHOTOS_PATH, "unityTestPhoto.jpg");

    // -------------------------------------------------------------------------------
    // Start is called before the first frame update
    // Check for an already selected image and display it if we have one, otherwise set
    // the rawImage's opacity to 0 to show the "no selected photo" message
    // -------------------------------------------------------------------------------
    void Start()
    {
        if (System.IO.File.Exists(photoPathFileName))
        {
            Debug.Log("File exists");
            selectedPhotoTexture = NativeGallery.LoadImageAtPath(photoPathFileName, maxPhotoSize);
            photoView.texture = selectedPhotoTexture;
            photoView.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
        else
        {
            Debug.Log("File does not exist");
            photoView.CrossFadeAlpha(0.0f, 0.1f, false); // hide rawimage so message shows
        }
    }

    // -------------------------------------------------------------------------------
    // On "Pick A Photo" button click
    // -------------------------------------------------------------------------------
    public void pickAPhoto()
    {
        Debug.Log("Pick A Photo button pressed");

        // Do not allow a second click on the button if a selection is in progress
        if ( !(NativeGallery.IsMediaPickerBusy()) )
        {
            // Pick a PNG image from Gallery/Photos if permission is granted
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    selectedPhotoTexture = NativeGallery.LoadImageAtPath(path, maxPhotoSize);
                    if (selectedPhotoTexture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }

                    int pW = selectedPhotoTexture.width; // photo width
                    int pH = selectedPhotoTexture.height; // photo height
                    Debug.Log("Photo Width: " + pW + " Height: " + pH);

                    // -------------------------------------------------------------------------------
                    // load the selected photo onto the scene's rawimage object and scale it
                    // -------------------------------------------------------------------------------
                    photoView.texture = selectedPhotoTexture;
                    photoView.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                    photoView.CrossFadeAlpha(1.0f, 0.1f, false); // make sure opacity is 1!

                    // -------------------------------------------------------------------------------
                    // Create a temporary readable texture of the same size of the photo that can be saved to a jpg file
                    // -------------------------------------------------------------------------------
                    RenderTexture tmpTexture = RenderTexture.GetTemporary(pW,pH,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);

                    Graphics.Blit(selectedPhotoTexture, tmpTexture); // Blit the pixels on texture to the temp RenderTexture
                    RenderTexture prevTexture = RenderTexture.active; // Backup the currently set RenderTexture
                    RenderTexture.active = tmpTexture; // Set the current RenderTexture to the temporary one we created
                    Texture2D readableTexture2D = new Texture2D(pW, pH); // Create a new readable Texture2D to copy the pixels to it

                    // Copy the pixels from the RenderTexture to the new Texture
                    readableTexture2D.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
                    readableTexture2D.Apply();

                    RenderTexture.active = prevTexture; // Reset the active RenderTexture
                    RenderTexture.ReleaseTemporary(tmpTexture); // Release the temporary RenderTexture

                    // write the new photo file
                    byte[] photoBytes = readableTexture2D.EncodeToJPG();
                    File.WriteAllBytes(photoPathFileName, photoBytes);
                }
            });

            Debug.Log("Permission result: " + permission);
        }
    } // pickAPhoto

}
