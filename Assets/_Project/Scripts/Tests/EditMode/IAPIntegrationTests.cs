using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Integration tests for IAP flow (mocked)
    /// Referência: QA-010
    /// </summary>
    [TestFixture]
    public class IAPIntegrationTests
    {
        [Test]
        public void IAP_PurchaseSoftCurrency_IncreasesBalance()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "soft_pack_small",
                Amount = 100,
                Type = PurchaseType.SoftCurrency
            });

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(100, economy.SoftCurrency);
        }

        [Test]
        public void IAP_PurchaseHardCurrency_IncreasesBalance()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "hard_pack_small",
                Amount = 50,
                Type = PurchaseType.HardCurrency
            });

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(50, economy.HardCurrency);
        }

        [Test]
        public void IAP_PurchaseMultipleTimes_Accumulates()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            iap.ProcessPurchase(new PurchaseData { ProductId = "soft_1", Amount = 100, Type = PurchaseType.SoftCurrency });
            iap.ProcessPurchase(new PurchaseData { ProductId = "soft_2", Amount = 200, Type = PurchaseType.SoftCurrency });

            // Assert
            Assert.AreEqual(300, economy.SoftCurrency);
        }

        [Test]
        public void IAP_InvalidProduct_ReturnsFalse()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "invalid_product",
                Amount = 100,
                Type = PurchaseType.SoftCurrency
            });

            // Assert
            Assert.IsFalse(success);
        }

        [Test]
        public void IAP_RevivePurchase_ChecksHardCurrency()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            economy.AddHardCurrency(50, "Initial");
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "revive",
                Amount = 0,
                Type = PurchaseType.Revive,
                CostHardCurrency = 30
            });

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(20, economy.HardCurrency); // 50 - 30 = 20
        }

        [Test]
        public void IAP_RevivePurchase_InsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            economy.AddHardCurrency(10, "Initial");
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "revive",
                Amount = 0,
                Type = PurchaseType.Revive,
                CostHardCurrency = 30
            });

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(10, economy.HardCurrency); // Unchanged
        }

        [Test]
        public void IAP_RemoveAds_SetsFlag()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "remove_ads",
                Amount = 0,
                Type = PurchaseType.RemoveAds
            });

            // Assert
            Assert.IsTrue(success);
            Assert.IsTrue(iap.AdsRemoved);
        }

        [Test]
        public void IAP_BattlePass_SetsFlag()
        {
            // Arrange
            var economy = new EconomyIntegrationTest();
            var iap = new IAPIntegrationTest(economy);

            // Act
            bool success = iap.ProcessPurchase(new PurchaseData
            {
                ProductId = "battle_pass",
                Amount = 0,
                Type = PurchaseType.BattlePass
            });

            // Assert
            Assert.IsTrue(success);
            Assert.IsTrue(iap.BattlePassActive);
        }

        /// <summary>
        /// Simple economy test helper
        /// </summary>
        private class EconomyIntegrationTest
        {
            public int SoftCurrency { get; private set; }
            public int HardCurrency { get; private set; }

            public bool AddSoftCurrency(int amount, string reason)
            {
                SoftCurrency += amount;
                return true;
            }

            public bool AddHardCurrency(int amount, string reason)
            {
                HardCurrency += amount;
                return true;
            }

            public bool SpendHardCurrency(int amount, string reason)
            {
                if (HardCurrency < amount) return false;
                HardCurrency -= amount;
                return true;
            }
        }

        /// <summary>
        /// Simple IAP test helper
        /// </summary>
        private class IAPIntegrationTest
        {
            private EconomyIntegrationTest economy;
            public bool AdsRemoved { get; private set; }
            public bool BattlePassActive { get; private set; }

            public IAPIntegrationTest(EconomyIntegrationTest economy)
            {
                this.economy = economy;
            }

            public bool ProcessPurchase(PurchaseData purchase)
            {
                switch (purchase.Type)
                {
                    case PurchaseType.SoftCurrency:
                    case PurchaseType.HardCurrency:
                        return ProcessCurrencyPurchase(purchase);

                    case PurchaseType.Revive:
                        return ProcessRevivePurchase(purchase);

                    case PurchaseType.RemoveAds:
                        AdsRemoved = true;
                        return true;

                    case PurchaseType.BattlePass:
                        BattlePassActive = true;
                        return true;

                    default:
                        return false;
                }
            }

            private bool ProcessCurrencyPurchase(PurchaseData purchase)
            {
                if (purchase.Amount <= 0) return false;

                if (purchase.Type == PurchaseType.SoftCurrency)
                {
                    economy.AddSoftCurrency(purchase.Amount, purchase.ProductId);
                }
                else
                {
                    economy.AddHardCurrency(purchase.Amount, purchase.ProductId);
                }

                return true;
            }

            private bool ProcessRevivePurchase(PurchaseData purchase)
            {
                if (purchase.CostHardCurrency > 0)
                {
                    return economy.SpendHardCurrency(purchase.CostHardCurrency, "Revive");
                }
                return true;
            }
        }

        /// <summary>
        /// Purchase data
        /// </summary>
        private class PurchaseData
        {
            public string ProductId;
            public int Amount;
            public PurchaseType Type;
            public int CostHardCurrency;
        }

        /// <summary>
        /// Purchase types
        /// </summary>
        private enum PurchaseType
        {
            SoftCurrency,
            HardCurrency,
            Revive,
            RemoveAds,
            BattlePass
        }
    }
}
