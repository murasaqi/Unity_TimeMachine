using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Iridescent.TimeMachine;
#if USE_EXTOSC
using extOSC;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineOscEvent
{
    public TimeMachineControlClip timeMachineControlClip;
    public string oscAddress;

}

public class TimeMachineExtOscReceiver : MonoBehaviour
{

    public TimeMachineTrackManager timeMachineTrackManager;
    public List<TimeMachineOscEvent> timeMachineOscEvents = new List<TimeMachineOscEvent>();
    public OSCReceiver oscReceiver;
    private void Start()
    {
        Bind();
    }

    private void Update()
    {
        
    }

    [ContextMenu("Init")]
    public void Init()
    {
        timeMachineOscEvents.Clear();
        if(timeMachineTrackManager== null) return;

        var clips = timeMachineTrackManager.clips;

        if(clips == null) return;
        foreach (var clip in clips)
        {
            var timeMachineControlClip = clip.asset as TimeMachineControlClip;
            timeMachineOscEvents.Add(new TimeMachineOscEvent()
            {
                timeMachineControlClip = timeMachineControlClip,
                oscAddress = clip.displayName
            });
        }
        if(oscReceiver == null) return;
        
    }

    [ContextMenu("Bind")]
    public void Bind()
    {
        foreach (var timeMachineOscEvent in timeMachineOscEvents)
        {
            oscReceiver.Bind(timeMachineOscEvent.oscAddress, (message) =>
            {
                var index = timeMachineOscEvent.timeMachineControlClip.clipIndex;
                timeMachineTrackManager.MoveClip(index);
            });
        }
    }

}

#endif
