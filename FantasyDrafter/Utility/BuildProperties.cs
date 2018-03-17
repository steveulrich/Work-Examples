using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Backend
{
    /// <summary>
    /// This class acts as an accessor for the properties that
    /// are injected into a build by Unity Cloud Build.
    /// 
    /// This is what is used for coordinating builds with 
    /// specific bundles. This is a layer under the DynamicProgramProperties class,
    /// which may reach into this class for specific stuff.
    /// 
    /// http://forum.unity3d.com/threads/how-to-get-build-commit-id-from-unity-cloud-build.296307/
    /// https://build.cloud.unity3d.com/support/guides/manifest/
    /// </summary>
    public class BuildProperties
    {
        #region Fields and Properties
        private static Dictionary<string, object> properties;

        // strings that can be used to load the various data sources
        private const string BUNDLE_VERSION_FILE = "Properties/BundleVersion";
        private const string BUNDLE_VERSION_LKG_FILE = "Properties/BundleVersionLKG";
        private const string BUILD_MANIFEST_FILE = "UnityCloudBuildManifest.json";
        private static bool isInitialized = false;
        #endregion

        /// <summary>
        /// Read the build properties from local files only.
        /// </summary>
        public static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            properties = new Dictionary<string, object>();

            // 1. Load build properties json - don't forget to omit the extension on the asset path within resources!
            var jsonAsset = Resources.Load<TextAsset>(BUILD_MANIFEST_FILE);
            string json = "";
            if (jsonAsset != null)
            {
                json = jsonAsset.text;
                LoadAllProperties(json);
            }
            else
                Logging.LogWarning("jsonAsset is null! Unable to load local build properties json! This SHOULD occur if you're testing in Editor. Don't freak out.");

            // 2. Load build properties from extra text files

            var bundleVersionAsset = Resources.Load<TextAsset>(BUNDLE_VERSION_FILE);
            int bundleVersion = -1;
            if (bundleVersionAsset != null)// && Backend.Utility.IsNumeric(bundleVersionAsset.text))
            {
                bundleVersion = System.Int32.Parse(bundleVersionAsset.text);

                Logging.Log("BuildProperties: Loaded BundleVersion " + bundleVersion + "...");
            }
            else
                Logging.LogError("Unable to load bundle version from " + BUNDLE_VERSION_FILE);

            properties["BundleVersion"] = bundleVersion;

            var bundleVersionLKGAsset = Resources.Load<TextAsset>(BUNDLE_VERSION_LKG_FILE);
            int bundleVersionLKG = -1;
            if (bundleVersionLKGAsset != null)// && Backend.Utility.IsNumeric(bundleVersionLKGAsset.text))
            {
                bundleVersionLKG = System.Int32.Parse(bundleVersionLKGAsset.text);

                Logging.Log("BuildProperties: Loaded BundleVersionLKG " + bundleVersionLKG + "...");
            }
            else
                Logging.LogError("Unable to load bundle version LKG from " + BUNDLE_VERSION_LKG_FILE);

            properties["BundleVersionLKG"] = bundleVersionLKG;

            // set the default protocol to https
            properties["protocol"] = "https://";
            isInitialized = true;
        }

        /// <summary>
        /// Try to get a build property.
        /// </summary>
        /// <typeparam name="T">The type of the out argument</typeparam>
        /// <param name="key">The string name for the key</param>
        /// <param name="value">The out argument in which will be stored the retrieved value OR THE DEFAULT VALUE FOR THE TYPE IF NOT FOUND!</param>
        /// <returns>true or false</returns>
        public static bool TryGetValue<T>(string key, out T value)
        {
            // if the dictionary hasn't been initialized then initialize it
            if (properties == null)
                properties = new Dictionary<string, object>();

            // if the key exists in the dictionary
            if (properties.ContainsKey(key))
            {
                object tmpObject = properties[key];
                //SafeLog(key + " " + tmpObject);
                if (tmpObject != null)
                {
                    try
                    {
                        value = (T)tmpObject;

                        //SafeLog("BuildProperties." + key + " = " + value.ToString());

                        return true;
                    }
                    catch (System.Exception err)
                    {
                        Logging.LogError("BuildProperties: Failed to convert value in " + key + " to " + typeof(T).Name + "! Object is of type " + tmpObject.GetType().Name + "\n" + err.Message);
                    }
                }
            }

            // if something went wrong, then return the default for the type
            value = default(T);
            return false;
        }


        private static void LoadAllProperties(string json)
        {
            if (json.Length > 0)
            {
                Logging.Log("Attempting to load build properties...");

                // try to deserialize the json from the file
                Dictionary<string, object> jsonDict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;

                if (jsonDict == null) return;

                // loop through each new property and add it to the existing properties
                foreach (KeyValuePair<string, object> kvp in jsonDict)
                {
                    //SafeLog(kvp.Key + " = " + kvp.Value.ToString());

                    properties[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                Logging.LogWarning("Unable to load build properties from empty json! You will always get this warning in editor!");
            }
        }

    }
}
