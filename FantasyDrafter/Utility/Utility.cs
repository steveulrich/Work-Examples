using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend
{

    public class Utility
    {

        public static int maxSlotsPerContest
        {
            get
            {
                int maxSlots = 0;
                if(!Backend.DynamicProgramProperties.TryGetValue("MaxSlotsPerContest", out maxSlots))
                {
                    maxSlots = 5;
                }

                return maxSlots;
            }
        }

        public static WaitForEndOfFrame WaitForFrame = new WaitForEndOfFrame();

        public static GenericPopup MakeNewGenericPopup(string titleText, string messageText, bool showTwoButtons, string buttonOneText = "Ok", string buttonTwoText = "Cancel")
        {
            GenericPopup popup = UnityEngine.MonoBehaviour.Instantiate(Resources.Load<GenericPopup>("GenericPopup")) as GenericPopup;

            popup.TitleText.text = titleText;
            popup.MessageText.text = messageText;

            if(showTwoButtons)
            {
                popup.CancelButton.gameObject.SetActive(true);
            }
            else
            {
                popup.CancelButton.gameObject.SetActive(false);
            }

            popup.OkButtonText.text = buttonOneText;
            popup.CancelButtonText.text = buttonTwoText;

            return popup;
        }

        public static void CleanMemory()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static IEnumerator LoadScene(string sceneName)
        {
            yield return Loading.FadeToOpaque();

            yield return Backend.Utility.WaitForFrame;

            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

            yield return Backend.Utility.WaitForFrame;

            yield return Loading.FadeToClear();

        }

        #region Web Helpers

        /// <summary>
        /// A simple Coroutine that checks internet connectivity using google.com.
        /// Will call the callbackOnTestComplete with a true or false depending on the status of the WWW request
        /// </summary>
        /// <param name="callbackOnTestComplete"></param>
        /// <returns></returns>
        public static IEnumerator InternetConnectionTest(System.Action<bool> callbackOnTestComplete)
        {
            UnityWebRequest www = new UnityWebRequest("http://google.com");
            yield return www.Send();
            if (www.error != null)
            {
                callbackOnTestComplete(false);
            }
            else
            {
                callbackOnTestComplete(true);
            }
        }

        /// <summary>
        /// Run a web request with a string callback.
        /// </summary>
        public static IEnumerator WebRequest(string url, System.Action<byte[]> callback = null)
        {
            string httpsUrl = ForceHttps(url);

            Logging.Log("Sending web request to " + httpsUrl + "...");
            // start the web request
            UnityWebRequest www = UnityWebRequest.Get(httpsUrl);
            // wait for it
            yield return www.Send();

            // if there is an error, then print it
            if (www == null)
            {
                Logging.LogError("Error in request to " + httpsUrl + "!\n Unable to create www object!");
            }
            else if (www.error != null)
            {
                Logging.LogError("Error in request to " + httpsUrl + "!\n" + www.error);
            }
            else
            {
                Logging.Log("Successful web request to " + httpsUrl);
            }

            // when it's done, if the callback is not null
            if (callback != null)
            {
                byte[] result = null;

                if (www != null)
                {
                    result = www.downloadHandler.data;
                    // make sure the memory manager can clean up this request
                    www.Dispose();
                    www = null;
                }

                // call the callback, passing it the returned text
                callback(result);
            }
        }

        /// <summary>
        /// Run a web request with a string callback.
        /// </summary>
        public static IEnumerator WebRequest(string url, System.Action<string> callback = null)
        {
            string httpsUrl = ForceHttps(url);

            Logging.Log("Sending web request to " + httpsUrl + "...");
            // start the web request
            UnityWebRequest www = UnityWebRequest.Get(httpsUrl);
            // wait for it
            yield return www.Send();

            // if there is an error, then print it
            if (www == null)
            {
                Logging.LogError("Error in request to " + httpsUrl + "!\n Unable to create www object!");
            }
            else if (www.error != null)
            {
                Logging.LogError("Error in request to " + httpsUrl + "!\n" + www.error);
            }
            else
            {
                Logging.Log("Successful web request to " + httpsUrl);
            }

            // when it's done, if the callback is not null
            if (callback != null)
            {
                string result = "";

                if (www != null)
                {
                    result = www.downloadHandler.text;
                    // make sure the memory manager can clean up this request
                    www.Dispose();
                    www = null;
                }

                // call the callback, passing it the returned text
                callback(result);
            }
        }

        /// <summary>
        /// Make sure that a request uses https
        /// </summary>
        /// <param name="url">the url to convert</param>
        /// <returns></returns>
        public static string ForceHttps(string url)
        {
            if (Debug.isDebugBuild) return url;

            string strippedUrl = url.Replace("http://", "").Replace("https://", "");
            return "https://" + strippedUrl;
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Enclosing accessor method for DateTime.UtcNow, in case we ever change 
        /// how dates are handled or accessed.
        /// </summary>
        public static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        public static DateTime ConvertTimestampToDatetime(string timestamp)
        {
            if (string.IsNullOrEmpty(timestamp))
            {
                Logging.LogWarning("The timestamp passed into Utility.ConvertTimestampToDatetime is null or empty!");
            }

            DateTime newDateTime = DateTime.Now;
            if (!DateTime.TryParse(timestamp, out newDateTime))
            {
                Logging.LogWarning("Unable to parse '" + timestamp + "' as a DateTime! Using DateTime.Now!");
            }
            return newDateTime;
        }
        
        /// <summary>
        /// Convert a Datetime to a format that SQL will like.
        /// </summary>
        public static long ConvertDateTimeToLongMilliseconds(DateTime date)
        {
            return (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// Convert a DateTime to a timestamp.
        /// </summary>
        public static string ConvertDateTimeToTimestamp(DateTime date)
        {
            string convertedDate = date.ToString("yyyy-MM-dd HH:mm:ss");
            //Logging.Log("ConvertDateTimeToTimestamp: " + convertedDate, Logging.MessageFilter.GameData);
            return convertedDate;
        }

        /// <summary>
        /// Convert a DateTime to a short representation as a numeric string with only a 2-digit year.
        /// 
        /// The format string is "yyMMddHHmmss". This is intended to be used as a unique moment identifier, rather than something to be indexed.
        /// </summary>
        public static string ConvertDateTimeToNumeric(DateTime date)
        {
            return date.ToString("yyMMddHHmmss");
        }

        /// <summary>
        /// Convert a UTF8 encoded string to base 64.
        /// </summary>
        public static string ConvertStringToBase64(string input)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var base64 = Convert.ToBase64String(bytes);
                return base64;
            }
            catch (Exception e)
            {
                Logging.LogException(e);
                Logging.LogError("Unable to convert " + input + " to base 64 string! " + e.Message);
            }
            return "";
        }

        #endregion

        #region Parsing

        /// <summary>
        /// This mimics Enum.TryParse, which is in the current version of .Net, but not in the version used by Unity as of this writing.
        /// </summary>
        public static bool TryParseEnum<T>(string inputValue, out T outputValue)
        {
            try
            {
                object tmpObject = Enum.Parse(typeof(T), inputValue, true);
                if (tmpObject != null && tmpObject is T)
                {
                    outputValue = (T)tmpObject;
                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogError("Unable to parse enum! " + e.Message);
            }
            outputValue = default(T); // the default value for enums in C# is ALWAYS 0 - set the value you want to be the default to 0
            return false;
        }

        // from http://stackoverflow.com/questions/894263/how-to-identify-if-a-string-is-a-number
        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static bool IsInt(object inputObject)
        {
            int returnNumber;
            string inputString = Convert.ToString(inputObject);
            bool isInt32 = false;

            if (!string.IsNullOrEmpty(inputString))
            {
                isInt32 = Int32.TryParse(inputString,
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.NumberFormatInfo.InvariantInfo,
                    out returnNumber);
            }

            return isInt32;
        }

        public static string RemoveNonAlphaNumericCharacters(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }

        public static string RandomAlphaNumericString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        /// <summary>
        /// Return a stream that can stream a string.
        /// </summary>
        public static System.IO.Stream GenerateStreamFromString(string s)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);

            // make sure the string is encoded as utf 8
            byte[] bytes = Encoding.Default.GetBytes(s);
            string utf8String = Encoding.UTF8.GetString(bytes);

            writer.Write(utf8String);

            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion

    }

}
