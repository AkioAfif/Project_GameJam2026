using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private InventoryScript inventory;

    [Header("UI References")]
    [SerializeField] private Transform background;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;

    [Header("Level 6 Tutorial UI (Opsional)")]
    [SerializeField] private GameObject tutorialPanel;

    public static bool IsTutorialActive { get; private set; } = false;
    private static bool hasShownLevel6Tutorial = false;

    private void Awake()
    {
        if (background == null)
        {
            background = transform.Find("Background");
        }
        if (itemSlotContainer == null)
        {
            itemSlotContainer = transform.Find("ItemSlotContainer");
        }
        if (itemSlotTemplate == null && itemSlotContainer != null)
        {
            itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");
        }
        UpdateVisibility();
    }

    private void Start()
    {
        UpdateVisibility();
        RefreshInventoryItems();
    }

    public static void ResetTutorialState()
    {
        hasShownLevel6Tutorial = false;
        IsTutorialActive = false;
    }

    public void SetInventory(InventoryScript inventory)
    {
        this.inventory = inventory;
        UpdateVisibility();
        RefreshInventoryItems();
    }

    public void UpdateVisibility()
    {
        int level = Player.GetCurrentLevel();
        bool showItemBar = (level >= 6);

        // Sembunyikan/tampilkan semua elemen anak dari UI_Inventory (Background, ItemSlotContainer, dll)
        foreach (Transform child in transform)
        {
            // Jangan sembunyikan tutorialPanel yang sedang dibuat di Canvas Root
            if (tutorialPanel != null && child == tutorialPanel.transform) continue;
            child.gameObject.SetActive(showItemBar);
        }
    }

    public void CheckAndShowLevel6Tutorial()
    {
        if (!hasShownLevel6Tutorial)
        {
            hasShownLevel6Tutorial = true;
            ShowLevel6Tutorial();
        }
    }

    public void ShowLevel6Tutorial()
    {
        IsTutorialActive = true;
        Time.timeScale = 0f;

        if (tutorialPanel == null)
        {
            CreateDefaultTutorialPanel();
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        Debug.Log("[UI_Inventory] Tutorial Level 6 Aktif - Game Paused.");
    }

    public void CloseLevel6Tutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        IsTutorialActive = false;
        Time.timeScale = 1f;
        Debug.Log("[UI_Inventory] Tutorial Level 6 Ditutup - Game Resumed.");
    }

    private void Update()
    {
        // Tutup tutorial jika menekan Space, Enter, atau Escape saat tutorial aktif
        if (IsTutorialActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseLevel6Tutorial();
        }
    }

    private void CreateDefaultTutorialPanel()
    {
        // Buat overlay hitam transparan
        GameObject panelObj = new GameObject("Level6_TutorialPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObj.transform.SetParent(transform.root, false); // Attach ke Root Canvas

        RectTransform rect = panelObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        Image img = panelObj.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);

        // Card Container
        GameObject cardObj = new GameObject("Card", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cardObj.transform.SetParent(panelObj.transform, false);
        RectTransform cardRect = cardObj.GetComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(520f, 320f);
        Image cardImg = cardObj.GetComponent<Image>();
        cardImg.color = new Color(0.12f, 0.14f, 0.2f, 0.98f);

        // Judul
        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(cardObj.transform, false);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0f, 100f);
        titleRect.sizeDelta = new Vector2(480f, 50f);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.text = "FITUR BARU: ITEM BAR!";
        titleText.fontSize = 26;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.85f, 0.2f);

        // Isi Teks
        GameObject bodyObj = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        bodyObj.transform.SetParent(cardObj.transform, false);
        RectTransform bodyRect = bodyObj.GetComponent<RectTransform>();
        bodyRect.anchoredPosition = new Vector2(0f, 15f);
        bodyRect.sizeDelta = new Vector2(460f, 130f);
        TextMeshProUGUI bodyText = bodyObj.GetComponent<TextMeshProUGUI>();
        bodyText.text = "Mulai Level 6, PowerUp yang kamu ambil akan disimpan di Item Bar (maksimal 4 item).\n\nTekan tombol 1, 2, 3, atau 4 pada keyboard untuk mengaktifkan PowerUp yang tersimpan!";
        bodyText.fontSize = 18;
        bodyText.alignment = TextAlignmentOptions.Center;
        bodyText.color = Color.white;

        // Tombol Lanjutkan
        GameObject btnObj = new GameObject("OKButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(cardObj.transform, false);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0f, -100f);
        btnRect.sizeDelta = new Vector2(180f, 48f);
        Image btnImg = btnObj.GetComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 1f);

        GameObject btnTextObj = new GameObject("BtnText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        btnTextObj.transform.SetParent(btnObj.transform, false);
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.sizeDelta = btnRect.sizeDelta;
        TextMeshProUGUI btnText = btnTextObj.GetComponent<TextMeshProUGUI>();
        btnText.text = "Lanjutkan";
        btnText.fontSize = 20;
        btnText.fontStyle = FontStyles.Bold;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(CloseLevel6Tutorial);

        tutorialPanel = panelObj;
    }

    public void RefreshInventoryItems()
    {
        // Pengaman jika belum ter-assign
        if (itemSlotContainer == null || itemSlotTemplate == null)
        {
            return;
        }

        // Clean the slate before drawing to prevent duplicates
        foreach (Transform child in itemSlotContainer)
        {
            // Skip the master template so it doesn't get destroyed
            if (child == itemSlotTemplate) continue;

            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 80f;

        ItemScript[] itemList = (inventory != null) ? inventory.GetItemList() : new ItemScript[4];

        for (int i = 0; i < itemList.Length; i++)
        {
            ItemScript item = itemList[i];

            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);

            Transform imageTransform = itemSlotRectTransform.Find("Images");
            if (imageTransform != null)
            {
                Image image = imageTransform.GetComponent<Image>();
                if (image != null)
                {
                    if (item != null)
                    {
                        image.gameObject.SetActive(true);
                        image.sprite = item.GetSprite();
                    }
                    else
                    {
                        image.gameObject.SetActive(false);
                    }
                }
            }

            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}