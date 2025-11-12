using MelonLoader;
using MelonLoader.Utils;
using Sirenix.Utilities;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModManager
{
    public static class ModListManager
    {
        public static void CreateListLayout(RectTransform baseRect, TextMeshProUGUI referenceText)
        {
            var scrollObj = new GameObject("ScrollArea", typeof(RectTransform), typeof(ScrollRect));
            scrollObj.transform.SetParent(baseRect, false);

            var scrollRT = scrollObj.GetComponent<RectTransform>();
            scrollRT.anchorMin = Vector2.zero;
            scrollRT.anchorMax = Vector2.one;
            scrollRT.offsetMin = Vector2.zero;
            scrollRT.offsetMax = Vector2.zero;

            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(scrollObj.transform, false);

            var contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 0f);
            contentRT.anchorMax = new Vector2(1f, 1f);
            contentRT.pivot = new Vector2(0.5f, 1f);
            contentRT.offsetMin = Vector2.zero;
            contentRT.offsetMax = Vector2.zero;
            contentRT.anchoredPosition = Vector2.zero;

            var vLayout = content.GetComponent<VerticalLayoutGroup>();
            vLayout.spacing = 6f;
            vLayout.padding = new RectOffset(12, 12, 12, 12);
            vLayout.childAlignment = TextAnchor.UpperLeft;
            vLayout.childForceExpandWidth = true;

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.vertical = true;
            scroll.horizontal = false;
            scroll.viewport = baseRect;
            scroll.content = contentRT;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            PopulateModList(content.transform, referenceText);
        }

        public static void PopulateModList(Transform content, TextMeshProUGUI refText)
        {
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "..", "Mods");
            Directory.CreateDirectory(path);

            var activeMods = MelonTypeBase<MelonMod>.RegisteredMelons.Where(m => m.GetType() != typeof(Core)).ToArray();

            foreach (var mod in activeMods)
            {
                CreateModItem(content, refText, mod.Info.Name, mod.Info.Version, mod.Info.Author, mod.MelonAssembly.Location, false);
            }

            foreach (var file in Directory.GetFiles(path, "*.disabled"))
            {
                string real = file.Replace(".disabled", "");
                if (!File.Exists(real))
                {
                    string modName = Path.GetFileNameWithoutExtension(real);
                    CreateModItem(content, refText, modName, "", "", real, true);
                }
            }
        }

        private static void CreateModItem(Transform content, TextMeshProUGUI refText, string name, string version, string author, string path, bool disabled)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(HorizontalLayoutGroup));
            item.transform.SetParent(content, false);

            var itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 1);
            itemRT.anchorMax = new Vector2(1, 1);
            itemRT.pivot = new Vector2(0, 1f);

            var layout = item.GetComponent<LayoutElement>();
            layout.preferredHeight = 26f;

            var hLayout = item.GetComponent<HorizontalLayoutGroup>();
            hLayout.spacing = 6f;
            hLayout.childAlignment = TextAnchor.MiddleLeft;
            hLayout.childControlWidth = false;
            hLayout.childControlHeight = false;
            hLayout.childForceExpandWidth = false;
            hLayout.padding = new RectOffset(4, 4, 2, 2);

            // --- TOGGLE ---
            if (!name.Contains("ModManager"))
            {
                string disabledPath = path + ".disabled";
                var toggleObj = new GameObject("Toggle", typeof(RectTransform), typeof(Toggle));
                toggleObj.transform.SetParent(item.transform, false);
                var tRT = toggleObj.GetComponent<RectTransform>();
                tRT.sizeDelta = new Vector2(18f, 18f);

                // Background
                var bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
                bgObj.transform.SetParent(toggleObj.transform, false);
                var bgImg = bgObj.GetComponent<Image>();
                bgImg.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                bgImg.rectTransform.anchorMin = Vector2.zero;
                bgImg.rectTransform.anchorMax = Vector2.one;
                bgImg.rectTransform.offsetMin = Vector2.zero;
                bgImg.rectTransform.offsetMax = Vector2.zero;

                // Checkmark
                var checkObj = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
                checkObj.transform.SetParent(bgObj.transform, false);
                var checkImg = checkObj.GetComponent<Image>();
                checkImg.color = Color.white;
                checkImg.rectTransform.anchorMin = new Vector2(0.25f, 0.25f);
                checkImg.rectTransform.anchorMax = new Vector2(0.75f, 0.75f);
                checkImg.rectTransform.offsetMin = Vector2.zero;
                checkImg.rectTransform.offsetMax = Vector2.zero;

                var toggle = toggleObj.GetComponent<Toggle>();
                toggle.targetGraphic = bgImg;
                toggle.graphic = checkImg;
                toggle.isOn = !disabled;
                checkImg.enabled = toggle.isOn;

                toggle.onValueChanged.AddListener(state =>
                {
                    try
                    {
                        if (state && File.Exists(disabledPath))
                            File.Move(disabledPath, path);
                        else if (!state && File.Exists(path))
                            File.Move(path, disabledPath);

                        checkImg.enabled = state;
                        if (Core.modListUI?.restartLabel != null)
                            Core.modListUI.restartLabel.SetActive(true);

                        MelonLogger.Msg($"{(state ? "Enabled" : "Disabled")} {name}, restart required.");
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Error toggling {name}: {ex.Message}");
                    }
                });
            }

            // --- LABEL ---
            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(item.transform, false);

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            if (refText != null)
            {
                label.font = refText.font;
                label.fontSharedMaterial = refText.fontSharedMaterial;
            }

            string text = $"<b>{name}</b>";
            if (!version.IsNullOrWhitespace())
                text += $"  <color=#B0B0B0>V{version}</color>";
            if (!author.IsNullOrWhitespace())
                text += $"  <size=90%><color=#888888>by {author}</color></size>";

            label.text = text;
            label.richText = true;
            label.fontSize = 16f;
            label.alignment = TextAlignmentOptions.MidlineLeft;

            var labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.flexibleWidth = 1f;
        }


        public static void OpenModsFolder()
        {
            string dir = Path.Combine(MelonEnvironment.UserDataDirectory, "..", "Mods");
            MelonLogger.Msg($"Mod folder is: {dir}");
            Directory.CreateDirectory(dir);
            Process.Start("explorer.exe", dir);
        }

        public static void RestartGame()
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(new ProcessStartInfo { FileName = exe, UseShellExecute = true });
            Application.Quit();
            Process.GetCurrentProcess().Kill();
        }
    }
}
