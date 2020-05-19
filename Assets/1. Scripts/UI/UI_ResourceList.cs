using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace CraftingResources
{
    public class UI_ResourceList : MonoBehaviour
    {
#pragma warning disable CS0649

        [SerializeField] private TextMeshProUGUI titleText, countText;
        [SerializeField] private UI_BoxedIcon[] resourceIcons;
        [SerializeField] private float tweenDelay = 0.125f;
        private int resourceCount = 0;

        private void Awake()
        {
            // Set tween delays
            for (int i = 0; i < resourceIcons.Length; i++)
            {
                resourceIcons[i].delay = (i + 1) * tweenDelay;
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetData(RawMaterialData[] rawMaterials)
        {
            if (!gameObject.activeSelf) Show(true);
            resourceCount = rawMaterials.Length;
            countText.text = resourceCount.ToString();

            // Set icon data
            for (int i = 0; i < resourceIcons.Length; i++)
            {
                // If there are materials - set data
                if (i < resourceCount)
                {
                    resourceIcons[i].icon.sprite = rawMaterials[i].icon;
                    resourceIcons[i].title.text = rawMaterials[i].LocalizedName;
                    resourceIcons[i].gameObject.SetActive(true);
                    continue;
                }

                // No materials for remaining icons, disable
                resourceIcons[i].gameObject.SetActive(false);
            }
        }

        public void ClearResources()
        {
            for (int i = 0; i < resourceIcons.Length; i++)
            {
                resourceIcons[i].gameObject.SetActive(false);
            }
            countText.text = "0";
        }

        public void Show(bool show)
        {
            transform.DOKill();

            if (show)
            {
                gameObject.SetActive(true);
                transform.localScale = Vector2.zero;
            }

            transform.DOScale(show ? Vector2.one : Vector2.zero, 1f)
                .SetEase(show ? Ease.InBounce : Ease.OutBounce)
                .OnComplete(() =>
                {
                    if (!show)
                    {
                        ClearResources();
                        gameObject.SetActive(false);
                    }
                }
            );
        }

    }
}