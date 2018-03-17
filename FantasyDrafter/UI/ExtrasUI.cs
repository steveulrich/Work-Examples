using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasUI : MonoBehaviour {

    private const string DONATE_URL = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=UGPDFGKGPUSXN&lc=US&item_name=Developer%20Appreciation&item_number=Breakaway%20Fantasy%20Drafter&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted";

    private string twitterURL
    {
        get
        {
            string url = string.Empty;
            if(!Backend.DynamicProgramProperties.TryGetValue("TwitterURL", out url))
            {
                url = "http://twitter.com/ButtaButtaJam_";
            }

            return url;
        }
    }

    private string twitchURL
    {
        get
        {
            string url = string.Empty;
            if (!Backend.DynamicProgramProperties.TryGetValue("TwitchURL", out url))
            {
                url = "http://twitch.tv/ButtaButtaJam";
            }

            return url;
        }
    }

    public void OnDonateButtonPressed()
    {
        GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Donate", "THANK YOU! Press Ok to be taken out of Breakaway Fantasy Draft to Paypal's Donation Portal", true);
        popup.OkButton.onClick.AddListener(() => Application.OpenURL(DONATE_URL));
    }

    public void OnTwitterPressed()
    {
        Application.OpenURL(twitterURL);
    }

    public void OnTwitchPressed()
    {
        Application.OpenURL(twitchURL);
    }

	public void OnCloseFinished()
    {
        gameObject.SetActive(false);
    }
}
