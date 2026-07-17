using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.WheelOfLuck
{
    [RequireComponent(typeof(RectTransform))]
    public class RewardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _label;

        [Space(5), Header("Reward Info")]
        [SerializeField] private WheelReward _reward;

        private int _index;
        private int _total;

        public WheelReward Reward => _reward;
        public int Index => _index;
        public float CenterAngle => _total > 0 ? _index * (360f / _total) + (360f / _total) * 0.5f : 0f;

        /// <summary>
        /// Настроить визуал для конкретной награды и позиции на колесе.
        /// Вызывает позиционирование сегмента и обновление текста/иконки.
        /// </summary>
        public void Configure(int index, int total)
        {
            _index = index;
            _total = Mathf.Max(1, total);

            UpdateVisuals();
            PositionOnWheel();
        }

        private void UpdateVisuals()
        {
            if (_label == null) 
                return;

            _label.text = _reward.Type switch
            {
                WheelReward.RewardType.Coins => _reward.Amount.ToString(),
                WheelReward.RewardType.FreeSpin => $"{_reward.Amount} SPINS",
                WheelReward.RewardType.Nothing => "FAIL",
                _ => string.Empty,
            };
        }

        private void PositionOnWheel()
        {
            float segment = 360f / _total;
            float centerAngle = _index * segment + segment * 0.5f;

            transform.localEulerAngles = new Vector3(0f, 0f, -centerAngle);

            if (_label != null)
                _label.rectTransform.localEulerAngles = new Vector3(0f, 0f, centerAngle);
        }
    }
}