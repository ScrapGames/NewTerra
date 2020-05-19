using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Buildings
{
    public class UI_PreviewBase : MonoBehaviour
    {
        private const int MAX_COMPONENTS = 6;
        private const float ANIMATION_DELAY = 0.145f;
        [SerializeField] protected TextMeshProUGUI text_Name;
        [SerializeField] protected TextMeshProUGUI descriptionText;

        [SerializeField] protected Transform componentPanel;
        [SerializeField] protected Image previewImage;
        private UI_ComponentIcon[] components;

        public void InitComponents()
        {
            components = new UI_ComponentIcon[MAX_COMPONENTS];
            for (int i = 0; i < MAX_COMPONENTS; i++)
            {
                UI_ComponentIcon prefab = GameManager.Instance.gameSettings.prefab_UI_ComponentIcon;
                components[i] = GameObject.Instantiate(prefab, componentPanel);
                components[i].gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        protected void ShowRequiredComponents(BuildingData data)
        {
            int nextComponentID = 0;

            // Add components
            for (int i = 0; i < data.requiredComponentsToBuild.Length; i++)
            {
                UI_ComponentIcon icon = components[nextComponentID];
                icon.gameObject.SetActive(true);
                icon.SetData(data.requiredComponentsToBuild[i]);
                icon.Animate(nextComponentID * ANIMATION_DELAY);
                nextComponentID++;
            }

            // Add materials
            for (int i = 0; i < data.requiredMaterialsToBuild.Length; i++)
            {
                UI_ComponentIcon icon = components[nextComponentID];
                icon.gameObject.SetActive(true);
                icon.SetData(data.requiredMaterialsToBuild[i]);
                icon.Animate(nextComponentID * ANIMATION_DELAY);
                nextComponentID++;
            }

            // Disable remaining icons
            for (int i = nextComponentID; i < MAX_COMPONENTS; i++)
            {
                components[i].gameObject.SetActive(false);
            }
        }

        public void ShowPanel(bool show)
        {
            gameObject.SetActive(show);
            descriptionText.transform.parent.gameObject.SetActive(show);
        }

        public virtual void ShowPreview(BuildingData data)
        {
            // Show Icon
            GameManager.Instance.BuildingPreview?.SetPreview(data.nameID);

            // Show Name
            text_Name.text = data.LocalizedName;

            // Show Description
            descriptionText.text = data.LocalizedDescription;

            // Show build components
            //ShowRequiredComponents(data);

            // Punch element
            transform.DOKill(true);
            transform.DOPunchScale(Vector2.one * 0.1f, 0.2f, 3, 1f);
        }
    }
}