using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Anti-cheat tests for score validation
    /// Referência: QA-030
    /// </summary>
    [TestFixture]
    public class AntiCheatTests
    {
        [Test]
        public void ScoreValidator_NegativeScore_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest();

            // Act
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = -1000,
                Duration = 60f,
                FragmentsKilled = 100,
                WavesCleared = 5
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("Negative"));
        }

        [Test]
        public void ScoreValidator_ImpossiblyHighScore_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest(maxScorePerSecond: 1000f);

            // Act - Score of 100000 in 10 seconds = 10000/s
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 100000,
                Duration = 10f,
                FragmentsKilled = 100,
                WavesCleared = 5
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("Score per second"));
        }

        [Test]
        public void ScoreValidator_TooManyFragments_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest(maxFragmentsPerSecond: 20);

            // Act - 500 fragments in 10 seconds = 50/s
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 50000,
                Duration = 10f,
                FragmentsKilled = 500,
                WavesCleared = 5
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("Fragments per second"));
        }

        [Test]
        public void ScoreValidator_ImpossibleCombo_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest(maxCombo: 5f);

            // Act
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 10000,
                Duration = 60f,
                FragmentsKilled = 100,
                WavesCleared = 5,
                MaxCombo = 10f
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("Max combo"));
        }

        [Test]
        public void ScoreValidator_TooManyWaves_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest(maxWavesPerMinute: 5);

            // Act - 20 waves in 2 minutes = 10/min
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 20000,
                Duration = 120f,
                FragmentsKilled = 200,
                WavesCleared = 20
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("Waves per minute"));
        }

        [Test]
        public void ScoreValidator_SuspiciousTiming_Rejected()
        {
            // Arrange
            var validator = new ScoreValidatorTest();

            // Act - Events happening too fast
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 1000,
                Duration = 60f,
                FragmentsKilled = 10,
                WavesCleared = 2,
                Events = new[]
                {
                    new ScoreEventTest { Timestamp = 0f, ScoreDelta = 500 },
                    new ScoreEventTest { Timestamp = 0.01f, ScoreDelta = 500 } // Too fast!
                }
            });

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Reason.Contains("timing") || result.Reason.Contains("Suspicious"));
        }

        [Test]
        public void ScoreValidator_ValidScore_Accepted()
        {
            // Arrange
            var validator = new ScoreValidatorTest();

            // Act
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 5000,
                Duration = 120f,
                FragmentsKilled = 100,
                WavesCleared = 10,
                MaxCombo = 3.5f
            });

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ScoreValidator_EmptyDuration_Accepted()
        {
            // Arrange
            var validator = new ScoreValidatorTest();

            // Act - Duration might be 0 for instant submissions
            var result = validator.ValidateScore(new ScoreSubmissionTest
            {
                Score = 100,
                Duration = 0f,
                FragmentsKilled = 5,
                WavesCleared = 1
            });

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Simple score validator test helper
        /// </summary>
        private class ScoreValidatorTest
        {
            private float maxScorePerSecond;
            private int maxFragmentsPerSecond;
            private float maxCombo;
            private int maxWavesPerMinute;

            public ScoreValidatorTest(
                float maxScorePerSecond = 1000f,
                int maxFragmentsPerSecond = 20,
                float maxCombo = 5f,
                int maxWavesPerMinute = 5)
            {
                this.maxScorePerSecond = maxScorePerSecond;
                this.maxFragmentsPerSecond = maxFragmentsPerSecond;
                this.maxCombo = maxCombo;
                this.maxWavesPerMinute = maxWavesPerMinute;
            }

            public ValidationResultTest ValidateScore(ScoreSubmissionTest submission)
            {
                // Check negative score
                if (submission.Score < 0)
                {
                    return new ValidationResultTest { IsValid = false, Reason = "Negative score" };
                }

                // Check score per second
                if (submission.Duration > 0)
                {
                    float scorePerSecond = submission.Score / submission.Duration;
                    if (scorePerSecond > maxScorePerSecond)
                    {
                        return new ValidationResultTest
                        {
                            IsValid = false,
                            Reason = $"Score per second ({scorePerSecond:F0}) exceeds maximum ({maxScorePerSecond})"
                        };
                    }
                }

                // Check fragments per second
                if (submission.Duration > 0 && submission.FragmentsKilled > 0)
                {
                    float fragmentsPerSecond = submission.FragmentsKilled / submission.Duration;
                    if (fragmentsPerSecond > maxFragmentsPerSecond)
                    {
                        return new ValidationResultTest
                        {
                            IsValid = false,
                            Reason = $"Fragments per second ({fragmentsPerSecond:F0}) exceeds maximum ({maxFragmentsPerSecond})"
                        };
                    }
                }

                // Check combo
                if (submission.MaxCombo > maxCombo)
                {
                    return new ValidationResultTest
                    {
                        IsValid = false,
                        Reason = $"Max combo ({submission.MaxCombo:F1}) exceeds maximum ({maxCombo})"
                    };
                }

                // Check waves per minute
                if (submission.Duration > 0 && submission.WavesCleared > 0)
                {
                    float wavesPerMinute = (submission.WavesCleared / submission.Duration) * 60f;
                    if (wavesPerMinute > maxWavesPerMinute)
                    {
                        return new ValidationResultTest
                        {
                            IsValid = false,
                            Reason = $"Waves per minute ({wavesPerMinute:F1}) exceeds maximum ({maxWavesPerMinute})"
                        };
                    }
                }

                // Check event timing
                if (submission.Events != null && submission.Events.Length > 1)
                {
                    for (int i = 1; i < submission.Events.Length; i++)
                    {
                        float timeDiff = submission.Events[i].Timestamp - submission.Events[i - 1].Timestamp;
                        if (timeDiff < 0.05f) // Less than 50ms between events
                        {
                            return new ValidationResultTest
                            {
                                IsValid = false,
                                Reason = "Suspicious event timing detected"
                            };
                        }
                    }
                }

                return new ValidationResultTest { IsValid = true };
            }
        }

        /// <summary>
        /// Score submission test data
        /// </summary>
        private class ScoreSubmissionTest
        {
            public int Score;
            public float Duration;
            public int FragmentsKilled;
            public int WavesCleared;
            public float MaxCombo;
            public ScoreEventTest[] Events;
        }

        /// <summary>
        /// Score event test data
        /// </summary>
        private class ScoreEventTest
        {
            public float Timestamp;
            public int ScoreDelta;
        }

        /// <summary>
        /// Validation result test data
        /// </summary>
        private class ValidationResultTest
        {
            public bool IsValid;
            public string Reason;
        }
    }
}
