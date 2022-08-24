using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;



namespace Iridescent.TimeMachine
{
    [System.Serializable]
    public class TimeMachineControlBehaviour : PlayableBehaviour 
    {
        [SerializeField] public TimeMachineClipEvent timeMachineClipEvent;
        // public bool isFinishRole;
        // public bool mute;
        // public string displayName { get; set; }
         
    }
}
