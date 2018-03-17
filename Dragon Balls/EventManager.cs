//C# Unity event manager that uses strings in a hashtable over delegates and events in order to
//allow use of events without knowing where and when they're declared/defined.
//iOS friendly version that uses ArrayList instead of C# generics.
//by Billy Fletcher of Rubix Studios
//
//See this webpage for how to use this class:
//http://wiki.unity3d.com/index.php?title=CSharpEventManager

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Diagnostics;
#endif
using Debug = UnityEngine.Debug;


public enum ListenerResult { Consume, Cascade };
public interface IEventListener
{
    ListenerResult HandleEvent(IEvent evt);
    void ManageListeners(EventManager.HandleMode mode);
}

public interface IEvent
{
    string GetName();
    object GetData();
}

public class EventManager : MonoBehaviour
{
    private static bool _appIsQuitting;
    private static EventManager _instance;
    public static EventManager instance
    {
        get
        {
            if (_appIsQuitting)
            {
                Debug.LogWarning("Someone is trying to use EventManager while it is being destroyed.");
                return null;
            }

            if (_instance == null)
            {
                _instance = Resources.Load<EventManager>("EventManager");
            }
            if (_instance == null)
                Debug.LogError("_instance is null and our code will almost never be expecting this to be the case.");
            return _instance;
        }
    }

    void Awake()
    {
        _appIsQuitting = false;
    }

    //  This will check if we have an instance (that is not being destroyed) but will not create one.
    public static bool HasInstance()
    {
        if (_appIsQuitting)
            return false;

        return _instance != null;
    }

    private Hashtable _listenerTable = new Hashtable();
    private Dictionary<IEventListener, GameObject> _listenerGameObjects = new Dictionary<IEventListener, GameObject>();

    //Add a listener to the event manager that will receive any events of the supplied event name.
    public bool AddListener(IEventListener listener, string eventName)
    {
        if (listener == null || eventName == null)
        {
            Debug.Log("Event Manager: AddListener failed due to no listener or event name specified.");
            return false;
        }

        if (!_listenerTable.ContainsKey(eventName))
            _listenerTable.Add(eventName, new ArrayList());

        ArrayList listenerList = _listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return false;

        if (listenerList.Contains(listener))
        {
            Debug.Log("Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
            return false; //listener already in list
        }

        listenerList.Add(listener);

        var behaviour = listener as MonoBehaviour;
        if (behaviour != null)
        {
            var gob = behaviour.gameObject;
            if (gob != null)
            {
                _listenerGameObjects[listener] = gob;
            }
        }
        return true;
    }


    public enum HandleMode { Attach, Detach };

    //Either add or remove a listener from the subscribed event
    public static bool ManageListener(HandleMode mode, IEventListener listener, string eventName)
    {
        if(instance != null)
        {
            if (mode == HandleMode.Attach)
            {
                instance.AddListener(listener, eventName);
                return true;
            }
            return DetachListener(listener, eventName);
        }

        return false;

    }


    //Remove a listener from the subscribed to event.
    public static bool DetachListener(IEventListener listener, string eventName)
    {
        if (!HasInstance())
            return false;

        if (!_instance._listenerTable.ContainsKey(eventName))
            return false;

        ArrayList listenerList = _instance._listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return false;

        if (!listenerList.Contains(listener))
            return false;

        listenerList.Remove(listener);

        _instance._listenerGameObjects.Remove(listener);

        return true;
    }

    //Trigger the event instantly, this should only be used in specific circumstances,
    //the QueueEvent function is usually fast enough for the vast majority of uses.
    public bool TriggerEvent(IEvent evt)
    {
        string eventName = evt.GetName();
        if (!_listenerTable.ContainsKey(eventName))
        {
            Debug.LogWarning("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
            return false; //No listeners for event so ignore it
        }

        ArrayList listenerList = _listenerTable[eventName] as ArrayList;
        if (listenerList == null)
            return true;

        Debug.Log("Triggering Event :: " + eventName);

        //  Changed this to a for loop because detaching a listener during the enumeration causes errors.
        for (var i = listenerList.Count - 1; i >= 0; --i)
        {
            i = Mathf.Clamp(i, 0, listenerList.Count - 1);  //  Ran into a case once where i became out of range.  Bandaid fix.  I tried moving anything that modifies _listenerTable to outside this loop with no good results.
            var listener = (IEventListener)listenerList[i];
            if (listener == null)
            {
                ((ArrayList)_listenerTable[eventName]).RemoveAt(i);
                continue;
            }

            if (_listenerGameObjects.ContainsKey(listener) && _listenerGameObjects[listener] == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("A listener for " + eventName + " is attached to a null game object and is being removed by the EventManger.");
#endif
                ((ArrayList)_listenerTable[eventName]).RemoveAt(i);
                _listenerGameObjects.Remove(listener);
                continue;
            }

            if (listener.HandleEvent(evt) == ListenerResult.Consume)
                return true;
        }

        return true;
    }

    //  Returns true or false depending on whether the event has a registered listener.
    public bool HasListener(IEvent evt)
    {
        return _listenerTable.ContainsKey(evt.GetName());
    }

    public void OnApplicationQuit()
    {
        _listenerTable.Clear();
        _instance = null;
    }

    void OnDestroy()
    {
        _instance = null;
        //_appIsQuitting = true;
    }

    public static void Reset()
    {
        _appIsQuitting = false;
        if (_instance == null) 
            return;

        _instance._listenerTable = new Hashtable();
        _instance._listenerGameObjects = new Dictionary<IEventListener, GameObject>();
    }
}