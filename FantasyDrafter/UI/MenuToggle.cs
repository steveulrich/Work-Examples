using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : MonoBehaviour {

    public UnityEngine.UI.Toggle m_Toggle;
    public UnityEngine.UI.Text Text;

    public bool ShouldStartPressed = false;

    private void Start()
    {
        if(ShouldStartPressed)
        {
            m_Toggle.isOn = true;
        }
    }

    public void OnToggleTapped()
    {
        Text.color = m_Toggle.isOn ? Color.black : Color.white;
    }

}
