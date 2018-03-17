using UnityEngine;
using System.Collections.Generic;
using FullInspector;

public class MainMenuUINavigation : BaseBehavior, IEventListener
{
    public enum MainMenuDialogs
    {
        PlayOptions,
        Customize,
        Store,
        Leaderboards

    }

    public IUIDialog CurrentDialog;

    public Dictionary<MainMenuDialogs, IUIDialog> UIDialogs = new Dictionary<MainMenuDialogs, IUIDialog>(new MainMenuNavigationEnumComparer());

    private void Start()
    {
        ManageListeners(EventManager.HandleMode.Attach);

        foreach(var dialog in UIDialogs)
        {
            IUIDialog uiDialog = dialog.Value;

            uiDialog.MoveOffScreen();
        }
    }

    private void SwitchDialog(IUIDialog target)
    {
        if (CurrentDialog != null)
        {
            CurrentDialog.MoveOffScreen();
        }

        if (target != CurrentDialog)
        {
            target.MoveOnScreen();
            CurrentDialog = target;

        }
        else
        {
            CurrentDialog = null;
        }

    }

    private void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    #region EventListener Stuffs
    public ListenerResult HandleEvent(IEvent evt)
    {
        string eventName = evt.GetName();

        switch(eventName)
        {
            case EventChangeMainMenuDialog.EventName:
                MainMenuUINavigation.MainMenuDialogs dialog = (MainMenuUINavigation.MainMenuDialogs)evt.GetData();

                SwitchDialog(UIDialogs[dialog]);

                break;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if (mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            Debug.Log("MainMenuUINavigation| No EventManager to detach from");
            return;
        }

        EventManager.ManageListener(mode, this, EventChangeMainMenuDialog.EventName);
    }
    #endregion
}
