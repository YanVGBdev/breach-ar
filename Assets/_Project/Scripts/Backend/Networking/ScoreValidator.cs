using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Backend
{
    /// <summary>
    /// Server-side score validation for anti-cheat
    /// Injected via VContainer DI
    /// </summary>
    public class ScoreValidator : MonoBehaviour
    {
        [Header("Validation Rules")]
        [SerializeField] private float maxScorePerSecond = 1000f;
        [SerializeField] private int maxFragmentsPerSecond = 20;
        [SerializeField] private float maxComboMultiplier = 5f;
        [SerializeField] private int maxWavesPerMinute = 5;

        /// <summary>
        /// Validate a score submission
        /// </summary>
        public ValidationResult ValidateScore(ScoreSubmission submission)
        {
            var result = new ValidationResult { IsValid = true };

            if (submission.Score < 0)
            {
                result.IsValid = false;
                result.Reason = "Negative score";
                return result;
            }

            if (submission.Duration > 0)
            {
                float scorePerSecond = submission.Score / submission.Duration;
                if (scorePerSecond > maxScorePerSecond)
                {
                    result.IsValid = false;
                    result.Reason = $"Score per second ({scorePerSecond:F0}) exceeds maximum ({maxScorePerSecond})";
                    return result;
                }
            }

            if (submission.Duration > 0 && submission.FragmentsKilled > 0)
            {
                float fragmentsPerSecond = submission.FragmentsKilled / submission.Duration;
                if (fragmentsPerSecond > maxFragmentsPerSecond)
                {
                    result.IsValid = false;
                    result.Reason = $"Fragments per second ({fragmentsPerSecond:F0}) exceeds maximum ({maxFragmentsPerSecond})";
                    return result;
                }
            }

            if (submission.MaxCombo > maxComboMultiplier)
            {
                result.IsValid = false;
                result.Reason = $"Max combo ({submission.MaxCombo:F1}) exceeds maximum ({maxComboMultiplier})";
                return result;
            }

            if (submission.Duration > 0 && submission.WavesCleared > 0)
            {
                float wavesPerMinute = (submission.WavesCleared / submission.Duration) * 60f;
                if (wavesPerMinute > maxWavesPerMinute)
                {
                    result.IsValid = false;
                    result.Reason = $"Waves per minute ({wavesPerMinute:F1}) exceeds maximum ({maxWavesPerMinute})";
                    return result;
                }
            }

            if (submission.Events.Count > 0)
            {
                var suspiciousEvents = DetectSuspiciousPatterns(submission);
                if (suspiciousEvents.Count > 0)
                {
                    result.IsValid = false;
                    result.Reason = $"Suspicious patterns detected: {string.Join(", ", suspiciousEvents)}";
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Detect suspicious patterns in events
        /// </summary>
        private List<string> DetectSuspiciousPatterns(ScoreSubmission submission)
        {
            var suspicious = new List<string>();

            if (submission.Events.Count > 1)
            {
                for (int i = 1; i < submission.Events.Count; i++)
                {
                    float timeDiff = submission.Events[i].Timestamp - submission.Events[i - 1].Timestamp;
                    if (timeDiff < 0.05f)
                    {
                        suspicious.Add("Impossible event timing");
                        break;
                    }
                }
            }

            foreach (var scoreEvent in submission.Events)
            {
                if (scoreEvent.ScoreDelta > 5000)
                {
                    suspicious.Add($"Impossible score delta: {scoreEvent.ScoreDelta}");
                    break;
                }
            }

            return suspicious;
        }
    }

    /// <summary>
    /// Score submission data
    /// </summary>
    [System.Serializable]
    public class ScoreSubmission
    {
        public string PlayerId;
        public string LeaderboardId;
        public int Score;
        public int WavesCleared;
        public int FragmentsKilled;
        public int RiftsClosed;
        public float MaxCombo;
        public float Duration;
        public List<ScoreEvent> Events;
        public string RunSignature;
    }

    /// <summary>
    /// Individual score event
    /// </summary>
    [System.Serializable]
    public class ScoreEvent
    {
        public string EventType;
        public int ScoreDelta;
        public float Timestamp;
        public Dictionary<string, object> Parameters;
    }

    /// <summary>
    /// Validation result
    /// </summary>
    [System.Serializable]
    public class ValidationResult
    {
        public bool IsValid;
        public string Reason;
    }
}
