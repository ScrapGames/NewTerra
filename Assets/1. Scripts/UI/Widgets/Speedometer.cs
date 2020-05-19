using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Widgets
{
    public class Speedometer : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] Image fillBar;
        [SerializeField] float animationTime, value;
        [SerializeField] Ease easeType;
        [SerializeField] bool animateOnStart;

        private void OnEnable()
        {
            if (animateOnStart)
                Animate();
        }

        public void SetValue(float value)
        {
            this.value = value;
            Animate();
        }

        public void Animate()
        {
            fillBar.DOKill();
            fillBar.fillAmount = 0;
            fillBar.DOFillAmount(value, animationTime).SetEase(easeType);
        }
    }
}