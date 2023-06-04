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
public class TimeMachineOscMoveScetionEvent : TimeMachineOscEvent
{
    [SerializeField,NonEditable] public int clipIndex;
    [SerializeField,NonEditable] public string sectionName;
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