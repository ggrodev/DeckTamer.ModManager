using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModManager.UI
{
    public class ModListUI
    {
        private TextMeshProUGUI referenceText;
        private GameObject uiRoot;

        public ModListUI() { }

        Canvas Canvas;
        GameObject buttonContainer;

        public void Initialize()
        {
            GameObject canvasObj = new GameObject("ModCanvas");
            Canvas = canvasObj.AddComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            MelonLogger.Msg("Mod Manager Loading...");

            CreateUI();
        }

        private void CreateUI()
        {
            //Create root panel
            uiRoot = new GameObject("ModManagerUI", typeof(RectTransform), typeof(Image));
            uiRoot.transform.SetParent(Canvas.transform, false);

            RectTransform prt = uiRoot.GetComponent<RectTransform>();
            prt.anchorMin = new Vector2(0f, 1f);
            prt.anchorMax = new Vector2(0f, 1f);
            prt.pivot = new Vector2(0f, 1f);
            prt.sizeDelta = new Vector2(350f, 800f);
            prt.anchoredPosition = new Vector2(30f, -30f);

            // Background
            var bgImg = uiRoot.GetComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, .4f);

            ModListManager.CreateListLayout(prt, referenceText);

            //Bottom Bar of buttons
            buttonContainer = new GameObject("ModButtonContainer", typeof(RectTransform));
            buttonContainer.transform.SetParent(prt, false);

            RectTransform rt = buttonContainer.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 80f);

            CreateButton(buttonContainer.transform, "Open Mods Folder", -80f, () => ModListManager.OpenModsFolder());
            CreateButton(buttonContainer.transform, "Restart Game", 80f, () => ModListManager.RestartGame());
            CreateLabel(buttonContainer.transform, "<color=#FFFF00>Restart the game to apply changes</color>", new Vector2(0, -40f));
        }

        private GameObject CreateButton(Transform parent, string text, float xOffset, UnityAction onClick)
        {
            GameObject buttonObj = new GameObject(text.Replace(" ", "") + "Button", typeof(RectTransform), typeof(Button), typeof(Image));
            buttonObj.transform.SetParent(parent, false);

            RectTransform rt = buttonObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150, 30f);
            rt.anchoredPosition = new Vector2(xOffset, 0f);

            Image img = buttonObj.GetComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.25f, 1f);

            TextMeshProUGUI textMesh = new GameObject("ButtonText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            textMesh.transform.SetParent(buttonObj.transform, false);
            textMesh.text = text;
            textMesh.fontSize = 16f;
            textMesh.alignment = TextAlignmentOptions.Center;
            if (referenceText != null)
            {
                textMesh.font = referenceText.font;
                textMesh.fontSharedMaterial = referenceText.fontSharedMaterial;
            }

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(onClick);
            return buttonObj;
        }

        public GameObject restartLabel;
        private void CreateLabel(Transform parent, string text, Vector2 offset)
        {
            restartLabel = new GameObject("WarningLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            restartLabel.transform.SetParent(parent, false);
            restartLabel.SetActive(false);

            RectTransform rt = restartLabel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600f, 50f);
            rt.anchoredPosition = offset;

            var tmp = restartLabel.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20f;
            tmp.alignment = TextAlignmentOptions.Center;
            if (referenceText != null)
            {
                tmp.font = referenceText.font;
                tmp.fontSharedMaterial = referenceText.fontSharedMaterial;
            }
        }

        public void ToggleUI()
        {
            if (uiRoot == null)
                return;

            uiRoot.SetActive(!uiRoot.activeSelf);
            buttonContainer.SetActive(!buttonContainer.activeSelf);
        }
    }
}
