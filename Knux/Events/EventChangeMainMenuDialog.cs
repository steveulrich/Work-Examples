
public class EventChangeMainMenuDialog : IEvent
{
    public const string EventName = "EventChangeMainMenuDialog";

    MainMenuUINavigation.MainMenuDialogs dialog;

    public EventChangeMainMenuDialog(MainMenuUINavigation.MainMenuDialogs targetDialog)
    {
        dialog = targetDialog;

    }

    public object GetData()
    {
        return dialog;
    }

    public string GetName()
    {
        return EventName;
    }
}
