using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : Singleton<MainMenuUI>
{

    public MainMenuHeader MenuHeader;
    public MainMenuBody MenuBody;
    public MainMenuFooter MenuFooter;

    public ViewsNavigation Navigation;

	void Awake()
    {
        if(User.ActiveUser != null)
        {
            MenuHeader.DisplayNameText.text = User.ActiveUser.DisplayName;
        }
    }

    public void OnPressMyAccountInfo()
    {

    }

}
