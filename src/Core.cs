using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(ModManager.Core), "ModManager", "1.0.0", "GGro", null)]
[assembly: MelonGame("Horizon Edge", "Decktamer")]

namespace ModManager
{
    public class Core : MelonMod
    {
        public static UI.ModListUI modListUI;
        private bool initialized = false;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!initialized)
            {
                CreateModListUI();
            }
        }

        private void CreateModListUI()
        {
            modListUI = new UI.ModListUI();
            modListUI.Initialize();
            modListUI.ToggleUI();

            initialized = true;
        }

        public override void OnUpdate()
        {
            if (!initialized || modListUI == null)
                return;

            // Toggle UI on F1
            if (Input.GetKeyDown(KeyCode.F1))
            {
                modListUI.ToggleUI();
            }
        }
    }
}
