using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ModeSelection : MonoBehaviour
{
    public Button button_Messages, button_Build, button_ResourceView, button_Orbit;

    public void EnableAllButtons(bool enable)
    {
        button_Messages.interactable = button_Build.interactable
        = button_ResourceView.interactable = button_Orbit.interactable = enable;
    }

    public void Show(bool show)
    {
        EnableAllButtons(show);
        gameObject.SetActive(show);
    }

    public void ClearButtonListeners()
    {
        button_Messages.onClick.RemoveAllListeners();
        button_Build.onClick.RemoveAllListeners();
        button_ResourceView.onClick.RemoveAllListeners();
        button_Orbit.onClick.RemoveAllListeners();
    }


}
