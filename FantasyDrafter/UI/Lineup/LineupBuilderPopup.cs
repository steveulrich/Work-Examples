using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupBuilderPopup : MonoBehaviour, IEventListener {

    public Animator LineupPopupAnim;
    public Transform LineupScrollingWindow;
    public UnityEngine.UI.Image BackgroundPanel;
    public UnityEngine.UI.Text SalaryText;

    private int OpenAnimHash;
    private int CloseAnimHash;

    private void Awake()
    {
        OpenAnimHash = Animator.StringToHash("OpenLineupPopup");
        CloseAnimHash = Animator.StringToHash("CloseLineupPopup");
    }

    private void Start()
    {
        ManageListeners(EventManager.HandleMode.Attach);
    }

    private void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    private void OnEnable()
    {
        LineupPopupAnim.Play(OpenAnimHash, -1, 0);
    }

    private void OnDisable()
    {
        DisablePopup();
    }

    public void DisablePopup()
    {
        gameObject.SetActive(false);
    }

    public void OnClosePopup()
    {
        LineupPopupAnim.Play(CloseAnimHash);
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch (evtName)
        {
            case EventContestEntryPlayerAdded.EventName:
                // Close the UI
                OnClosePopup();
                break;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if (mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            return;
        }

        EventManager.ManageListener(mode, this, EventContestEntryPlayerAdded.EventName);
    }

}
