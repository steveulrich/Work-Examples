using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetLoader : MonoBehaviour {

    public enum DownloadStatus
    {
        NONE,
        DOWNLOADING,
        SUCCESS,
        FAIL,
        FAIL_NO_INTERNET,
        FAIL_NO_DISK_SPACE
    }

    private const string _bundleServerDefault = "http://s3.amazonaws.com/ericsson.norcal";
    private static string BundleServer
    {
        get
        {
            string tmp = string.Empty;
            if(!Backend.DynamicProgramProperties.TryGetValue<string>("AssetBundleServerURL", out tmp))
            {
                tmp = _bundleServerDefault;
            }

            return tmp;
        }
    }

    private static string BundlePlatform
    {
        get
        {
#if UNITY_EDITOR_WIN
            return "Windows";
#elif UNITY_EDITOR_OSX
            return "OSX";
#else
            return Application.platform == RuntimePlatform.Android ? "Android" : "iOS";
#endif
        }
    }

    // this describes the bundle version that will be loaded from online
    private static int BUNDLE_VERSION = -1;

    /// <summary>
    /// Return either "Latest" or the number for the version if there is one
    /// </summary>
    public static string BundleVersion
    {
        get
        {
            if (BUNDLE_VERSION == -1)
            {
                string tmpVersion = "Latest";
                // try to read the BundleVersion from DynamicProperties
                if (Backend.DynamicProgramProperties.TryGetValue<string>("BundleVersion", out tmpVersion))
                {
                    // if the bundle version is numeric
                    if (Backend.Utility.IsNumeric(tmpVersion))
                    {
                        BUNDLE_VERSION = System.Int32.Parse(tmpVersion);
                    }
                }
            }

            return (BUNDLE_VERSION == -1) ? "Latest" : BUNDLE_VERSION.ToString();
        }
    }

    // the list of loaded asset bundles. Key = bundle name. Value = AssetBundle.
    private static Dictionary<string, AssetBundle> loadedAssetBundles;

    /// <summary>
    /// Get the url for the bundle based on name.
    /// 
    /// REMEMBER: All bundle names MUST be lowercase according to the Unity specification.
    /// </summary>
    /// <param name="bundleName">the name of the bundle for the url</param>
    private static string GetFullBundleUrl(string bundleName)
    {
        return AssetLoader.BundleServer + "Bundles/" + BundleVersion + "/" + BundlePlatform + "/" + bundleName.ToLower();
    }

    // download bundle coroutine
    public static IEnumerator DownloadBundleTask(string bundleName, bool loadAll, System.Action<DownloadStatus> downloadCallback = null, bool keepInMemory = true)
    {

        DownloadStatus status = DownloadStatus.DOWNLOADING;

        // get the url for this bundle
        string bundleUrl = GetFullBundleUrl(bundleName);


        AssetBundle assetBundle = null;

        string pathDir = "";

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        pathDir = "Windows";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            pathDir = "OSX";
#elif UNITY_ANDROID
            pathDir = "Android";
#elif UNITY_IOS || UNITY_IPHONE
            pathDir = "iOS";
#endif

#if UNITY_EDITOR
        // tell the world that we are downloading a bundle
        Logging.Log("Attempting to retrieve bundle " + bundleName + " from " + Application.streamingAssetsPath + "/" + pathDir + "/" + bundleName);
        AssetBundleCreateRequest streamwww = null;
        //Verifies path before trying to load bundle file
        if (Directory.Exists(Application.streamingAssetsPath + "/" + pathDir) && File.Exists(Application.streamingAssetsPath + "/" + pathDir + "/" + bundleName))
        {
            streamwww = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + pathDir + "/" + bundleName, (uint)0);

            yield return streamwww;
            if (streamwww != null)
            {
                assetBundle = streamwww.assetBundle;
            }
        }
        else
        {
            Logging.LogWarning(bundleName + "Bundle not found in streaming assets: ");
        }
        //clean up
        streamwww = null;

#endif

        WWW www = null;

        //Check if null from not set from editor or if was set but error occured
        if (assetBundle == null)
        {
            // clean up the WWW + UnityWebRequest instance, since first one failed
            // tell the world that we are downloading a bundle
            Logging.Log("LoadFromCacheOrDownload " + bundleName + " from " + bundleUrl);

            int version = 0;
            if (!int.TryParse(BundleVersion, out version))
            {
                Logging.Log("Cleaning Cached Bundles!");
                Caching.CleanCache();
                version = 0;
            }

            if (Caching.IsVersionCached(bundleUrl, version))
            {
                Logging.Log(bundleUrl + " " + version + " is already cached");
            }
            else
            {
                Caching.CleanCache(); // Clean the cache to remove old versions if htere are any.
                Logging.Log(bundleUrl + " " + version + "is NOT cached, downloading!");
            }

            www = WWW.LoadFromCacheOrDownload(bundleUrl, version);

            // wait until the download finishes
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {

                // store a reference to the new assetbundle
                assetBundle = www.assetBundle;

                Logging.Log("Bundle " + bundleName + " downloaded...");
            }
            else // if there IS an error
            {
                Logging.LogError("Failed to download bundle " + bundleName + " due to error: " + www.error);
                //PopupMessage.LogError("Failed to download the level.\nPlease check your internet connection.");
                status = DownloadStatus.FAIL;
            }
        }
        else
        {
            Logging.Log("Bundle " + bundleName + " Successfully Loaded from Streaming Assets...");
        }

        // clean up the WWW instance
        if (www != null)
        {
            // tell the garbage collector that it's okay to clean up the chunk of memory formerly reserved by this UnityWebRequest instance
            www.Dispose();
            www = null;
        }


        // store the bundle
        if (assetBundle != null)
        {
            // if we should load all assets
            if (loadAll && !assetBundle.isStreamedSceneAssetBundle)
            {
                Logging.Log("Loading all assets from bundle " + bundleName + "...");

                // Load all assets. This may be useful for bundles that are required for the level.
                Dictionary<string, GameObject> BundledGOs = new Dictionary<string, GameObject>();
                Object[] loadedAssets = assetBundle.LoadAllAssets();
                foreach(var obj in loadedAssets)
                {
                    BundledGOs.Add(obj.name, (GameObject)obj);
                }

            }
            else if (loadAll && assetBundle.isStreamedSceneAssetBundle)
            {
                Logging.LogWarning("Unable to load all assets in bundle " + bundleName + " because it is a streamed scene bundle!");
            }

            // tell the world that we are done downloading the bundle
            Logging.Log("Done downloading bundle " + bundleName);

            // if we should keep the bundle in memory for immediate use
            if (keepInMemory == true)
            {
                // store a reference to the bundle
                StoreBundle(bundleName, assetBundle);

                Logging.Log("STORED BUNDLE: " + bundleName);
            }
            // store the last bundle download time
            PlayerPrefs.SetString("LastBundleDownloadTime", Backend.Utility.ConvertDateTimeToTimestamp(System.DateTime.Now));
            PlayerPrefs.Save();

            // note our success
            status = DownloadStatus.SUCCESS;
        }
        else
        {
            Logging.LogError("Asset bundle " + bundleName + " is null. Unable to store bundle!");
            status = DownloadStatus.FAIL;
        }

        if(downloadCallback != null)
        {
            downloadCallback(status);
            downloadCallback = null;
        }
    }


    /// <summary>
    /// Store a newly loaded asset bundle
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="newBundle"></param>
    private static void StoreBundle(string bundleName, AssetBundle newBundle)
    {
        string lwrBundleName = bundleName.ToLower();

        // make sure that loadedAssetBundles has been isntantiated
        if (loadedAssetBundles == null)
            loadedAssetBundles = new Dictionary<string, AssetBundle>();

        // add the new key value pair
        loadedAssetBundles[lwrBundleName] = newBundle;
    }


    #region Loading Bundled Assets Dynamically

    /// <summary>
    /// Load a bundled asset.
    /// 
    /// If the build uses bundles, then this will load from a bundle that
    /// was downloaded previously. If the build does NOT use bundles, then
    /// this will load from Resources.
    /// </summary>
    /// <param name="bundleName">the name of the bundle to which this asset belongs</param>
    /// <param name="assetPath">the path to the asset in the resources folder</param>
    /// <returns></returns>
    public static Object LoadBundledAsset(string bundleName, string assetPath, bool checkResourcesFirst = false)
    {
        // get the ACTUAL bundle and asset names

        // make sure the bundleName is all lowercase - it's probably coming from activeParticle.guid, so it will probably be all caps
        string lwrBundleName = bundleName.ToLower();

        // pick out the name of the asset
        int startIndex = assetPath.LastIndexOf("/") + 1;
        int length = assetPath.Length - startIndex;
        // remove the file extension if there is one
        if (assetPath.Length - assetPath.LastIndexOf(".") >= 3 && assetPath.Length - assetPath.LastIndexOf(".") <= 5)
            length = assetPath.LastIndexOf(".") - startIndex;
        // call substring to get the actual asset name
        string assetName = assetPath.Substring(startIndex, length);

        Logging.Log("Attempting to load " + assetName + " from bundle " + lwrBundleName);

        // if this argument is true, then we should first try to load from Resources
        if (checkResourcesFirst)
        {
            Logging.Log("Attempting to load " + assetPath + " from resources...");

            var tmpObject = Resources.Load(assetPath);
            if (tmpObject != null) return tmpObject;
        }

        // if we should use bundles
        return LoadAssetFromBundle(lwrBundleName, assetName);
    }

    public static T LoadBundledAsset<T>(string bundleName, string assetPath, bool checkResourcesFirst = false)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            Logging.LogError("assetPath is null or empty. Something bad happened?");
            return default(T);
        }

        // get the ACTUAL bundle and asset names
        // make sure the bundleName is all lowercase - it's probably coming from activeParticle.guid, so it will probably be all caps
        string lwrBundleName = bundleName.ToLower();

        // pick out the name of the asset
        int startIndex = assetPath.LastIndexOf("/") + 1;
        int length = assetPath.Length - startIndex;

        //Checks if there is a lastIndex value, if there isn't and == -1 set last index to its length
        int lastIndexOf = assetPath.LastIndexOf(".") != -1 ? assetPath.LastIndexOf(".") : assetPath.Length;
        // remove the file extension if there is one
        if (assetPath.Length - lastIndexOf >= 3 && assetPath.Length - lastIndexOf <= 5)
            length = lastIndexOf - startIndex;
        // call substring to get the actual asset name
        string assetName = assetPath.Substring(startIndex, length);

        Logging.Log("Attempting to load " + assetName + " from bundle " + lwrBundleName);

        // if this argument is true, then we should first try to load from Resources
        if (checkResourcesFirst)
        {
            Logging.Log("Attempting to load " + assetPath + " from resources...");
            if (typeof(T) != typeof(Sprite))
            {
                var tmpObject = (T)(object)Resources.Load(assetPath);
                if (tmpObject != null) return tmpObject;
            }
            else // if we are converting a Sprite
            {
                // use the parameterized load
                var tmpObject = Resources.Load<Sprite>(assetPath);
                if (tmpObject != null) return (T)(object)tmpObject;
            }
        }

        // if we should use bundles
        return LoadAssetFromBundle<T>(lwrBundleName, assetName);
    }

    /// <summary>
    /// Get an asset from an asset bundle.
    /// </summary>
    /// <returns>null or the asset</returns>
    private static Object LoadAssetFromBundle(string bundleName, string assetName)
    {
        // if the bundle has been loaded
        if (BundleLoaded(bundleName))
        {
            // if the bundle contains the specified asset
            if (loadedAssetBundles[bundleName].Contains(assetName))
            {
                // return it
                return loadedAssetBundles[bundleName].LoadAsset(assetName);
                //return loadedAssetBundles[bundleName].LoadAssetWithSubAssets(assetName);
            }
            else // if the bundle does NOT contain the specified asset
            {
                Logging.LogError("Bundle " + bundleName + " does not contain " + assetName);
            }
        }
        else // if the bundle is NOT loaded
        {
            // log an error
            Logging.LogError("Bundle " + bundleName + " has not been loaded! Unable to get " + assetName + " from bundle.");
        }
        return null;
    }

    private static T LoadAssetFromBundle<T>(string bundleName, string assetName)
    {
        if (string.IsNullOrEmpty(bundleName))
        {
            Logging.LogError("bundleName came into BundleLoaded check NULL");
            return default(T);
        }

        // if the bundle has been loaded
        if (BundleLoaded(bundleName))
        {
            // if the bundle contains the specified asset
            if (loadedAssetBundles[bundleName].Contains(assetName))
            {
                Object readData = null;
                if (typeof(T) != typeof(Sprite))
                {
                    readData = loadedAssetBundles[bundleName].LoadAsset(assetName);
                }
                else // if we are loading a Sprite asset, then use the parameterized load method (because casts don't seem to work with Sprite type assets)
                {
                    readData = loadedAssetBundles[bundleName].LoadAsset<Sprite>(assetName);
                }

                if (readData is T)
                {
                    return (T)(object)readData;
                }
                else if (readData != null)
                {
                    try
                    {
                        return (T)System.Convert.ChangeType(readData, typeof(T));
                    }
                    catch (System.InvalidCastException e)
                    {
                        Logging.LogException(e);
                    }
                }
            }
            else // if the bundle does NOT contain the specified asset
            {
                Logging.LogError("Bundle " + bundleName + " does not contain " + assetName);
            }
        }
        else // if the bundle is NOT loaded
        {
            // log an error
            Logging.LogError("Bundle " + bundleName + " has not been loaded! Unable to get " + assetName + " from bundle.");
        }

        // return the default for the type
        return default(T);
    }

    #endregion

    /// <summary>
    /// Check if a bundle has been downloaded and loaded into memory.
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static bool BundleLoaded(string bundleName)
    {
        // make sure that loadedAssetBundles has been isntantiated
        if (loadedAssetBundles == null)
        {
            loadedAssetBundles = new Dictionary<string, AssetBundle>();
            return false;
        }

        // convert the bundle name to lowercase as required by Unity
        string lwrBundleName = bundleName.ToLower();

        // if the bundle name key exists, and the bundle is NOT null, then return true
        if (loadedAssetBundles.ContainsKey(lwrBundleName) && loadedAssetBundles[lwrBundleName] != null) return true;

        // default to false
        return false;
    }
}
