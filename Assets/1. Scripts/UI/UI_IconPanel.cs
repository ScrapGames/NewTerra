using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_IconPanel : MonoBehaviour
{
#pragma warning disable CS0649
    public Image icon;
    public Image image_Corner;
    [SerializeField] private bool startFadeOnStart;
    [SerializeField] private float fadeTime = 0.2f;


    private void Start()
    {
        if (startFadeOnStart) Fade(true);
    }


    public void Fade(bool start)
    {
        if (image_Corner == null) return;

        image_Corner.DOKill(true);

        if (start)
        {
            image_Corner.DOFade(0, fadeTime).SetLoops(-1);
            return;
        }

        // reset alpha
        image_Corner.DOFade(1, fadeTime);
    }
}
