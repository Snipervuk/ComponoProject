using UnityEngine;
using UnityEngine.AddressableAssets;


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
    public Vector3 StartingEulerAngles;

    public int photoIndexCounter = 0;
    public int itemListNum = 0;
    public string objectNameGet;

    //Called Before Start
    private void Awake()
    {
        //Set Rotate Obj by rotationAmount
        rotateObj = rotationAmount;

        //Get Starting Angle Vars
        StartingEulerAngles = transform.position;
    }

    //Called First Frame
    private void Start()
    {
        if (m_itemList == null || m_itemList.AssetReferenceCount == 0)
        {
            Debug.LogError("Spawn list not setup correctly");
        }
        LoadItemAtIndex(m_itemList, itemListNum);
    }

    //Called Every Frame
    private void Update()
    {
        //Gets object name when it changes
        objectNameGet = m_instanceObject.name;

        RotateObjectandScreenCap();
    }

    /// <summary>
    /// Rotation and Screenshot function for the objects
    /// To use press the SpaceBar function on the keyboard!
    /// </summary>
    void RotateObjectandScreenCap()
    {
        //SpaceBar Function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Rotate the Object
            this.transform.Rotate(0, rotateObj, 0);

            //Incremenent Counter
            photoIndexCounter++;

            //10 or greater change formating of frame string
            if (photoIndexCounter >= 10)
            {
                //4 digits double number output
                //ScreenCapture.CaptureScreenshot(@"Assets\Outputs\Car-1203-Black\frame00" + photoIndexCounter.ToString() + ".png");
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + "\frame00" + photoIndexCounter.ToString() + ".png");

            }
            else //Photo frame num is 9 or below
            {   //4 Digits single number output

                //ScreenCapture.CaptureScreenshot(@"Assets\Outputs\Car-1203-Black\frame000" + photoIndexCounter.ToString() + ".png");
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + "\frame000" + photoIndexCounter.ToString() + ".png");
            }  
        }

        //At Last Photo
        if (photoIndexCounter == 17)
        {
            //Increament itemListNum for the SpawnList
            itemListNum++;

            //Reset for starting photo
            startingPhotoTaken = false;

            //Destroy AND LOADS Load New Object / Increment 
            LoadItemAtIndex(m_itemList, itemListNum);
            
            //Reset
            photoIndexCounter = 0;

            //For Testing
            Debug.Log("Photos finished for this object, moving onto " + objectNameGet);
        }
    }

    private void LoadItemAtIndex(SpawnItemList itemList, int index)
    {
        if (m_instanceObject != null)
        {
            /*
            Destroy(m_instanceObject);
            */
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
                //First Screenshot goes here and increment counter
                //ScreenCapture.CaptureScreenshot(@"Assets\Outputs\Car-1203-Black\frame000" + photoIndexCounter.ToString() + ".png");

                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\" + objectNameGet.ToString() + "a" + "frame000" + photoIndexCounter.ToString() + ".png");

                photoIndexCounter++;
                startingPhotoTaken = true;
            }
        }
    }
}
