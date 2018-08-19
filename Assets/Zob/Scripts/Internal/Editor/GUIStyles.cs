using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class GUIStyles
    {
        public GUIStyle SelectableLabel { get; private set; }
        public GUIStyle OddBackground { get; private set; }
        public GUIStyle EvenBackground { get; private set; }

        public GUIStyles()
        {
            // these styles are internal unity editor
            // check unity csharp source code
            SelectableLabel = new GUIStyle("CN Message");
            OddBackground = new GUIStyle("CN EntryBackodd");
            EvenBackground = new GUIStyle("CN EntryBackEven");
        }
    }
}