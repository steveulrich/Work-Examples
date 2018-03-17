using UnityEngine;

/// <summary>
/// Helper class designed to be attached to gameobjects we want to persist
/// when no DontDestroyOnLoad method is called on that object already
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
}
