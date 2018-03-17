using System.Collections.Generic;

public class MainMenuNavigationEnumComparer : IEqualityComparer<MainMenuUINavigation.MainMenuDialogs>
{
    public bool Equals(MainMenuUINavigation.MainMenuDialogs x, MainMenuUINavigation.MainMenuDialogs y)
    {
        return x == y;
    }

    public int GetHashCode(MainMenuUINavigation.MainMenuDialogs x)
    {
        return (int)x;
    }
}