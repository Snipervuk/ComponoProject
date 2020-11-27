using UnityEngine;
using UnityEngine.AddressableAssets;

public class ExportRoutine : MonoBehaviour
{
    //Their Inbuilt Vars
    public SpawnItemList m_itemList = null;
    public AssetReferenceGameObject m_assetLoadedAsset;
    public GameObject m_instanceObject = null;

    //My Vars
    float rotationAmount = 22.5f;
    float rotateObj;
    bool hasRotated = false;
    bool takenPhoto = false;
    bool allScreenshotsTaken = false;
    public Vector3 currentEulerAngles;
    int photoIndexCounter = 0;

    private void Awake()
    {
        //Set Rotate Obj by rotationAmount
        rotateObj = rotationAmount;
    }

    private void Update()
    {
        currentEulerAngles = this.transform.rotation.eulerAngles;

        RotateObjectandScreenCap();

    }

    void RotateObjectandScreenCap()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //10 or greater change formating of frame string
            if (photoIndexCounter >= 10)
            {
                //4 digits double number output
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\Car-1203-Black\frame00" + photoIndexCounter.ToString() + ".png");
            }
            else //Photo frame num is 9 or below
            {   //4 Digits single number output
                ScreenCapture.CaptureScreenshot(@"Assets\Outputs\Car-1203-Black\frame000" + photoIndexCounter.ToString() + ".png");
            }

            //Increament Index Counter
            photoIndexCounter++;

            this.transform.Rotate(0, rotateObj, 0);
        }

        //At Last Photo
        if (photoIndexCounter == 15)
        {
            //Destroy Current Object


            //Increase index and Load in NewObject


            //Reset
            photoIndexCounter = 0;
        }
    }

    private void Start()
    {
        if (m_itemList == null || m_itemList.AssetReferenceCount == 0)
        {
            Debug.LogError("Spawn list not setup correctly");
        }
        LoadItemAtIndex(m_itemList, 0);
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
        }
    }
}
