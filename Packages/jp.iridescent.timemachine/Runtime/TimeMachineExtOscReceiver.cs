using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iridescent.TimeMachine;
#if USE_EXTOSC
using extOSC;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineOscEvent
{
    [HideInInspector]public int clipIndex;
    [SerializeField,NonEditable]
    public string clipName;
    public string oscValue;
}

public class TimeMachineExtOscReceiver : MonoBehaviour
{
    public string oscAddress = "/TimeMachine/MoveTo";
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
        if(oscReceiver == null) return;
        if(timeMachineTrackManager== null) return;
        
        timeMachineOscEvents.Clear();
       

        var clips = timeMachineTrackManager.clips;
        
        if(clips == null) return;
        foreach (var clip in clips)
        {
            var timeMachineControlClip = clip.asset as TimeMachineControlClip;
            timeMachineOscEvents.Add(new TimeMachineOscEvent()
            {
                clipIndex = timeMachineControlClip.clipIndex,
                clipName = clip.displayName,
                oscValue = "C"+timeMachineControlClip.clipIndex.ToString()
            });
        }
        timeMachineOscEvents.Add(new TimeMachineOscEvent()
        {
            clipIndex = -1,
            clipName = "Finish Current Role",
            oscValue = "Finish"
        });
        
        
        timeMachineOscEvents.Sort((a, b) => a.clipIndex.CompareTo(b.clipIndex));
    }

    [ContextMenu("Bind")]
    public void Bind()
    {
        
        oscReceiver.Bind(oscAddress, (message) =>
        {
            
            var value = message.FindValues(OSCValueType.String).First().Value as string;
            
            if (value != null)
            {
                if (value == "Finish")
                {
                    timeMachineTrackManager.FinishCurrentClip();
                }
                else
                {
                    foreach (var timeMachineOscEvent in timeMachineOscEvents)
                    {
                        if (value == timeMachineOscEvent.oscValue)
                        {
                            var index = timeMachineOscEvent.clipIndex;
                            timeMachineTrackManager.MoveClip(index);     
                        }
                   
                    }   
                }
                 
            }
            
        });
        
    }

}

#endif
