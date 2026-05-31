using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Core
{
    public static class LogDefineSymbolToggleTool
    {
        private const string DEFINE_SYMBOL = "LOGS_ENABLE";
        private const string MENU_ITEM_PATH = "Tools/Core/Enable Logs";

        [MenuItem(MENU_ITEM_PATH)]
        public static void ToggleLogs()
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            PlayerSettings.GetScriptingDefineSymbols(target, out string[] symbols);

            var list = symbols.ToList();

            if (list.Contains(DEFINE_SYMBOL))
            {
                list.Remove(DEFINE_SYMBOL);
            }
            else
            {
                list.Add(DEFINE_SYMBOL);
            }

            PlayerSettings.SetScriptingDefineSymbols(target, list.ToArray());
        }

        [MenuItem(MENU_ITEM_PATH, true)]
        public static bool ToggleLogsValidate()
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            PlayerSettings.GetScriptingDefineSymbols(target, out string[] symbols);

            Menu.SetChecked(MENU_ITEM_PATH, symbols.Contains(DEFINE_SYMBOL));
            return true;
        }
    }
}