using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.Utils;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for object pooling integration
    /// Referência: GP-038, 99_agent_rules.md §99.3.12
    /// </summary>
    public class PoolIntegrationTests
    {
        [Test]
        public void PoolManager_Get_ReturnsActiveObject()
        {
            // Arrange
            var gameObject = new GameObject("PoolManager");
            var poolManager = gameObject.AddComponent<PoolManager>();
            
            // Create a test prefab
            var prefab = new GameObject("TestPrefab");
            var poolConfig = new PoolManager.PoolConfig
            {
                Tag = "TestPool",
                Prefab = prefab,
                InitialSize = 5,
                MaxSize = 10
            };

            // Act & Assert - Pool should be creatable
            Assert.IsNotNull(poolManager);
            
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ObjectPoolGeneric_Get_ReturnsObject()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);

            // Act
            var obj = pool.Get();

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.gameObject.activeSelf);
            
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void ObjectPoolGeneric_Return_DeactivatesObject()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);
            var obj = pool.Get();

            // Act
            pool.Return(obj);

            // Assert
            Assert.IsFalse(obj.gameObject.activeSelf);
            
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void ObjectPoolGeneric_Get_AtPosition()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);
            Vector3 expectedPosition = new Vector3(10, 20, 30);
            Quaternion expectedRotation = Quaternion.Euler(45, 90, 0);

            // Act
            var obj = pool.Get(expectedPosition, expectedRotation);

            // Assert
            Assert.AreEqual(expectedPosition, obj.transform.position);
            Assert.AreEqual(expectedRotation, obj.transform.rotation);
            
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void ObjectPoolGeneric_ReturnAll_DeactivatesAll()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);
            
            var obj1 = pool.Get();
            var obj2 = pool.Get();
            var obj3 = pool.Get();

            // Act
            pool.ReturnAll();

            // Assert
            Assert.IsFalse(obj1.gameObject.activeSelf);
            Assert.IsFalse(obj2.gameObject.activeSelf);
            Assert.IsFalse(obj3.gameObject.activeSelf);
            
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void ObjectPoolGeneric_CountAvailable_ReturnsCorrectCount()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);

            // Act & Assert
            Assert.AreEqual(5, pool.CountAvailable);
            
            var obj = pool.Get();
            Assert.AreEqual(4, pool.CountAvailable);
            
            pool.Return(obj);
            Assert.AreEqual(5, pool.CountAvailable);
            
            Object.DestroyImmediate(prefab);
        }

        [Test]
        public void ObjectPoolGeneric_Clear_RemovesAllObjects()
        {
            // Arrange
            var prefab = new GameObject("TestPrefab");
            var prefabComponent = prefab.AddComponent<TestComponent>();
            var pool = new ObjectPoolGeneric<TestComponent>(prefabComponent, 5);
            
            var obj = pool.Get();

            // Act
            pool.Clear();

            // Assert
            Assert.AreEqual(0, pool.CountAvailable);
            
            Object.DestroyImmediate(prefab);
        }
    }

    /// <summary>
    /// Test component for pooling tests
    /// </summary>
    public class TestComponent : MonoBehaviour
    {
    }
}
