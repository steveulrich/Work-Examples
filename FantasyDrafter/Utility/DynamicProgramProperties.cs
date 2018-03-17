using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Backend
{

    /// <summary>
    /// This class enables access to dynamic properties. Dynamic properties include things like
    /// the bundle version to use, announcement links, etc.
    /// 
    /// Dynamic properties are scoped according to the following keys.
    /// Default > Platform > Release-Type > Version > Platform_Release-Type > Platform_Release-Type_Version
    /// 
    /// So if you are playing a debug build on iOS with version 1.0.1, then properties would be loaded in the following order.
    /// Default > iOS > Debug > 1.0.1 > iOS_Debug > iOS_Debug_1.0.1
    /// 
    /// The more specific scope overrides and adds to the settings in the less-specific scopes.
    /// </summary>
    public class DynamicProgramProperties
    {
        
        #region Fields and Properties
        private static Dictionary<string, object> properties;

        private static bool isInitialized = false;
        // the method to be called AFTER initialization
        private static System.Action<bool> initializeCallback = null;

        #endregion

        /// <summary>
        /// Read the dynamic properties from the online json file.
        /// </summary>
        public static IEnumerator Initialize(System.Action<bool> newInitializeCallback = null)
        {
            if (isInitialized)
            {
                yield break;
            }
            // store the callback for use after we are sure we have loaded all relevant properties, which may require waiting for a web request
            initializeCallback = newInitializeCallback;

            properties = new Dictionary<string, object>();

            new GameSparks.Api.Requests.GetPropertyRequest().SetPropertyShortCode("fantasyJamProperties").Send((response) =>
            {
                bool propertiesLoaded = false;
                if (response.HasErrors)
                {
                    Logging.LogError("UNABLE TO LOAD DYNAMIC PROPERTIES");
                    propertiesLoaded = false;
                }
                else
                {
                    Backend.DynamicProgramProperties.LoadPropertiesForAllScopes(response.Property.JSON);

                    propertiesLoaded = true;
                    
                }
                if (newInitializeCallback != null)
                {
                    newInitializeCallback(propertiesLoaded);
                }
            });
        }

        /// <summary>
        /// Read the dynamic properties synchronously from the online json file.
        /// </summary>
        public static void Initialize()
        {
            properties = new Dictionary<string, object>();

            new GameSparks.Api.Requests.GetPropertyRequest().SetPropertyShortCode("fantasyJamProperties").Send((response) =>
            {
                if (response.HasErrors)
                {
                    Logging.LogError("UNABLE TO LOAD DYNAMIC PROPERTIES");
                }
                else
                {
                    Backend.DynamicProgramProperties.LoadPropertiesForAllScopes(response.Property.JSON);
                }
            });
        }

        public static bool GetBool(string key)
        {
            bool tmp = false;
            if (TryGetValue<bool>(key, out tmp))
                return tmp;

            return false;
        }

        /// <summary>
        /// Try to get a dynamic program property.
        /// </summary>
        /// <typeparam name="T">The type of the out argument</typeparam>
        /// <param name="key">The string name for the key</param>
        /// <param name="value">The out argument in which will be stored the retrieved value OR THE DEFAULT VALUE FOR THE TYPE IF NOT FOUND!</param>
        /// <returns>true or false</returns>
        public static bool TryGetValue<T>(string key, out T value)
        {
            // if the dictionary{ hasn't been initialized then initialize it
            if (properties == null)
            {
                // Initialize it
                DynamicProgramProperties.Initialize();
            }

            // if the key exists in the dictionary
            if (properties.ContainsKey(key))
            {
                object tmpObject = properties[key];

                if (tmpObject != null)
                {
                    try
                    {
                        value = (T)tmpObject;
                        return true;
                    }
                    catch (System.Exception err)
                    {
                        Logging.LogError("Failed to convert value in " + key + ": to " + typeof(T).Name + "! Value is " + tmpObject.GetType().Name + "\n" + err.Message);
                    }
                }
            }

            // if something went wrong, then return the default for the type
            value = default(T);
            return false;
        }



        public static bool ContainsKey(string key)
        {
            if (properties == null) return false;
            return properties.ContainsKey(key);
        }

        #region Downloading and Parsing JSON

        /// <summary>
        /// Accessor method for getting the list of properties.
        /// </summary>
        public static Dictionary<string, object> GetProperties()
        {
            var dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in properties)
                dict[kvp.Key] = kvp.Value;

            return dict;
        }

        public static void LoadPropertiesForAllScopes(string json)
        {
            if (json.Length > 0)
            {
                // get the variables that will describe dynamic properties scopes
                string platform = Application.platform == RuntimePlatform.Android ? "Android" : "iOS";

                string releaseType = Debug.isDebugBuild ? "Debug" : "Release";

                string version;
                //Work around till unity bug fix, Application.version doesn't work on ios
                //We are reading from the initial Local scope that was loaded before this
                if (!TryGetValue<string>("Version", out version))
                    version = "0.0.0"; //Default version to fallback on

                // make a list of all valid scopes
                List<string> scopes = new List<string>();
                scopes.Add("Default");
                scopes.Add(platform);
                scopes.Add(releaseType);
                scopes.Add(version);
                scopes.Add(platform + "_" + releaseType);
                scopes.Add(platform + "_" + releaseType + "_" + version);

                // load the dynamic properties for each scope in order
                foreach (string scope in scopes)
                {
                    LoadPropertiesForScope(json, scope);
                }
            }
            else
            {
                Logging.LogError("No json retrieved for dynamic properties. You may not be connected to the internet!");
            }
        }

        /// <summary>
        /// Load dynamic properties for the given scope from the given json.
        /// </summary>
        /// <param name="json">the json to parse</param>
        /// <param name="scopeKey">the scope to load</param>
        private static void LoadPropertiesForScope(string json, string scopeKey)
        {
            if (json.Length > 0)
            {
                Logging.Log("Attempting to load dynamic properties for scope " + scopeKey);
                try
                {
                    // try to deserialize the json from the file
                    Dictionary<string, object> jsonDict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
                    if (jsonDict != null && jsonDict.ContainsKey(scopeKey))
                    {
                        Dictionary<string, object> newProperties = jsonDict[scopeKey] as Dictionary<string, object>;

                        // loop through each new property and add it to the existing properties
                        foreach (KeyValuePair<string, object> kvp in newProperties)
                        {
                            Logging.Log(kvp.Key + " = " + kvp.Value.ToString());

                            properties[kvp.Key] = kvp.Value;
                        }
                    }
                    else
                    {
                        Logging.LogWarning("Dynamic properties not found for scope " + scopeKey);
                    }
                }
                catch (System.Exception e) // if someone makes a mistake in the dynamic properties, then we should try to go on without them
                {
                    Logging.LogException(e);
                    Logging.LogError("Error while parsing dynamic propertied! " + e.Message);
                }
            }
            else
            {
                Logging.LogError("Unable to load dynamic properties from empty json!");
            }
        }
        #endregion
    }
    
}