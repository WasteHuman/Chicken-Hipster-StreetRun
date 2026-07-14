using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Loading
{
    public class UILoadingView : MonoBehaviour
    {
        [SerializeField] private float _progressAnimDuration = 0.5f;
        [SerializeField] private Image _progressBarFill;

        public void ResetProgress() => _progressBarFill.fillAmount = 0f;

        public void SetLoadingProgress(float progress) => _progressBarFill.DOFillAmount(progress, _progressAnimDuration);
    }
}