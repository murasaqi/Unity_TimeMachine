using System;
using Iridescent.TimeMachine;
using UnityEngine;

[Serializable]
public abstract class TimeMachineOscEvent
{
    public string oscAddress;
    // public string oscValue;
}

[Serializable]
public class TimeMachineOscMoveSectionEvent : TimeMachineOscEvent
{
    [SerializeField,NonEditable] public int clipIndex;
    [SerializeField,NonEditable] public string sectionName;
    [SerializeField] public float offsetTime = 0f;
}

[Serializable]
public class TimeMachineOscPlayerOscEvent : TimeMachineOscEvent
{
    [SerializeField] public TimeMachinePlayerEventType playerEvent;
}


[Serializable]
public enum TimeMachinePlayerEventType
{
    FinishCurrentRole,
    ResetAndReplay,
    Stop,
}