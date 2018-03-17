using UnityEngine;

public class BallReleaseHandler : MonoBehaviour, IEventListener
{
    public BallBunch BallBunch;
    public int BallBunchIndex;

    private void Awake()
    {
        BallBunch = GetComponentInParent<BallBunch>();
    }

    [ContextMenu("ReleaseBalls")]
    public void OnBallReleaseSelected()
    {
        Debug.Log("BallReleaseHandler| OnBallReleaseSelected()");
        EventManager.instance.TriggerEvent(new EventBallBunchPressed(this));
    }
    
    private void ReleaseBalls()
    {
        Debug.Log("BallReleaseHandler| ReleaseBalls()");
        BallBunch.ReleaseBalls();

        gameObject.SetActive(false);
    }


    void Start()
    {
        ManageListeners(EventManager.HandleMode.Attach);
    }

    void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch (evtName)
        {
            case EventRequestBallBunchRelease.EventName:
                int handlerToRelease =  (int)(evt.GetData());
                if (handlerToRelease == this.BallBunchIndex)
                {
                    ReleaseBalls();
                }
                break;
            case EventBallBunchResetRequested.EventName:
                BallBunch.ResetBalls();
                gameObject.SetActive(true);
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

        EventManager.ManageListener(mode, this, EventRequestBallBunchRelease.EventName);
        EventManager.ManageListener(mode, this, EventBallBunchResetRequested.EventName);
    }
}
