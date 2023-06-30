using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldMobileSupport : MonoBehaviour
#if UNITY_WEBGL && !UNITY_EDITOR
    ,IPointerClickHandler
#endif
{
    public InputField Input;

    public string message;

    private void Awake()
    {
        Input = GetComponent<InputField>();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void Prompt(string name, string message, string defaultValue);

    public void OnPointerClick(PointerEventData eventData)
    {
        var str =  Input.text;
        if (string.IsNullOrWhiteSpace(str))
        {
            str = message;
        }
        Prompt(name, message, str);
    }

    public void OnPromptOk(string message)
    {
        Input.text = message;
    }

    public void OnPromptCancel()
    {
        Input.text = "";
    }
#endif
}