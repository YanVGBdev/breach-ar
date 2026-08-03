using UnityEngine;
using BreachAR.Core;

namespace BreachAR.Utils
{
    /// <summary>
    /// Utilities for optimizing shaders for mobile
    /// Referência: OPT-006
    /// </summary>
    public class MobileShaderOptimizer : MonoBehaviour
    {
        [Header("Shader Settings")]
        [SerializeField] private bool enableShaderStripping = true;
        [SerializeField] private bool reduceInstructionCount = true;
        [SerializeField] private bool avoidOverdraw = true;

        [Header("Quality Thresholds")]
        [SerializeField] private int maxInstructionsLow = 50;
        [SerializeField] private int maxInstructionsMedium = 100;
        [SerializeField] private int maxInstructionsHigh = 200;

        /// <summary>
        /// Get recommended shader complexity for device tier
        /// </summary>
        public ShaderComplexity GetRecommendedComplexity(Core.DeviceTier tier)
        {
            return tier switch
            {
                Core.DeviceTier.Low => ShaderComplexity.Minimal,
                Core.DeviceTier.Medium => ShaderComplexity.Standard,
                Core.DeviceTier.High => ShaderComplexity.Full,
                _ => ShaderComplexity.Standard
            };
        }

        /// <summary>
        /// Check if shader passes overdraw budget
        /// </summary>
        public bool IsOverdrawAcceptable(int passCount, Core.DeviceTier tier)
        {
            int maxPasses = tier switch
            {
                Core.DeviceTier.Low => 2,
                Core.DeviceTier.Medium => 3,
                Core.DeviceTier.High => 5,
                _ => 3
            };

            return passCount <= maxPasses;
        }

        /// <summary>
        /// Get texture sampling budget per tier
        /// </summary>
        public int GetTextureSamplingBudget(Core.DeviceTier tier)
        {
            return tier switch
            {
                Core.DeviceTier.Low => 4,
                Core.DeviceTier.Medium => 8,
                Core.DeviceTier.High => 16,
                _ => 8
            };
        }

        /// <summary>
        /// Generate shader optimization report
        /// </summary>
        public ShaderOptimizationReport AnalyzeShader(Material material)
        {
            var report = new ShaderOptimizationReport
            {
                MaterialName = material.name,
                ShaderName = material.shader.name
            };

            // Count shader passes
            report.PassCount = material.passCount;

            // Estimate instruction count (rough approximation)
            report.EstimatedInstructions = EstimateInstructionCount(material);

            // Check for transparency
            int renderQueue = material.renderQueue;
            report.IsTransparent = renderQueue > 2500;

            // Generate recommendations
            report.Recommendations = GenerateRecommendations(report);

            return report;
        }

        /// <summary>
        /// Estimate instruction count (rough)
        /// </summary>
        private int EstimateInstructionCount(Material material)
        {
            int instructions = 0;

            // Basic estimation based on shader features
            if (material.HasProperty("_MainTex")) instructions += 5;
            if (material.HasProperty("_NormalMap")) instructions += 10;
            if (material.HasProperty("_EmissionColor")) instructions += 3;
            if (material.HasProperty("_Metallic")) instructions += 5;
            if (material.HasProperty("_Smoothness")) instructions += 3;

            // Transparency adds instructions
            if (material.renderQueue > 2500) instructions += 10;

            return instructions;
        }

        /// <summary>
        /// Generate optimization recommendations
        /// </summary>
        private string[] GenerateRecommendations(ShaderOptimizationReport report)
        {
            var recommendations = new System.Collections.Generic.List<string>();

            if (report.EstimatedInstructions > maxInstructionsMedium)
            {
                recommendations.Add("Consider using a simpler shader variant for mobile");
            }

            if (report.PassCount > 3)
            {
                recommendations.Add("Reduce number of shader passes to minimize overdraw");
            }

            if (report.IsTransparent)
            {
                recommendations.Add("Transparent shaders cause overdraw - consider using alpha cutout instead");
            }

            return recommendations.ToArray();
        }
    }

    /// <summary>
    /// Shader complexity levels
    /// </summary>
    public enum ShaderComplexity
    {
        Minimal,
        Standard,
        Full
    }

    /// <summary>
    /// Shader optimization report
    /// </summary>
    [System.Serializable]
    public class ShaderOptimizationReport
    {
        public string MaterialName;
        public string ShaderName;
        public int PassCount;
        public int EstimatedInstructions;
        public bool IsTransparent;
        public string[] Recommendations;
    }
}
