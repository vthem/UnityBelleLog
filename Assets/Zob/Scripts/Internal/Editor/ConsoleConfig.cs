using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class ConsoleConfig : ScriptableObject
    {
        [SerializeField]
        private float _logEntryArrayRowHeight = 20f;
        public float LogEntryArrayRowHeight { get { return _logEntryArrayRowHeight; } set { _logEntryArrayRowHeight = value; } }

        public static ConsoleConfig Load()
        {
            var config = Resources.Load<ConsoleConfig>("ConsoleConfig.asset");
            if (null == config)
            {
                config = CreateConfig();
            }
            return config;
        }

        private static ConsoleConfig CreateConfig()
        {
            var config = ScriptableObject.CreateInstance<ConsoleConfig>();
            AssetDatabase.CreateAsset(config, "Assets/Zob/Resources/ConsoleConfig.asset");
            AssetDatabase.SaveAssets();
            return config;
        }
    }
}