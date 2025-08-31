#if USE_LTC_DECODER
using UnityEngine;
using UnityEditor;
using Iridescent.TimeMachine;

namespace Iridescent.TimeMachineEditor
{
    [CustomEditor(typeof(TimeMachineLtcBridge))]
    public class TimeMachineLtcBridgeEditor : Editor
    {
        private SerializedProperty timeMachineTrackManager;
        private SerializedProperty oscReceiver;
        private SerializedProperty ltcTimelineSync;
        private SerializedProperty ltcDecoder;  // Add ltcDecoder property
        private SerializedProperty ltcSettings;
        private SerializedProperty onLtcStartReceived;
        private SerializedProperty onLtcStopReceived;
        private SerializedProperty enableDebugLogging;
        
        private bool showReferences = true;
        private bool showLtcSettings = true;
        private bool showCustomEvents = false;
        private bool showDebug = false;
        
        private void OnEnable()
        {
            timeMachineTrackManager = serializedObject.FindProperty("timeMachineTrackManager");
            oscReceiver = serializedObject.FindProperty("oscReceiver");
            ltcTimelineSync = serializedObject.FindProperty("ltcTimelineSync");
            ltcDecoder = serializedObject.FindProperty("ltcDecoder");  // Find ltcDecoder property
            ltcSettings = serializedObject.FindProperty("ltcSettings");
            onLtcStartReceived = serializedObject.FindProperty("onLtcStartReceived");
            onLtcStopReceived = serializedObject.FindProperty("onLtcStopReceived");
            enableDebugLogging = serializedObject.FindProperty("enableDebugLogging");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var bridgeComponent = target as TimeMachineLtcBridge;
            
            // Check if setup is needed using serialized properties
            bool needsSetup = timeMachineTrackManager.objectReferenceValue == null || 
                             ltcTimelineSync.objectReferenceValue == null;
            
            if (needsSetup)
            {
                // Show Auto Setup button prominently
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(
                    "TimeMachine LTC Bridge needs to be configured.\n" +
                    "Click the Auto Setup button to automatically configure all components.",
                    MessageType.Warning
                );
                
                GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
                if (GUILayout.Button("Auto Setup", GUILayout.Height(40)))
                {
                    // Apply any pending changes before setup
                    serializedObject.ApplyModifiedProperties();
                    
                    // Run the setup
                    bridgeComponent.SetupLtcComponents();
                    
                    // Mark the object as dirty and re-fetch the serialized object
                    EditorUtility.SetDirty(bridgeComponent);
                    
                    // Re-fetch the serialized object to get updated values
                    serializedObject.Update();
                    
                    // Re-cache the properties
                    ltcDecoder = serializedObject.FindProperty("ltcDecoder");
                    ltcTimelineSync = serializedObject.FindProperty("ltcTimelineSync");
                    timeMachineTrackManager = serializedObject.FindProperty("timeMachineTrackManager");
                    oscReceiver = serializedObject.FindProperty("oscReceiver");
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.Space(10);
            }
            else
            {
                // Header with help box
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    "This component bridges LTC Decoder events with TimeMachine and OSC controls.\n" +
                    "✓ All components are configured and ready.",
                    MessageType.Info
                );
                EditorGUILayout.Space(10);
            }
            
            // References Section
            showReferences = EditorGUILayout.BeginFoldoutHeaderGroup(showReferences, "References");
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(timeMachineTrackManager);
                EditorGUILayout.PropertyField(oscReceiver);
                EditorGUILayout.PropertyField(ltcDecoder, new GUIContent("LTC Decoder", "Reference to the LTC Decoder component"));
                
                // Validation warnings
                if (timeMachineTrackManager.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("TimeMachineTrackManager is required for mute/unmute functionality", MessageType.Warning);
                }
                
                if (oscReceiver.objectReferenceValue == null)
                {
                    var settings = ltcSettings.FindPropertyRelative("ignoreOscOnStart");
                    var settings2 = ltcSettings.FindPropertyRelative("enableOscOnStop");
                    if (settings.boolValue || settings2.boolValue)
                    {
                        EditorGUILayout.HelpBox("OSC Receiver is required for OSC control functionality", MessageType.Warning);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space(5);
            
            // LTC Timeline Sync Section
            EditorGUILayout.LabelField("LTC Timeline Sync", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(ltcTimelineSync, new GUIContent("LTC Timeline Sync"));
            
            if (ltcTimelineSync.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("✓ LTC Timeline Sync is connected and configured automatically", MessageType.Info);
            }
            
            // Show LTC Decoder status
            if (ltcDecoder.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("✓ LTC Decoder is connected and will bind events at runtime", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("LTC Timeline Sync will be automatically configured when entering Play Mode", MessageType.Info);
            }
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(5);
            
            // LTC Settings Section
            showLtcSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLtcSettings, "LTC Event Settings");
            if (showLtcSettings)
            {
                EditorGUI.indentLevel++;
                
                // Enable toggles
                EditorGUILayout.LabelField("Master Toggles", EditorStyles.boldLabel);
                var enableOnStart = ltcSettings.FindPropertyRelative("enableOnLtcStart");
                var enableOnStop = ltcSettings.FindPropertyRelative("enableOnLtcStop");
                
                EditorGUILayout.PropertyField(enableOnStart);
                EditorGUILayout.PropertyField(enableOnStop);
                
                EditorGUILayout.Space(10);
                
                // On Start Actions
                GUI.enabled = enableOnStart.boolValue;
                EditorGUILayout.LabelField("On LTC Start Actions", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ltcSettings.FindPropertyRelative("muteTimeMachineOnStart"));
                EditorGUILayout.PropertyField(ltcSettings.FindPropertyRelative("ignoreOscOnStart"));
                GUI.enabled = true;
                
                EditorGUILayout.Space(10);
                
                // On Stop Actions
                GUI.enabled = enableOnStop.boolValue;
                EditorGUILayout.LabelField("On LTC Stop Actions", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ltcSettings.FindPropertyRelative("unmuteTimeMachineOnStop"));
                EditorGUILayout.PropertyField(ltcSettings.FindPropertyRelative("enableOscOnStop"));
                EditorGUILayout.PropertyField(ltcSettings.FindPropertyRelative("updateClipsStateOnStop"));
                GUI.enabled = true;
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space(5);
            
            // Custom Events Section
            showCustomEvents = EditorGUILayout.BeginFoldoutHeaderGroup(showCustomEvents, "Custom Events");
            if (showCustomEvents)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(onLtcStartReceived);
                EditorGUILayout.PropertyField(onLtcStopReceived);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space(5);
            
            // Debug Section
            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(enableDebugLogging);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Test Controls (only in play mode)
            if (Application.isPlaying)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Test Controls", EditorStyles.boldLabel);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Simulate LTC Start", GUILayout.Height(30)))
                    {
                        var bridge = target as TimeMachineLtcBridge;
                        bridge.OnLtcStartReceiving();
                    }
                    
                    if (GUILayout.Button("Simulate LTC Stop", GUILayout.Height(30)))
                    {
                        var bridge = target as TimeMachineLtcBridge;
                        bridge.OnLtcStopReceiving();
                    }
                }
                
                EditorGUILayout.HelpBox("Test buttons are available during Play Mode", MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Enter Play Mode to access test controls", MessageType.Info);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif