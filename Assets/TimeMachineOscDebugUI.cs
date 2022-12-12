using System.Collections;
using System.Collections.Generic;
using extOSC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeMachineOscDebugUI : MonoBehaviour
{

    public GameObject buttonPrefab;
    public OSCTransmitter oscTransmitter;
    public TimeMachineExtOscReceiver timeMachineOscReceiver;
    // Start is called before the first frame update
    void Start()
    {
        if (timeMachineOscReceiver != null)
        {
            foreach (var timeMachineOscEvent in timeMachineOscReceiver.timeMachineOscEvents)
            {
                var instantiate = Instantiate(buttonPrefab, transform);
                instantiate.transform.SetParent(transform);
                
                instantiate.GetComponentInChildren<TextMeshProUGUI>().text = timeMachineOscReceiver.oscAddress;
                var button = instantiate.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    var message = new OSCMessage(timeMachineOscReceiver.oscAddress, new OSCValue[]
                    {
                        new OSCValue( OSCValueType.String, timeMachineOscEvent.oscValue)
                    });
                    if(oscTransmitter)oscTransmitter.Send(message);
                });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
