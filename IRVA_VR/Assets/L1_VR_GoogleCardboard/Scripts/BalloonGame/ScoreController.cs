using L1_VR_GoogleCardboard.Scripts.Helpers;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace L1_VR_GoogleCardboard.Scripts.BalloonGame
{
    /// <summary>
    /// Used to keep track of the game score & update a world space UI canvas.
    /// </summary>
    public class ScoreController : Singleton<ScoreController>
    {
        // Check the comments in 'BalloonController' for more info about the attributes used below.
        
        [SerializeField] 
        [BoxGroup("External components")] 
        [Required]
        private TextMeshProUGUI scoreText;

        // This code is part of the bonus.
        // Optional text used to display the current streak. If left empty, one is created at runtime by
        // cloning 'scoreText' and placing it right above it.
        [SerializeField]
        [BoxGroup("Streak (bonus)")]
        private TextMeshProUGUI streakText;

        // This code is part of the bonus.
        // Maximum extra points a single pop can give (pop #1 -> +1, pop #2 -> +2, ..., capped at 1 + maxStreakBonus).
        [SerializeField]
        [Range(0, 10)]
        [BoxGroup("Streak (bonus)")]
        private int maxStreakBonus = 4;

        // This code is part of the bonus.
        // Vertical offset (in canvas units) of the auto-created streak text relative to the score text.
        [SerializeField]
        [BoxGroup("Streak (bonus)")]
        private float streakTextOffsetY = 1.1f;

        private int _score;

        // This code is part of the bonus.
        private int _streak;

        // This code is part of the bonus.
        protected override void Awake()
        {
            base.Awake();
            CreateStreakTextIfMissing();
            UpdateStreakText();
        }

        public void IncrementScore()
        {
            // TODO 6.2 : Keep track of the score & set the UI text.
            // This code is part of the bonus: consecutive pops grow the streak and award extra points.
            _streak++;
            _score += 1 + Mathf.Min(_streak - 1, maxStreakBonus);
            scoreText.text = _score.ToString();
            UpdateStreakText(); // This code is part of the bonus.
        }

        public void DecrementScore()
        {
            // TODO 6.3 : Keep track of the score & set the UI text.
             _score--;
            scoreText.text = _score.ToString();

            // This code is part of the bonus: a balloon reached the ceiling, so the streak is broken.
            _streak = 0;
            UpdateStreakText();
        }

        // This code is part of the bonus.
        private void CreateStreakTextIfMissing()
        {
            if (streakText != null || scoreText == null)
                return;

            var scoreRect = scoreText.rectTransform;
            streakText = Instantiate(scoreText, scoreRect.parent, worldPositionStays: false);
            streakText.name = "StreakText";
            streakText.fontSize = scoreText.fontSize * 0.6f;
            streakText.color = new Color(1f, 0.85f, 0.2f);

            var streakRect = streakText.rectTransform;
            streakRect.anchorMin = scoreRect.anchorMin;
            streakRect.anchorMax = scoreRect.anchorMax;
            streakRect.sizeDelta = scoreRect.sizeDelta;
            streakRect.anchoredPosition = scoreRect.anchoredPosition + new Vector2(0f, streakTextOffsetY);
        }

        // This code is part of the bonus.
        private void UpdateStreakText()
        {
            if (streakText == null)
                return;

            var bonus = Mathf.Min(_streak, maxStreakBonus + 1);
            streakText.text = _streak > 0 ? $"Streak: {_streak} (+{bonus})" : "Streak: 0";
        }
    }
}
