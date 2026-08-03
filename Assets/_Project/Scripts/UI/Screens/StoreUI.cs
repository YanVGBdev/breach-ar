using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace BreachAR.UI
{
    /// <summary>
    /// Store UI screen
    /// </summary>
    public class StoreUI : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button cosmeticsTab;
        [SerializeField] private Button currencyTab;
        [SerializeField] private Button battlePassTab;
        [SerializeField] private GameObject cosmeticsPanel;
        [SerializeField] private GameObject currencyPanel;
        [SerializeField] private GameObject battlePassPanel;

        [Header("Currency Display")]
        [SerializeField] private TextMeshProUGUI softCurrencyText;
        [SerializeField] private TextMeshProUGUI hardCurrencyText;

        [Header("Items")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject storeItemPrefab;

        [Header("Battle Pass")]
        [SerializeField] private Transform battlePassTrack;
        [SerializeField] private GameObject battlePassTierPrefab;
        [SerializeField] private Button buyPremiumButton;
        [SerializeField] private TextMeshProUGUI battlePassTimerText;

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        private List<StoreItemUI> storeItems;

        private void Start()
        {
            SetupTabs();
            SetupButtons();
            LoadItems();
        }

        private void SetupTabs()
        {
            cosmeticsTab?.onClick.AddListener(() => ShowPanel(cosmeticsPanel));
            currencyTab?.onClick.AddListener(() => ShowPanel(currencyPanel));
            battlePassTab?.onClick.AddListener(() => ShowPanel(battlePassPanel));
        }

        private void SetupButtons()
        {
            backButton?.onClick.AddListener(OnBackClicked);
            buyPremiumButton?.onClick.AddListener(OnBuyPremiumClicked);
        }

        private void ShowPanel(GameObject panel)
        {
            cosmeticsPanel?.SetActive(false);
            currencyPanel?.SetActive(false);
            battlePassPanel?.SetActive(false);
            panel?.SetActive(true);
        }

        private void LoadItems()
        {
            // TODO: Load from backend
            // For now, create placeholder items
            storeItems = new List<StoreItemUI>();

            CreateStoreItem("orb_fire_skin", "Fire Orb Skin", 500, "orb_fire.png");
            CreateStoreItem("orb_ice_skin", "Ice Orb Skin", 500, "orb_ice.png");
            CreateStoreItem("core_golden", "Golden Core", 1000, "core_gold.png");
            CreateStoreItem("remove_ads", "Remove Ads", 1499, "no_ads.png");
        }

        private void CreateStoreItem(string id, string name, int price, string iconPath)
        {
            if (storeItemPrefab == null || itemContainer == null) return;

            GameObject itemObj = Instantiate(storeItemPrefab, itemContainer);
            StoreItemUI itemUI = itemObj.GetComponent<StoreItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(id, name, price, null); // Pass icon sprite
                itemUI.OnPurchaseClicked += OnItemPurchaseClicked;
                storeItems.Add(itemUI);
            }
        }

        private void OnItemPurchaseClicked(string itemId)
        {
            Debug.Log($"[Store] Purchase clicked: {itemId}");
            // TODO: Process purchase
        }

        private void OnBuyPremiumClicked()
        {
            Debug.Log("[Store] Buy premium battle pass clicked");
            // TODO: Process purchase
        }

        private void OnBackClicked()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Update currency display
        /// </summary>
        public void UpdateCurrencyDisplay(int soft, int hard)
        {
            if (softCurrencyText != null)
                softCurrencyText.text = soft.ToString("N0");
            if (hardCurrencyText != null)
                hardCurrencyText.text = hard.ToString("N0");
        }
    }

    /// <summary>
    /// Store item UI component
    /// </summary>
    public class StoreItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private GameObject ownedBadge;

        public event System.Action<string> OnPurchaseClicked;

        private string itemId;

        public void Initialize(string id, string name, int price, Sprite icon)
        {
            itemId = id;
            if (nameText != null) nameText.text = name;
            if (priceText != null) priceText.text = price.ToString();
            if (iconImage != null && icon != null) iconImage.sprite = icon;

            purchaseButton?.onClick.AddListener(() => OnPurchaseClicked?.Invoke(itemId));
        }

        public void SetOwned(bool owned)
        {
            if (ownedBadge != null) ownedBadge.SetActive(owned);
            if (purchaseButton != null) purchaseButton.interactable = !owned;
        }
    }
}
