using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(GeneratorSettings), isFallback = true)]
    public class GeneratorSettingsEditor : UnityEditor.Editor
    {
        public GeneratorSettings Target => target as GeneratorSettings;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tool = Generator.Tool(Target);
            var alive = Generator.TryProcess(tool, out var process);

            EditorGUILayout.Separator();

            if (alive)
            {
                EditorGUILayout.HelpBox(
$@"Generator process:
-> Id: {process.Id}
-> Name: {process.ProcessName}
-> Start time: {process.StartTime}
-> Process time: {process.TotalProcessorTime}",
                    MessageType.Info);
            }
            else EditorGUILayout.HelpBox("Generator process was not found.", MessageType.Warning);

            using (LayoutUtility.Disable(alive))
                if (GUILayout.Button("Birth")) Generator.Birth(tool, Target, true);

            using (LayoutUtility.Disable(!alive))
            {
                if (GUILayout.Button("Generate")) Generator.Generate(Target, true);
                if (GUILayout.Button("Kill")) Generator.Kill(tool, true);
            }
        }
    }
}