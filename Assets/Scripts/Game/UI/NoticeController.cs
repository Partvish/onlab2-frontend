using TMPro;
using System.Collections.Generic;
using UnityEngine;


public class NoticeController : MonoBehaviour
{
    [SerializeField]
    private GameObject messageRoot = null;

    [SerializeField]
    private TextMeshProUGUI messageText = null;

    public void SetMessage(string text)
    {
        messageText.text = text;
    }

}
