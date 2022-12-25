using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iridescent.TimeMachine;

#if USE_EXTOSC
using extOSC;

using UnityEngine.VFX;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(TimeMachineExtOscReceiver),true)]
[CanEditMultipleObjects]
public class TimeMachineExtOscReceiverEditor : Editor
{
    
    public TimeMachineExtOscReceiver timeMachineExtOscReceiver;
    public List<VisualElement> oscEventFields = new List<VisualElement>();
    public VisualElement container;
    public VisualElement listViewContainer;
    public ListView oscEventListView;
    public override VisualElement CreateInspectorGUI()
    {
        container = new VisualElement();
        timeMachineExtOscReceiver = target as TimeMachineExtOscReceiver;
        var oscReceiverField = new PropertyField(serializedObject.FindProperty("oscReceiver"));
       oscReceiverField.Bind(serializedObject);
        var timeMachineTrackManagerField = new PropertyField(serializedObject.FindProperty("timeMachineTrackManager"));
        timeMachineTrackManagerField.Bind(serializedObject);
        var initButton = new Button();
        initButton.text = "Init";
        initButton.clicked += () =>
        { 
            timeMachineExtOscReceiver.Init(); 
            InitListView();

            var i = 0;
            foreach (var oscEventField in oscEventFields)
            {
                var oscAddressField = oscEventField.Q<TextField>("oscAddressField");
                var oscevent = timeMachineExtOscReceiver.timeMachineOscEvents[i];
                oscAddressField.RegisterValueChangedCallback((v) =>
                {
                    oscevent.oscAddress = v.newValue;
                });
                
                var oscValueField = oscEventField.Q<TextField>("oscValueField");
                oscValueField.RegisterValueChangedCallback((v) =>
                {
                    oscevent.oscValue = v.newValue;
                });
                
            }
        };

        var timeMachineOscEventType = timeMachineExtOscReceiver.timeMachineOscEventType;
        var timeMachineOscEventTypeField = new PropertyField(serializedObject.FindProperty("timeMachineOscEventType"));
        timeMachineOscEventTypeField.Bind(serializedObject);
        var oscAddressExpressionField = new PropertyField(serializedObject.FindProperty("oscAddressExpression"));
        oscAddressExpressionField.Bind(serializedObject);
        oscAddressExpressionField.RegisterValueChangeCallback((v) =>
        {
            timeMachineExtOscReceiver.oscAddressExpression = v.changedProperty.stringValue;
        });
        var oscStringValueExpressionField = new PropertyField(serializedObject.FindProperty("oscStringValueExpression"));
        oscStringValueExpressionField.Bind(serializedObject);
        oscStringValueExpressionField.RegisterValueChangeCallback((v) =>
        {
            timeMachineExtOscReceiver.oscStringValueExpression = v.changedProperty.stringValue;
        });
        var initializeOnStartField = new PropertyField(serializedObject.FindProperty("initializeOnStart"));
        initializeOnStartField.Bind(serializedObject);
        initializeOnStartField.RegisterValueChangeCallback((v) =>
        {
            timeMachineExtOscReceiver.initializeOnStart = v.changedProperty.boolValue;
        });
        oscStringValueExpressionField.SetEnabled(timeMachineOscEventType == TimeMachineOscEventType.AddressAndValue);

        listViewContainer = new VisualElement();
        InitListView();
        
        timeMachineOscEventTypeField.RegisterValueChangeCallback((v) =>
        {
            var value = v.changedProperty.enumValueFlag;
            foreach (var oscEventField in oscEventFields)
            {
                var oscValueField = oscEventField.Q<TextField>("oscValueField");
                oscValueField.visible = value == 1;
                oscStringValueExpressionField.SetEnabled(value == 1);
            }
        });

        oscEventListView.AddToClassList("unity-collection-view");
        oscEventListView.AddToClassList("unity-list-view");
        container.Add(oscReceiverField);
        container.Add(timeMachineTrackManagerField);
        container.Add(timeMachineOscEventTypeField);
        container.Add(oscAddressExpressionField);
        container.Add(oscStringValueExpressionField);
        container.Add(initializeOnStartField);
        container.Add(initButton);
        container.Add(listViewContainer);
        
        return container;
    }

    private void InitListView()
    {
        oscEventFields.Clear();
        if(oscEventListView != null)oscEventListView.Clear();
        listViewContainer.Clear();
        oscEventListView = new ListView();
        oscEventListView.makeItem = () =>
        {
            var childContainer = new VisualElement();
            var label = new Label("clipName");
            label.name = "clipName";
            childContainer.Add(label);
            var oscAddressField = new TextField("oscAddressField");
            oscAddressField.name = "oscAddressField";
            var oscValueField = new TextField("oscValueField");
            oscValueField.name = "oscValueField";
            childContainer.Add(oscAddressField);
            childContainer.Add(oscValueField);
            childContainer.style.flexWrap = Wrap.NoWrap;
            // childContainer.style.justifyContent = Justify.;
            childContainer.style.overflow = Overflow.Visible;
            oscEventFields.Add(childContainer);
            return childContainer;
        };
        
      
        oscEventListView.bindItem = (element, index) =>
        {
            if(index >= timeMachineExtOscReceiver.timeMachineOscEvents.Count) return;
            var label = element.Q<Label>("clipName");
            var oscEvent = timeMachineExtOscReceiver.timeMachineOscEvents[index];
            
            label.text = oscEvent.clipName;
            var oscAddressField = element.Q<TextField>("oscAddressField");
            oscAddressField.value = oscEvent.oscAddress;
            var oscValueField = element.Q<TextField>("oscValueField");
            oscValueField.value = oscEvent.oscValue;
            oscValueField.visible = timeMachineExtOscReceiver.timeMachineOscEventType == TimeMachineOscEventType.AddressAndValue;
        };
        oscEventListView.bindingPath = "timeMachineOscEvents";
        oscEventListView.Bind(serializedObject);
        oscEventListView.itemsSource = timeMachineExtOscReceiver.timeMachineOscEvents;
        oscEventListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        oscEventListView.focusable = true;
        oscEventListView.showBorder = true;
        oscEventListView.selectionType = SelectionType.Multiple;
        oscEventListView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
        oscEventListView.showFoldoutHeader = true;
        oscEventListView.showAddRemoveFooter = true;
        oscEventListView.reorderMode = ListViewReorderMode.Animated;
        oscEventListView.reorderable = true;
        oscEventListView.showBoundCollectionSize = true;
        listViewContainer.Add(oscEventListView);
    }
}


#endif
[Serializable]
public class TimeMachineOscEvent
{
    public int clipIndex = 0;
    // public TimeMachineEventType timeMachineEventType = TimeMachineEventType.MoveClip;
    public TimeMachineOscEventType oscEventType = TimeMachineOscEventType.AddressAndValue;
    public string clipName = "";
    public string oscAddress = "";
    public string oscValue = "";
}

public enum TimeMachineOscEventType
{
    AddressOnly,
    AddressAndValue
}

public class TimeMachineExtOscReceiver : MonoBehaviour
{
    public TimeMachineOscEventType timeMachineOscEventType;
    
    public string oscAddressExpression = "/TimeMachine/<EventType>/<ClipIndex>_<SectionName>";
    public string oscStringValueExpression = "<SectionName>";
    public string globalOscAddress = "/TimeMachine/MoveTo";
    public TimeMachineTrackManager timeMachineTrackManager;
    [SerializeField]public List<TimeMachineOscEvent> timeMachineOscEvents = new List<TimeMachineOscEvent>();
    public bool initializeOnStart = false;
    public OSCReceiver oscReceiver;
    private void Start()
    {
        Init();
        Bind();
    }

    private void Update()
    {
        
    }
    
    public string ReplaceOscEventString(string expression, string eventType, int clipIndex,string sectionName)
    {

        var replaced = expression.Replace("<EventType>", eventType);
        replaced = replaced.Replace("<ClipIndex>", clipIndex.ToString());
        replaced = replaced.Replace("<SectionName>", sectionName);
        return replaced;
    }

    [ContextMenu("Init")]
    public void Init()
    {
        
        if(timeMachineTrackManager== null) return;
        timeMachineTrackManager.Init();
        timeMachineOscEvents.Clear();
        var clips = timeMachineTrackManager.clips;
        
        if(clips == null) return;
        foreach (var clip in clips)
        {
            var timeMachineControlClip = clip.asset as TimeMachineControlClip;
            if(timeMachineControlClip == null) continue;
            // Debug.Log($"{timeMachineControlClip.sectionName},{oscAddressExpression},{oscStringValueExpression}");
            var oscAddress = ReplaceOscEventString(oscAddressExpression, "MoveClip", timeMachineControlClip.clipIndex, timeMachineControlClip.sectionName);
            var oscValue = ReplaceOscEventString(oscStringValueExpression, "MoveClip", timeMachineControlClip.clipIndex, timeMachineControlClip.sectionName);
            
            Debug.Log(oscValue);
            timeMachineOscEvents.Add(new TimeMachineOscEvent()
            {
                // timeMachineEventType = TimeMachineEventType.MoveClip,
                oscAddress = oscAddress,
                clipIndex = timeMachineControlClip.clipIndex,
                clipName = clip.displayName,
                oscEventType = timeMachineOscEventType,
                oscValue = oscValue
            });
        }
        
        
        timeMachineOscEvents.Sort((a, b) => a.clipIndex.CompareTo(b.clipIndex));
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        #endif
    }

    [ContextMenu("Bind")]
    public void Bind()
    {

        if (timeMachineOscEventType == TimeMachineOscEventType.AddressOnly)
        {
           foreach (var timeMachineOscEvent in timeMachineOscEvents)
           {
          
               oscReceiver.Bind(timeMachineOscEvent.oscAddress, (message) =>
               {
                   var index = timeMachineOscEvent.clipIndex;
                   
                    timeMachineTrackManager.MoveClip(index);    
                   
               });
           } 
        }
        else if (timeMachineOscEventType == TimeMachineOscEventType.AddressAndValue)
        {
            oscReceiver.Bind(globalOscAddress, (message) =>
            {
            
                var value = message.FindValues(OSCValueType.String).First().Value as string;
            
                if (value != null)
                {
                    
                    foreach (var timeMachineOscEvent in timeMachineOscEvents)
                    {
                        if (value == timeMachineOscEvent.oscValue)
                        {
                            var index = timeMachineOscEvent.clipIndex;
                            
                                timeMachineTrackManager.MoveClip(index);
                            // }
                            
                        }
               
                    }   
                 
                }
            
            });
        }

    }
}
#endif