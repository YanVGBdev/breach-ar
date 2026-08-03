using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for deterministic physics behavior
    /// </summary>
    public class DeterministicPhysicsTests
    {
        [Test]
        public void FixedTimestep_IsConsistent()
        {
            // Arrange
            float expectedTimestep = 0.02f;
            
            // Act
            float actualTimestep = Time.fixedDeltaTime;
            
            // Assert
            Assert.AreEqual(expectedTimestep, actualTimestep, 
                "Fixed timestep should be 0.02f for deterministic physics");
        }

        [Test]
        public void Gravity_IsScaledCorrectly()
        {
            // Arrange
            float expectedGravityScale = 0.6f;
            float baseGravity = -9.81f;
            float expectedGravity = baseGravity * expectedGravityScale;
            
            // Act
            Vector3 actualGravity = Physics.gravity;
            
            // Assert
            Assert.AreEqual(expectedGravity, actualGravity.y, 0.001f,
                "Gravity should be scaled by 0.6f for game feel");
        }

        [Test]
        public void TrajectorySimulation_IsDeterministic()
        {
            // Arrange
            Vector3 startPosition = new Vector3(0, 0, 0);
            Vector3 initialVelocity = new Vector3(10, 15, 0);
            float timeStep = 0.05f;
            int steps = 20;
            
            // Act - Run simulation twice
            Vector3[] trajectory1 = SimulateTrajectory(startPosition, initialVelocity, timeStep, steps);
            Vector3[] trajectory2 = SimulateTrajectory(startPosition, initialVelocity, timeStep, steps);
            
            // Assert - Results should be identical
            Assert.AreEqual(trajectory1.Length, trajectory2.Length, 
                "Trajectory length should be consistent");
            
            for (int i = 0; i < trajectory1.Length; i++)
            {
                Assert.AreEqual(trajectory1[i], trajectory2[i], 0.0001f,
                    $"Trajectory point {i} should be deterministic");
            }
        }

        [Test]
        public void ProjectileMotion_FollowsParabolicPath()
        {
            // Arrange
            Vector3 startPos = Vector3.zero;
            Vector3 velocity = new Vector3(10, 20, 0);
            float timeStep = 0.1f;
            
            // Act
            Vector3[] trajectory = SimulateTrajectory(startPos, velocity, timeStep, 30);
            
            // Assert - Check parabolic characteristics
            // At peak, vertical velocity should be zero
            float maxHeight = 0f;
            int peakIndex = 0;
            
            for (int i = 1; i < trajectory.Length - 1; i++)
            {
                if (trajectory[i].y > maxHeight)
                {
                    maxHeight = trajectory[i].y;
                    peakIndex = i;
                }
            }
            
            // Peak should be approximately at v²/(2g)
            float expectedPeak = (velocity.y * velocity.y) / (2f * 9.81f * 0.6f);
            Assert.AreEqual(expectedPeak, maxHeight, 1f,
                "Peak height should follow projectile motion formula");
            
            // Peak should be roughly in the middle of trajectory
            Assert.IsTrue(peakIndex > 5 && peakIndex < trajectory.Length - 5,
                "Peak should occur in the middle portion of trajectory");
        }

        [Test]
        public void PhysicsMaterial_Bounciness_IsWithinRange()
        {
            // Arrange & Act
            PhysicMaterial wallMaterial = new PhysicMaterial("TestWall");
            wallMaterial.bounciness = 0.7f;
            
            PhysicMaterial furnitureMaterial = new PhysicMaterial("TestFurniture");
            furnitureMaterial.bounciness = 0.4f;
            
            PhysicMaterial floorMaterial = new PhysicMaterial("TestFloor");
            floorMaterial.bounciness = 0.2f;
            
            // Assert
            Assert.GreaterOrEqual(wallMaterial.bounciness, 0f);
            Assert.LessOrEqual(wallMaterial.bounciness, 1f);
            
            Assert.GreaterOrEqual(furnitureMaterial.bounciness, 0f);
            Assert.LessOrEqual(furnitureMaterial.bounciness, 1f);
            
            Assert.GreaterOrEqual(floorMaterial.bounciness, 0f);
            Assert.LessOrEqual(floorMaterial.bounciness, 1f);
            
            // Wall should be bounciest
            Assert.Greater(wallMaterial.bounciness, furnitureMaterial.bounciness);
            Assert.Greater(furnitureMaterial.bounciness, floorMaterial.bounciness);
            
            // Cleanup
            Object.DestroyImmediate(wallMaterial);
            Object.DestroyImmediate(furnitureMaterial);
            Object.DestroyImmediate(floorMaterial);
        }

        [Test]
        public void CollisionMatrix_IsConfiguredCorrectly()
        {
            // Arrange
            int orbLayer = LayerMask.NameToLayer("Orb");
            int fragmentLayer = LayerMask.NameToLayer("Fragment");
            int coreLayer = LayerMask.NameToLayer("Core");
            int surfaceLayer = LayerMask.NameToLayer("RealWorldSurface");
            
            // Act & Assert
            // Orb should collide with Fragment
            if (orbLayer >= 0 && fragmentLayer >= 0)
            {
                bool orbFragmentCollision = !Physics.GetIgnoreLayerCollision(orbLayer, fragmentLayer);
                Assert.IsTrue(orbFragmentCollision, 
                    "Orb should collide with Fragment");
            }
            
            // Orb should collide with Core
            if (orbLayer >= 0 && coreLayer >= 0)
            {
                bool orbCoreCollision = !Physics.GetIgnoreLayerCollision(orbLayer, coreLayer);
                Assert.IsTrue(orbCoreCollision, 
                    "Orb should collide with Core");
            }
            
            // Fragment should collide with Core
            if (fragmentLayer >= 0 && coreLayer >= 0)
            {
                bool fragmentCoreCollision = !Physics.GetIgnoreLayerCollision(fragmentLayer, coreLayer);
                Assert.IsTrue(fragmentCoreCollision, 
                    "Fragment should collide with Core");
            }
            
            // Orb should collide with surfaces
            if (orbLayer >= 0 && surfaceLayer >= 0)
            {
                bool orbSurfaceCollision = !Physics.GetIgnoreLayerCollision(orbLayer, surfaceLayer);
                Assert.IsTrue(orbSurfaceCollision, 
                    "Orb should collide with RealWorldSurface");
            }
        }

        [Test]
        public void ForceApplication_IsFrameRateIndependent()
        {
            // Arrange
            float force = 100f;
            float mass = 1f;
            
            // Act - Simulate at different timesteps
            float timestep1 = 0.02f; // 50 FPS
            float timestep2 = 0.01667f; // 60 FPS
            
            Vector3 velocity1 = CalculateVelocityAfterForce(force, mass, timestep1);
            Vector3 velocity2 = CalculateVelocityAfterForce(force, mass, timestep2);
            
            // Assert - Velocity should be similar (impulse-based)
            // Note: With AddForce as impulse, velocity should be the same
            Assert.AreEqual(velocity1.x, velocity2.x, 0.1f,
                "Force application should be consistent across frame rates");
        }

        [Test]
        public void RicochetAngle_IsWithinPhysicalLimits()
        {
            // Arrange
            Vector3 incomingDirection = new Vector3(1, -1, 0).normalized;
            Vector3 surfaceNormal = Vector3.up;
            
            // Act
            Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, surfaceNormal);
            
            // Assert
            float angle = Vector3.Angle(incomingDirection, reflectedDirection);
            
            // Reflection angle should be less than 180 degrees
            Assert.Less(angle, 180f, "Reflection angle should be physically valid");
            
            // Reflected direction should have positive Y component (bouncing up)
            Assert.Greater(reflectedDirection.y, 0f, 
                "Reflected orb should move upward after hitting floor");
        }

        /// <summary>
        /// Simulate trajectory with physics
        /// </summary>
        private Vector3[] SimulateTrajectory(Vector3 startPos, Vector3 velocity, float timeStep, int steps)
        {
            Vector3[] trajectory = new Vector3[steps + 1];
            trajectory[0] = startPos;
            
            Vector3 position = startPos;
            Vector3 vel = velocity;
            
            for (int i = 1; i <= steps; i++)
            {
                vel += Physics.gravity * timeStep;
                position += vel * timeStep;
                trajectory[i] = position;
            }
            
            return trajectory;
        }

        /// <summary>
        /// Calculate velocity after applying force
        /// </summary>
        private Vector3 CalculateVelocityAfterForce(float force, float mass, float timestep)
        {
            // Using impulse: v = F * dt / m
            float impulse = force * timestep;
            return new Vector3(impulse / mass, 0, 0);
        }
    }
}
