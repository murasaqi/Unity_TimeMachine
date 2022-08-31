using UnityEditor;
using UnityEngine.UIElements;

namespace Iridescent.TimeMachine
{
    [CustomEditor(typeof(TimeMachineTrack))]
    public class TimeMachineControlTrackEditor: Editor
    {
        // OnInspectorGUIではなくCreateInspectorGUIを使う
        public override VisualElement CreateInspectorGUI()
        {
            TimeMachineTrack track = target as TimeMachineTrack;
            // Inspector拡張の場合、VisualElementはnewする
            VisualElement root = new VisualElement();

            // デフォルトのInspector表示を追加
            IMGUIContainer defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
            root.Add(defaultInspector);

            Button button = new Button();
            button.text = "Initialize clips";

            button.clicked += () =>
            {
                track.Initialize();
            };
            root.Add(button);
            return root;
        }
    }
}
