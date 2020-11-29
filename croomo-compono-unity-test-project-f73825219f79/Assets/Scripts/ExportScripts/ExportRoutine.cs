using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;
using UnityEditor;


/// <summary>
/// Description: Script to use for taking photos and exporting them. By the use of userinput and goes through the hierarchy of SpawnList 
/// By: Christian Milicevic
/// Date: 27/11/202
/// </summary>
public class ExportRoutine : MonoBehaviour
{
    //Their Inbuilt Vars
    public SpawnItemList m_itemList = null;
    public AssetReferenceGameObject m_assetLoadedAsset;
    public GameObject m_instanceObject = null;

    //My Vars
    float rotationAmount = 22.5f;
    float rotateObj;

    bool startingPhotoTaken = false;
    bool userInput = true;
    public Vector3 StartingEulerAngles;

    public int photoIndexCounter = 0;
    private int maxNumOfPhotos = 15;
    private int maxNumOfItems = 6;
    public int itemListNum = 0;
    public string objectNameGet;

    //Camera Resolution Vars
    Camera screenshotCamera;
    int resHeight = 512;
    int resWidth = 512;

    //Called First Frame
    private void Start()
    {
        //Set Resolution on Camera for appropriate output 
        //screenshotCamera.targetTexture = new RenderTexture(resWidth, resHeight, 24);

        //Set Rotate Obj by rotationAmount
        rotateObj = rotationAmount;

        //Get Starting Angle Vars
        StartingEulerAngles = transform.position;

        FirstObjectSetup();
    }

    //Called Every Frame
    private void Update()
    {
        //UserInput and ScreenCapture
        RotateObjectandScreenCap();
        
        //Last Object in List
        ExitPlayer(maxNumOfItems);
    }

    /// <summary>
    /// Function for putting the first spawn item in the itemList
    /// </summary>
    private void FirstObjectSetup()
    {
        if (m_itemList == null || m_itemList.AssetReferenceCount == 0)
        {
            Debug.LogError("Spawn list not setup correctly");
        }
        LoadItemAtIndex(m_itemList, itemListNum);

        //Gets object name when it changes
        objectNameGet = m_instanceObject.name;
    }

    /// <summary>
    /// Exit out of player mode when all photos are done 
    /// </summary>
    /// <param name="index"></param>
    private void ExitPlayer(int index)
    {
        if (itemListNum == index && photoIndexCounter == maxNumOfPhotos)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    /// <summary>
    /// Rotation and Screenshot function for the objects
    /// To use press the SpaceBar function on the keyboard!
    /// </summary>
    void RotateObjectandScreenCap()
    {
        //SpaceBar Function
        if (Input.GetKeyDown(KeyCode.Space) && userInput == true)
        {
            //Rotate the Object
            this.transform.Rotate(0, rotateObj, 0);

            //Incremenent Counter
            photoIndexCounter++;

            //10 or greater change formating of frame string
            if (photoIndexCounter >= 10)
            {
                //4 digits double number output
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + @"\frame00" + photoIndexCounter.ToString() + ".png", 1);

            }
            else //Photo frame num is 9 or below
            {   
                //4 Digits single number output
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + @"\frame000" + photoIndexCounter.ToString() + ".png", 1);
            }  
        }

        
        if (itemListNum != maxNumOfItems)
        {
            //At Last Photo
            if (photoIndexCounter == maxNumOfPhotos)
            {
                //User can't screenshot/rotate object
                userInput = false;

                //Reset for starting photo
                startingPhotoTaken = false;

                //Delete Object w/ Delay (1 sec)
                StartCoroutine(ObjectDeleteDelay());

                //Spawn Object w/ Delay (2 secs)
                StartCoroutine(SpawnObjectDelay());

                //Reset
                photoIndexCounter = 0;

                //For Testing
                Debug.Log("Photos finished for this object, moving onto " + objectNameGet);
            }
        }
    }

    
    /// <summary>
    /// IEnumerator Timer from System Collections for Object Delete Delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator ObjectDeleteDelay()
    {
        //Destroy Previous GameObject and get first child as gameObject
        GameObject previousGameobject = this.gameObject.transform.GetChild(0).gameObject;

        //Wait for 1 second to delete
        yield return new WaitForSeconds(1);

        Destroy(previousGameobject);
    }

    /// <summary>
    /// IEnumerator Timer from System Collections for Object Spawn Delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnObjectDelay()
    {
        yield return new WaitForSeconds(2);

        //Increament itemListNum for the SpawnList
        itemListNum++;

        //Destroy AND LOADS Load New Object / Increment 
        LoadItemAtIndex(m_itemList, itemListNum);

        //User has input again
        userInput = true;
    }


    private void LoadItemAtIndex(SpawnItemList itemList, int index)
    {
        if (m_instanceObject != null)
        { 
            Destroy(m_instanceObject);
        }

        m_assetLoadedAsset = itemList.GetAssetReferenceAtIndex(index);
        var spawnPosition = new Vector3();
        var spawnRotation = Quaternion.identity;
        var parentTransform = this.transform;

        var loadRoutine = m_assetLoadedAsset.LoadAssetAsync();
        loadRoutine.Completed += LoadRoutine_Completed;

        void LoadRoutine_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
        {
            m_instanceObject = Instantiate(obj.Result, spawnPosition, spawnRotation, parentTransform);

            //Set name, so object can be stored in correct Output folder
            m_instanceObject.name = obj.Result.name;

            if (StartingEulerAngles == new Vector3(0, 0, 0) && startingPhotoTaken == false)
            {
                //Update Object Name var for screenshoot
                objectNameGet = m_instanceObject.name;

                //First Screenshot goes here and increment counter
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + @"\frame000" + photoIndexCounter.ToString() + ".png", 1);

                //First Photo has been 
                startingPhotoTaken = true;
            }
        }
    }
}
