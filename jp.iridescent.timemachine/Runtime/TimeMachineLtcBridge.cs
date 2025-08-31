#if USE_LTC_DECODER
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Iridescent.TimeMachine
{
    [Serializable]
    public class LtcEventSettings
    {
        [Header("Event Enable Toggles")]
        [Tooltip("Enable processing when LTC start is received")]
        public bool enableOnLtcStart = true;
        
        [Tooltip("Enable processing when LTC stop is received")]
        public bool enableOnLtcStop = true;
        
        [Header("On LTC Start Actions")]
        [Tooltip("Mute TimeMachine clips when LTC starts")]
        public bool muteTimeMachineOnStart = true;
        
        [Tooltip("Ignore OSC commands when LTC starts")]
        public bool ignoreOscOnStart = true;
        
        [Header("On LTC Stop Actions")]
        [Tooltip("Unmute TimeMachine clips when LTC stops")]
        public bool unmuteTimeMachineOnStop = true;
        
        [Tooltip("Enable OSC commands when LTC stops")]
        public bool enableOscOnStop = true;
        
        [Tooltip("Update clips finish state based on current time when LTC stops")]
        public bool updateClipsStateOnStop = true;
    }

    /// <summary>
    /// Bridge component that connects LTC Decoder events with TimeMachine and OSC controls.
    /// This component acts as an intermediary, allowing flexible configuration of responses
    /// to LTC start/stop events.
    /// </summary>
    public class TimeMachineLtcBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] 
        [Tooltip("Reference to the TimeMachine Track Manager")]
        private TimeMachineTrackManager timeMachineTrackManager;
        
        [SerializeField]
        [Tooltip("Reference to the TimeMachine OSC Receiver")]
        private TimeMachineOscReceiver oscReceiver;
        
        [Header("LTC Timeline Sync")]
        [SerializeField]
        [Tooltip("Reference to LTC Timeline Sync component (auto-configured)")]
        private Component ltcTimelineSync;  // Component型で汎用的に参照
        
        // LTC Decoder参照を保持
        [SerializeField]
        [Tooltip("Reference to LTC Decoder component (auto-configured)")]
        private Component ltcDecoder;
        
        // リフレクション用のキャッシュ
        private Type ltcTimelineSyncType;
        private PropertyInfo playableDirectorProperty;
        private MethodInfo bindMethod;
        
        [Header("LTC Event Settings")]
        [SerializeField] 
        private LtcEventSettings ltcSettings = new LtcEventSettings();
        
        [Header("Custom Events")]
        [Space(10)]
        [SerializeField]
        [Tooltip("Additional events to invoke when LTC start is received")]
        private UnityEvent onLtcStartReceived;
        
        [SerializeField]
        [Tooltip("Additional events to invoke when LTC stop is received")]
        private UnityEvent onLtcStopReceived;
        
        [Header("Debug")]
        [SerializeField]
        [Tooltip("Enable debug logging")]
        private bool enableDebugLogging = true;

        /// <summary>
        /// Called when LTC decoder starts receiving timecode.
        /// Register this method to your LTC Decoder's OnStart event.
        /// </summary>
        public void OnLtcStartReceiving()
        {
            // Always log to confirm method is called
            Debug.Log("[TimeMachineLtcBridge] ========== OnLtcStartReceiving CALLED ==========");
            Debug.Log($"[TimeMachineLtcBridge] Settings - Enable: {ltcSettings.enableOnLtcStart}, Mute: {ltcSettings.muteTimeMachineOnStart}, IgnoreOSC: {ltcSettings.ignoreOscOnStart}");
            Debug.Log($"[TimeMachineLtcBridge] Components - TrackManager: {timeMachineTrackManager != null}, OSCReceiver: {oscReceiver != null}");
            
            if (!ltcSettings.enableOnLtcStart)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Start event ignored (disabled in settings)");
                return;
            }
            
            Debug.Log("[TimeMachineLtcBridge] Processing LTC Start event");
            
            // Mute TimeMachine clips
            if (ltcSettings.muteTimeMachineOnStart && timeMachineTrackManager != null)
            {
                Debug.Log("[TimeMachineLtcBridge] Calling MuteTimeMachineControlTrack()");
                timeMachineTrackManager.MuteTimeMachineControlTrack();
                Debug.Log("[TimeMachineLtcBridge] ✓ TimeMachine clips muted");
            }
            else if (ltcSettings.muteTimeMachineOnStart && timeMachineTrackManager == null)
            {
                Debug.LogError("[TimeMachineLtcBridge] Cannot mute - TimeMachineTrackManager is null!");
            }
            
            // Disable OSC commands
            if (ltcSettings.ignoreOscOnStart && oscReceiver != null)
            {
                Debug.Log("[TimeMachineLtcBridge] Calling DisableOscCommands()");
                oscReceiver.DisableOscCommands();
                Debug.Log("[TimeMachineLtcBridge] ✓ OSC commands disabled");
            }
            else if (ltcSettings.ignoreOscOnStart && oscReceiver == null)
            {
                Debug.LogError("[TimeMachineLtcBridge] Cannot disable OSC - OSCReceiver is null!");
            }
            
            // Invoke custom events
            onLtcStartReceived?.Invoke();
            Debug.Log("[TimeMachineLtcBridge] ========== OnLtcStartReceiving COMPLETED ==========");
        }
        
        /// <summary>
        /// Called when LTC decoder stops receiving timecode.
        /// Register this method to your LTC Decoder's OnStop event.
        /// </summary>
        public void OnLtcStopReceiving()
        {
            // Always log to confirm method is called
            Debug.Log("[TimeMachineLtcBridge] ========== OnLtcStopReceiving CALLED ==========");
            Debug.Log($"[TimeMachineLtcBridge] Settings - Enable: {ltcSettings.enableOnLtcStop}, Unmute: {ltcSettings.unmuteTimeMachineOnStop}, EnableOSC: {ltcSettings.enableOscOnStop}");
            Debug.Log($"[TimeMachineLtcBridge] Components - TrackManager: {timeMachineTrackManager != null}, OSCReceiver: {oscReceiver != null}");
            
            if (!ltcSettings.enableOnLtcStop)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Stop event ignored (disabled in settings)");
                return;
            }
            
            Debug.Log("[TimeMachineLtcBridge] Processing LTC Stop event");
            
            // Unmute TimeMachine clips
            if (ltcSettings.unmuteTimeMachineOnStop && timeMachineTrackManager != null)
            {
                Debug.Log("[TimeMachineLtcBridge] Calling UnMuteTimeMachineControlTrack()");
                timeMachineTrackManager.UnMuteTimeMachineControlTrack();
                Debug.Log("[TimeMachineLtcBridge] ✓ TimeMachine clips unmuted");
            }
            else if (ltcSettings.unmuteTimeMachineOnStop && timeMachineTrackManager == null)
            {
                Debug.LogError("[TimeMachineLtcBridge] Cannot unmute - TimeMachineTrackManager is null!");
            }
            
            // Enable OSC commands
            if (ltcSettings.enableOscOnStop && oscReceiver != null)
            {
                Debug.Log("[TimeMachineLtcBridge] Calling EnableOscCommands()");
                oscReceiver.EnableOscCommands();
                Debug.Log("[TimeMachineLtcBridge] ✓ OSC commands enabled");
            }
            else if (ltcSettings.enableOscOnStop && oscReceiver == null)
            {
                Debug.LogError("[TimeMachineLtcBridge] Cannot enable OSC - OSCReceiver is null!");
            }
            
            // Update clips finish state
            if (ltcSettings.updateClipsStateOnStop && timeMachineTrackManager != null)
            {
                Debug.Log("[TimeMachineLtcBridge] Calling UpdateClipsFinishStateByCurrentTime()");
                timeMachineTrackManager.UpdateClipsFinishStateByCurrentTime();
                Debug.Log("[TimeMachineLtcBridge] ✓ Clips finish state updated");
            }
            
            // Invoke custom events
            onLtcStopReceived?.Invoke();
            Debug.Log("[TimeMachineLtcBridge] ========== OnLtcStopReceiving COMPLETED ==========");
        }
        
        /// <summary>
        /// Manually trigger LTC start behavior (useful for testing)
        /// </summary>
        [ContextMenu("Simulate LTC Start")]
        public void SimulateLtcStart()
        {
            OnLtcStartReceiving();
        }
        
        /// <summary>
        /// Manually trigger LTC stop behavior (useful for testing)
        /// </summary>
        [ContextMenu("Simulate LTC Stop")]
        public void SimulateLtcStop()
        {
            OnLtcStopReceiving();
        }
        
        private void LogDebug(string message)
        {
            if (enableDebugLogging)
            {
                Debug.Log($"[TimeMachineLtcBridge] {message}");
            }
        }
        
#if UNITY_EDITOR
        private void Reset()
        {
            // Auto Setup is now done via button in Inspector
            Debug.Log("[TimeMachineLtcBridge] Component added. Use 'Auto Setup' button in Inspector to configure all components.");
        }
#endif
        
        private void Start()
        {
            Debug.Log("[TimeMachineLtcBridge] Start method called - Setting up event bindings");
            
            // Verify LTC Timeline Sync setup at runtime
            if (ltcTimelineSync == null)
            {
                LogDebug("LTC Timeline Sync reference is missing. Attempting to setup...");
                SetupLtcTimelineSyncOnSameGameObject();
            }
            else
            {
                LogDebug("LTC Timeline Sync is properly referenced");
            }
            
            // IMPORTANT: If ltcDecoder is null, try to find it as a fallback
            if (ltcDecoder == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Decoder reference is null in Start. Attempting to find it...");
                
                // Try to find LTC Decoder on the same GameObject or in the scene
                ltcDecoder = FindLtcDecoder();
                
                if (ltcDecoder != null)
                {
                    Debug.Log($"[TimeMachineLtcBridge] Found LTC Decoder at runtime: {ltcDecoder.name}");
#if UNITY_EDITOR
                    // Mark as dirty if we found it at runtime
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }
            }
            
            // Bind all LTC Decoder events
            if (ltcDecoder != null)
            {
                Debug.Log($"[TimeMachineLtcBridge] Binding LTC Decoder events in Start (Decoder: {ltcDecoder.name})");
                BindAllLtcDecoderEvents();
            }
            else
            {
                Debug.LogError("[TimeMachineLtcBridge] Could not find LTC Decoder. Events not bound.");
                Debug.LogError("[TimeMachineLtcBridge] Please ensure LTC Decoder package is installed and run Auto Setup.");
            }
            
            // Log current settings status
            LogCurrentStatus();
        }
        
        private void OnDestroy()
        {
            // Clean up event bindings
            if (ltcDecoder != null)
            {
                UnbindAllLtcDecoderEvents();
            }
        }
        
        /// <summary>
        /// Log current component status for debugging
        /// </summary>
        private void LogCurrentStatus()
        {
            Debug.Log($"[TimeMachineLtcBridge] Current Status:");
            Debug.Log($"  - TimeMachineTrackManager: {(timeMachineTrackManager != null ? "Set" : "Not Set")}");
            Debug.Log($"  - OSC Receiver: {(oscReceiver != null ? "Set" : "Not Set")}");
            Debug.Log($"  - LTC Timeline Sync: {(ltcTimelineSync != null ? "Set" : "Not Set")}");
            Debug.Log($"  - LTC Decoder: {(ltcDecoder != null ? "Set" : "Not Set")}");
            Debug.Log($"  - Enable on LTC Start: {ltcSettings.enableOnLtcStart}");
            Debug.Log($"  - Enable on LTC Stop: {ltcSettings.enableOnLtcStop}");
            Debug.Log($"  - Mute on Start: {ltcSettings.muteTimeMachineOnStart}");
            Debug.Log($"  - Ignore OSC on Start: {ltcSettings.ignoreOscOnStart}");
        }
        
        /// <summary>
        /// Main auto setup method - configures all components automatically
        /// </summary>
        public void SetupLtcComponents()
        {
            Debug.Log("[TimeMachineLtcBridge] Starting Auto Setup...");
            
            // Step 1: TimeMachineTrackManager setup
            SetupTimeMachineTrackManager();
            
            // Step 2: OSC Receiver setup
            SetupOscReceiver();
            
            // Step 3: LTC Decoder setup
            ltcDecoder = SetupLtcDecoder();  // Store the reference
            
            // Step 4: LTC Timeline Sync setup
            SetupLtcTimelineSync(ltcDecoder);
            
            // Step 5: Bind LTC Decoder events (if in Play Mode)
            if (ltcDecoder != null && Application.isPlaying)
            {
                BindLtcDecoderEvents(ltcDecoder);
            }
            else if (ltcDecoder != null)
            {
                Debug.Log("[TimeMachineLtcBridge] LTC Decoder found but not in Play Mode. Events will be bound at Start.");
            }
            
#if UNITY_EDITOR
            // Ensure the reference is saved
            if (ltcDecoder != null)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            
            Debug.Log("[TimeMachineLtcBridge] Auto Setup completed!");
        }
        
        /// <summary>
        /// Setup LTC Timeline Sync component on the same GameObject (deprecated - use SetupLtcComponents)
        /// </summary>
        private void SetupLtcTimelineSyncOnSameGameObject()
        {
            // Try to get LTC Timeline Sync type
            if (ltcTimelineSyncType == null)
            {
                // Search through all loaded assemblies for LTC Timeline Sync type
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Skip system assemblies for performance
                    if (assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("mscorlib"))
                        continue;
                    
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.Name == "LtcTimelineSync" || type.Name == "LTCTimelineSync")
                            {
                                ltcTimelineSyncType = type;
                                Debug.Log($"[TimeMachineLtcBridge] Found LTC Timeline Sync type: {type.FullName} in assembly {assembly.GetName().Name}");
                                break;
                            }
                        }
                        
                        if (ltcTimelineSyncType != null)
                            break;
                    }
                    catch (Exception)
                    {
                        // Skip assemblies that can't be loaded
                        continue;
                    }
                }
                
                if (ltcTimelineSyncType == null)
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] LTC Timeline Sync type not found. LTC Decoder package may not be properly installed.");
                    return;
                }
            }
            
            // Try to get LTC Timeline Sync from the same GameObject
            ltcTimelineSync = gameObject.GetComponent(ltcTimelineSyncType);
            
            // If not found, add it to the same GameObject
            if (ltcTimelineSync == null)
            {
                try
                {
                    ltcTimelineSync = gameObject.AddComponent(ltcTimelineSyncType);
                    Debug.Log($"[TimeMachineLtcBridge] Successfully added LTC Timeline Sync to {gameObject.name}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[TimeMachineLtcBridge] Failed to add LTC Timeline Sync: {e.Message}");
                    return;
                }
            }
            else
            {
                Debug.Log($"[TimeMachineLtcBridge] Found existing LTC Timeline Sync on {gameObject.name}");
            }
            
            // Configure PlayableDirector reference
            if (timeMachineTrackManager != null && timeMachineTrackManager.playableDirector != null)
            {
                SetLtcTimelineSyncDirector(timeMachineTrackManager.playableDirector);
            }
            
            // Execute Bind
            BindLtcTimelineSync();
        }
        
        /// <summary>
        /// Setup TimeMachineTrackManager reference
        /// </summary>
        private void SetupTimeMachineTrackManager()
        {
            // Try same GameObject first
            timeMachineTrackManager = GetComponent<TimeMachineTrackManager>();
            
            if (timeMachineTrackManager == null)
            {
                // Search in scene
                timeMachineTrackManager = FindObjectOfType<TimeMachineTrackManager>();
                if (timeMachineTrackManager != null)
                {
                    Debug.Log("[TimeMachineLtcBridge] Found TimeMachineTrackManager in scene");
                }
                else
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] TimeMachineTrackManager not found");
                }
            }
            else
            {
                Debug.Log("[TimeMachineLtcBridge] Found TimeMachineTrackManager on same GameObject");
            }
        }
        
        /// <summary>
        /// Setup OSC Receiver reference
        /// </summary>
        private void SetupOscReceiver()
        {
            // Try same GameObject first
            oscReceiver = GetComponent<TimeMachineOscReceiver>();
            
            if (oscReceiver == null)
            {
                // Search in scene
                oscReceiver = FindObjectOfType<TimeMachineOscReceiver>();
                if (oscReceiver != null)
                {
                    Debug.Log("[TimeMachineLtcBridge] Found OSC Receiver in scene");
                }
            }
            else
            {
                Debug.Log("[TimeMachineLtcBridge] Found OSC Receiver on same GameObject");
            }
        }
        
        /// <summary>
        /// Find LTC Decoder component in the scene
        /// </summary>
        private Component FindLtcDecoder()
        {
            // Find LTC Decoder type
            Type ltcDecoderType = FindTypeByName("LtcDecoder", "LTCDecoder");
            
            if (ltcDecoderType == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Decoder type not found");
                return null;
            }
            
            // Try to find in same GameObject first
            Component decoder = gameObject.GetComponent(ltcDecoderType);
            if (decoder != null)
            {
                return decoder;
            }
            
            // Try parent
            if (transform.parent != null)
            {
                decoder = transform.parent.GetComponent(ltcDecoderType);
                if (decoder != null)
                {
                    return decoder;
                }
            }
            
            // Try children
            decoder = GetComponentInChildren(ltcDecoderType) as Component;
            if (decoder != null)
            {
                return decoder;
            }
            
            // Finally, search in entire scene
            decoder = FindObjectOfType(ltcDecoderType) as Component;
            return decoder;
        }
        
        /// <summary>
        /// Setup LTC Decoder component
        /// </summary>
        private Component SetupLtcDecoder()
        {
            // Find LTC Decoder type
            Type ltcDecoderType = FindTypeByName("LtcDecoder", "LTCDecoder");
            
            if (ltcDecoderType == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Decoder type not found");
                return null;
            }
            
            // Search for existing LTC Decoder in scene
            Component ltcDecoder = FindObjectOfType(ltcDecoderType) as Component;
            
            if (ltcDecoder == null)
            {
                // Create new GameObject with LTC Decoder
                GameObject ltcDecoderObj = new GameObject("LTC Decoder (Auto-Created)");
                ltcDecoder = ltcDecoderObj.AddComponent(ltcDecoderType);
                Debug.Log("[TimeMachineLtcBridge] Created new LTC Decoder GameObject");
            }
            else
            {
                Debug.Log("[TimeMachineLtcBridge] Found existing LTC Decoder in scene");
            }
            
            return ltcDecoder;
        }
        
        /// <summary>
        /// Setup LTC Timeline Sync component
        /// </summary>
        private void SetupLtcTimelineSync(Component ltcDecoder)
        {
            // Find LTC Timeline Sync type
            if (ltcTimelineSyncType == null)
            {
                ltcTimelineSyncType = FindTypeByName("LtcTimelineSync", "LTCTimelineSync");
            }
            
            if (ltcTimelineSyncType == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Timeline Sync type not found");
                return;
            }
            
            // Search for existing LTC Timeline Sync in scene
            ltcTimelineSync = FindObjectOfType(ltcTimelineSyncType) as Component;
            
            if (ltcTimelineSync == null)
            {
                // Add to same GameObject
                ltcTimelineSync = gameObject.AddComponent(ltcTimelineSyncType);
                Debug.Log($"[TimeMachineLtcBridge] Added LTC Timeline Sync to {gameObject.name}");
            }
            else
            {
                Debug.Log("[TimeMachineLtcBridge] Found existing LTC Timeline Sync in scene");
            }
            
            // Set LTC Decoder reference
            if (ltcDecoder != null)
            {
                bool decoderSet = SetPropertyValue(ltcTimelineSync, ltcTimelineSyncType, 
                    new[] { "ltcDecoder", "decoder", "LtcDecoder", "_ltcDecoder" }, ltcDecoder);
                if (decoderSet)
                {
                    Debug.Log("[TimeMachineLtcBridge] Successfully set LTC Decoder reference in LTC Timeline Sync");
                }
                else
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] Could not set LTC Decoder reference in LTC Timeline Sync");
                }
            }
            
            // Set PlayableDirector reference
            if (timeMachineTrackManager?.playableDirector != null)
            {
                bool directorSet = SetPropertyValue(ltcTimelineSync, ltcTimelineSyncType,
                    new[] { "playableDirector", "director", "Director", "targetDirector", "_playableDirector" }, 
                    timeMachineTrackManager.playableDirector);
                if (directorSet)
                {
                    Debug.Log("[TimeMachineLtcBridge] Successfully set PlayableDirector reference in LTC Timeline Sync");
                }
                else
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] Could not set PlayableDirector reference in LTC Timeline Sync");
                }
            }
            
            // Call Bind method to initialize the LTC Timeline Sync
            bool bindResult = InvokeMethod(ltcTimelineSync, ltcTimelineSyncType, 
                new[] { "Bind", "BindTimeline", "Initialize", "Setup" });
            if (bindResult)
            {
                Debug.Log("[TimeMachineLtcBridge] Successfully initialized LTC Timeline Sync");
            }
        }
        
        /// <summary>
        /// Setup LTC Timeline Sync component (for backwards compatibility)
        /// </summary>
        public void SetupLtcTimelineSync()
        {
            SetupLtcTimelineSyncOnSameGameObject();
        }
        
        private void SetLtcTimelineSyncDirector(PlayableDirector director)
        {
            if (ltcTimelineSync == null || ltcTimelineSyncType == null) return;
            
            // Cache property info if not already cached
            if (playableDirectorProperty == null)
            {
                playableDirectorProperty = ltcTimelineSyncType.GetProperty("playableDirector")
                                        ?? ltcTimelineSyncType.GetProperty("director")
                                        ?? ltcTimelineSyncType.GetProperty("Director")
                                        ?? ltcTimelineSyncType.GetProperty("targetDirector");
            }
            
            if (playableDirectorProperty != null && playableDirectorProperty.CanWrite)
            {
                try
                {
                    playableDirectorProperty.SetValue(ltcTimelineSync, director);
                    LogDebug("Set PlayableDirector reference to LTC Timeline Sync");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[TimeMachineLtcBridge] Failed to set PlayableDirector: {e.Message}");
                }
            }
        }
        
        private void BindLtcTimelineSync()
        {
            if (ltcTimelineSync == null || ltcTimelineSyncType == null) return;
            
            // Cache bind method if not already cached
            if (bindMethod == null)
            {
                bindMethod = ltcTimelineSyncType.GetMethod("Bind")
                          ?? ltcTimelineSyncType.GetMethod("BindTimeline")
                          ?? ltcTimelineSyncType.GetMethod("Initialize");
            }
            
            if (bindMethod != null)
            {
                try
                {
                    bindMethod.Invoke(ltcTimelineSync, null);
                    LogDebug("LTC Timeline Sync binding completed");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[TimeMachineLtcBridge] Failed to bind LTC Timeline Sync: {e.Message}");
                }
            }
        }
        
        /// <summary>
        /// Bind all LTC Decoder events to TimeMachineLtcBridge methods
        /// </summary>
        private void BindAllLtcDecoderEvents()
        {
            if (ltcDecoder == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] Cannot bind events - LTC Decoder is null");
                return;
            }
            
            // First unbind any existing events to prevent duplicates
            UnbindAllLtcDecoderEvents();
            
            // Then bind all events
            BindLtcDecoderEvents(ltcDecoder);
        }
        
        /// <summary>
        /// Unbind all LTC Decoder events
        /// </summary>
        private void UnbindAllLtcDecoderEvents()
        {
            if (ltcDecoder == null) return;
            
            try
            {
                Type ltcDecoderType = ltcDecoder.GetType();
                
                // Unbind LTCStarted event
                var ltcStartedEvent = ltcDecoderType.GetEvent("LTCStarted", BindingFlags.Public | BindingFlags.Instance);
                if (ltcStartedEvent != null)
                {
                    var startDelegate = Delegate.CreateDelegate(typeof(System.Action), this, "OnLtcStartReceiving");
                    ltcStartedEvent.RemoveEventHandler(ltcDecoder, startDelegate);
                    Debug.Log("[TimeMachineLtcBridge] Unbound from LTCStarted event");
                }
                
                // Unbind LTCStopped event
                var ltcStoppedEvent = ltcDecoderType.GetEvent("LTCStopped", BindingFlags.Public | BindingFlags.Instance);
                if (ltcStoppedEvent != null)
                {
                    var stopDelegate = Delegate.CreateDelegate(typeof(System.Action), this, "OnLtcStopReceiving");
                    ltcStoppedEvent.RemoveEventHandler(ltcDecoder, stopDelegate);
                    Debug.Log("[TimeMachineLtcBridge] Unbound from LTCStopped event");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TimeMachineLtcBridge] Failed to unbind events: {e.Message}");
            }
        }
        
        /// <summary>
        /// Bind LTC Decoder events to TimeMachineLtcBridge methods
        /// </summary>
        private void BindLtcDecoderEvents(Component ltcDecoder)
        {
            if (ltcDecoder == null)
            {
                Debug.LogWarning("[TimeMachineLtcBridge] LTC Decoder is null, cannot bind events");
                return;
            }
            
            Type ltcDecoderType = ltcDecoder.GetType();
            Debug.Log($"[TimeMachineLtcBridge] Binding events for LTC Decoder type: {ltcDecoderType.Name}");
            
            try
            {
                // Use C# standard events (LTCStarted and LTCStopped)
                // These are System.Action events that can be accessed directly
                
                // Get LTCStarted event
                var ltcStartedEvent = ltcDecoderType.GetEvent("LTCStarted", BindingFlags.Public | BindingFlags.Instance);
                if (ltcStartedEvent != null)
                {
                    var startDelegate = Delegate.CreateDelegate(typeof(System.Action), this, "OnLtcStartReceiving");
                    ltcStartedEvent.AddEventHandler(ltcDecoder, startDelegate);
                    Debug.Log("[TimeMachineLtcBridge] Successfully bound to LTCStarted event");
                }
                else
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] LTCStarted event not found");
                }
                
                // Get LTCStopped event
                var ltcStoppedEvent = ltcDecoderType.GetEvent("LTCStopped", BindingFlags.Public | BindingFlags.Instance);
                if (ltcStoppedEvent != null)
                {
                    var stopDelegate = Delegate.CreateDelegate(typeof(System.Action), this, "OnLtcStopReceiving");
                    ltcStoppedEvent.AddEventHandler(ltcDecoder, stopDelegate);
                    Debug.Log("[TimeMachineLtcBridge] Successfully bound to LTCStopped event");
                }
                else
                {
                    Debug.LogWarning("[TimeMachineLtcBridge] LTCStopped event not found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TimeMachineLtcBridge] Failed to bind C# events: {e.Message}");
                
                // Fallback to UnityEvent binding if C# events fail
                Debug.Log("[TimeMachineLtcBridge] Attempting fallback to UnityEvent binding...");
                bool startBound = BindUnityEvent(ltcDecoder, ltcDecoderType, "onLTCStarted", "OnLtcStartReceiving");
                bool stopBound = BindUnityEvent(ltcDecoder, ltcDecoderType, "onLTCStopped", "OnLtcStopReceiving");
                
                if (!startBound || !stopBound)
                {
                    Debug.LogError("[TimeMachineLtcBridge] Failed to bind events using both methods");
                }
            }
        }
        
        /// <summary>
        /// Helper method to bind UnityEvent (without parameters)
        /// </summary>
        private bool BindUnityEvent(Component ltcDecoder, Type ltcDecoderType, string eventFieldName, string methodName)
        {
            try
            {
                // Get the event field
                var eventField = ltcDecoderType.GetField(eventFieldName, BindingFlags.Public | BindingFlags.Instance);
                if (eventField == null)
                {
                    Debug.LogWarning($"[TimeMachineLtcBridge] Event field {eventFieldName} not found");
                    return false;
                }
                
                // Get the event object
                var eventObj = eventField.GetValue(ltcDecoder);
                if (eventObj == null)
                {
                    Debug.LogWarning($"[TimeMachineLtcBridge] Event object {eventFieldName} is null");
                    return false;
                }
                
                // Get AddListener method
                var eventType = eventObj.GetType();
                var addListenerMethod = eventType.GetMethod("AddListener");
                if (addListenerMethod == null)
                {
                    Debug.LogWarning($"[TimeMachineLtcBridge] AddListener method not found for {eventFieldName}");
                    return false;
                }
                
                // Create UnityAction delegate
                var targetMethod = this.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (targetMethod != null)
                {
                    var delegateInstance = Delegate.CreateDelegate(typeof(UnityAction), this, targetMethod);
                    addListenerMethod.Invoke(eventObj, new object[] { delegateInstance });
                    Debug.Log($"[TimeMachineLtcBridge] Successfully bound {methodName} to {eventFieldName}");
                    return true;
                }
                else
                {
                    Debug.LogError($"[TimeMachineLtcBridge] Method {methodName} not found");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TimeMachineLtcBridge] Exception binding {eventFieldName}: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// Get the configured LTC Timeline Sync component
        /// </summary>
        public Component GetLtcTimelineSync()
        {
            return ltcTimelineSync;
        }
        
        /// <summary>
        /// Check if LTC Timeline Sync is properly configured
        /// </summary>
        public bool IsLtcTimelineSyncConfigured()
        {
            return ltcTimelineSync != null;
        }
        
        /// <summary>
        /// Helper method to find type by name variations
        /// </summary>
        private Type FindTypeByName(params string[] typeNames)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                // Skip system assemblies for performance
                if (assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("mscorlib"))
                    continue;
                
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var typeName in typeNames)
                        {
                            if (type.Name == typeName)
                            {
                                Debug.Log($"[TimeMachineLtcBridge] Found type: {type.FullName} in assembly {assembly.GetName().Name}");
                                return type;
                            }
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that can't be loaded
                    continue;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Helper method to set property value via reflection
        /// </summary>
        private bool SetPropertyValue(Component target, Type targetType, string[] propertyNames, object value)
        {
            foreach (var propName in propertyNames)
            {
                var property = targetType.GetProperty(propName, 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        property.SetValue(target, value);
                        Debug.Log($"[TimeMachineLtcBridge] Set {propName} property");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[TimeMachineLtcBridge] Failed to set {propName}: {e.Message}");
                    }
                }
                
                // Try field if property doesn't exist
                var field = targetType.GetField(propName, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (field != null)
                {
                    try
                    {
                        field.SetValue(target, value);
                        Debug.Log($"[TimeMachineLtcBridge] Set {propName} field");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[TimeMachineLtcBridge] Failed to set {propName}: {e.Message}");
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Helper method to invoke method via reflection
        /// </summary>
        private bool InvokeMethod(Component target, Type targetType, string[] methodNames)
        {
            foreach (var methodName in methodNames)
            {
                var method = targetType.GetMethod(methodName, 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (method != null)
                {
                    try
                    {
                        method.Invoke(target, null);
                        Debug.Log($"[TimeMachineLtcBridge] Called {methodName} method");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[TimeMachineLtcBridge] Failed to call {methodName}: {e.Message}");
                    }
                }
            }
            return false;
        }
        
        private void OnValidate()
        {
            // Validate references and provide warnings if needed
            if (timeMachineTrackManager == null)
            {
                Debug.LogWarning($"[TimeMachineLtcBridge] TimeMachineTrackManager reference is not set on {gameObject.name}");
            }
            
            if (oscReceiver == null && (ltcSettings.ignoreOscOnStart || ltcSettings.enableOscOnStop))
            {
                Debug.LogWarning($"[TimeMachineLtcBridge] OSC Receiver reference is not set but OSC actions are enabled on {gameObject.name}");
            }
            
            // Don't warn about LTC Timeline Sync during initial setup
            // It will be configured automatically via Reset/Start
#if UNITY_EDITOR
            if (ltcTimelineSync == null && Application.isPlaying)
            {
                Debug.LogWarning($"[TimeMachineLtcBridge] LTC Timeline Sync is not configured on {gameObject.name}");
            }
#endif
        }
    }
}
#else
// Stub class when LTC Decoder is not installed
namespace Iridescent.TimeMachine
{
    /// <summary>
    /// Stub class displayed when LTC Decoder package is not installed
    /// </summary>
    public class TimeMachineLtcBridge : UnityEngine.MonoBehaviour
    {
        private void OnValidate()
        {
            UnityEngine.Debug.LogWarning("[TimeMachineLtcBridge] This component requires the LTC Decoder package (jp.iridescent.ltcdecoder) to be installed.");
        }
        
        private void Start()
        {
            UnityEngine.Debug.LogWarning($"[TimeMachineLtcBridge] The LTC Decoder package is not installed. This component on '{gameObject.name}' will not function.");
        }
    }
}
#endif