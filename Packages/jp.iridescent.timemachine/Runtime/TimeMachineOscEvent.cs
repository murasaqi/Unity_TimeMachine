using System;
using Iridescent.TimeMachine;
using UnityEngine;

[Serializable]
public class TimeMachineOscEvent
{
    [HideInInspector]public int clipIndex;
    [SerializeField,NonEditable]
    public string clipName;
    public string oscAddress;
    // public string oscValue;
}