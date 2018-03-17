using UnityEngine;

public class SVRInteractable : MonoBehaviour {

    public event System.Action OnOver;             // Called when the gaze moves over this object
    public event System.Action OnOut;              // Called when the gaze leaves this object
    public event System.Action OnClick;            // Called when click input is detected whilst the gaze is over this object.
    public event System.Action OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
    public event System.Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
    public event System.Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.

    protected bool m_IsOver;

    public UnityEngine.Events.UnityEvent OnClickCallback;
    public System.Action clickCB;

    public bool IsOver
    {
        get { return m_IsOver; }              // Is the gaze currently over this object?
    }

    private void Start()
    {
        clickCB = delegate { OnClickCallback.Invoke(); };

        OnClick += clickCB;
        OnClick += PlaySound;
    }

    private void PlaySound()
    {
        if (OnClickCallback != null)
        {
            EventManager.instance.TriggerEvent(new GoodClickSound());
        }
        else
        {
            EventManager.instance.TriggerEvent(new BadClickSound());
        }
    }

    private void OnDestroy()
    {
        OnClick -= clickCB;
    }

    // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
    // They in turn call the appropriate events should they have subscribers.
    public void Over()
    {
        m_IsOver = true;

        if (OnOver != null)
            OnOver();
    }

    public void Out()
    {
        m_IsOver = false;

        if (OnOut != null)
            OnOut();
    }

    public void Click()
    {
        if (OnClick != null)
            OnClick();
    }

    public void DoubleClick()
    {
        if (OnDoubleClick != null)
            OnDoubleClick();
    }

    public void Up()
    {
        if (OnUp != null)
            OnUp();
    }

    public void Down()
    {
        if (OnDown != null)
            OnDown();
    }
}