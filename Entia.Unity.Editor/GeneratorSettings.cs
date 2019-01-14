using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Entia/Generator Settings")]
    public class GeneratorSettings : ScriptableObject
    {
        public static GeneratorSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.FindAssets($"t:{nameof(GeneratorSettings)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<GeneratorSettings>)
                        .FirstOrDefault(settings => settings != null);
                }

                return _instance;
            }
        }
        static GeneratorSettings _instance;

        public string Tool => PathUtility.Replace(_tool);
        public bool Automatic => _automatic;
        public bool Debug => _debug;
        public float Timeout => _timeout;
        public string Log => _log;
        public string[] Inputs => Array.ConvertAll(_inputs, PathUtility.Replace);
        public string Output => PathUtility.Replace(_output);
        public string[] Assemblies => Array.ConvertAll(_assemblies, PathUtility.Replace);
        public string Suffix => _suffix;

        [SerializeField]
        string _tool = "|Entia.Unity.Generate.exe|";
        [SerializeField]
        bool _automatic = true;
        [SerializeField]
        bool _debug = false;
        [SerializeField]
        float _timeout = 5f;
        [SerializeField]
        string[] _inputs = { Path.Combine("{Assets}", "Scripts") };
        [SerializeField]
        string _output = Path.Combine("{Assets}", "Generated");
        [SerializeField]
        string[] _assemblies = { "{Editor.Mono}", "{Editor.Managed}", "{Editor.Extensions}" };
        [SerializeField]
        string _suffix = "Generated";
        [SerializeField]
        string _log = "Log.txt";
    }
}
