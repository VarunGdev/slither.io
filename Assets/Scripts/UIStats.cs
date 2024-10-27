using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class UIStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lengthText;

    private void OnEnable()
    {
        PlayerLength.ChangedLengthEvent += ChangeLengthText;
    }

    private void OnDisable()
    {
        PlayerLength.ChangedLengthEvent -= ChangeLengthText;
    }

    private void ChangeLengthText(ushort length)
    {
        lengthText.text =  length.ToString();
    }
}
