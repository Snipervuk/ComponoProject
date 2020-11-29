using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the Snapshot Camera for rendering a custom 
/// </summary>
public class CameraScreenshot : MonoBehaviour
{
    //Vars
    [SerializeField] GameObject spawnPoint;
    Camera screenshotCam;
    [SerializeField] int resWidth = 512;
    [SerializeField] int resHeight = 512;

    private void Start()
    {
        //Get Camera Component
        screenshotCam = GetComponent<Camera>();

        //Makes one if null
        if (screenshotCam.targetTexture == null)
        {
            screenshotCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else //If we do have one
        {
            resWidth = screenshotCam.targetTexture.width;
            resHeight = screenshotCam.targetTexture.height;
        }
    }

    /// <summary>
    /// Take Screenshot Function called in ExportRoutine
    /// </summary>
    public void TakeScreenshot()
    {
        Texture2D screenshotText = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        screenshotCam.Render();
        RenderTexture.active = screenshotCam.targetTexture;
        screenshotText.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        byte[] bytes = screenshotText.EncodeToPNG();
        
        if (spawnPoint.GetComponent<ExportRoutine>().photoIndexCounter >= 10)
        {
            System.IO.File.WriteAllBytes(@"Assets\Outputs\" + spawnPoint.GetComponent<ExportRoutine>().objectNameGet.ToString() + @"\frame00" + spawnPoint.GetComponent<ExportRoutine>().photoIndexCounter.ToString() + ".png", bytes);
        }
        else //Is 9 or less
        {
            System.IO.File.WriteAllBytes(@"Assets\Outputs\" + spawnPoint.GetComponent<ExportRoutine>().objectNameGet.ToString() + @"\frame000" + spawnPoint.GetComponent<ExportRoutine>().photoIndexCounter.ToString() + ".png", bytes);
        }

        //For Testing
        Debug.Log("Screenshot Taken!");
    }
}
