using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] TextMeshProUGUI hpTxt;
    [SerializeField] TextMeshProUGUI weaponText;
    [SerializeField] TextMeshProUGUI ammoTxt;

    //INVENTARIO
    public Inventory inventory = new Inventory();
    public GameObject slotPrefab;
    public Transform gridParent;
    public GameObject contenedorInventario;
    private List<InventorySlotUI> inventoryList = new List<InventorySlotUI>();

    public List<CraftingRecipe> allRecipes;
    private CraftingSystem _craftingSystem;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    void Start()
    {
        initializeSlots();
        RefreshInventory();
        contenedorInventario.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (contenedorInventario.activeInHierarchy)
            {
                contenedorInventario.SetActive(false);
            }
            else
            {
                contenedorInventario.SetActive(true);
                RefreshInventory();
            }
        }
    }

    public void HpUpdate(float life)
    {
        hpTxt.text = $"HP: {life}";
    }

    #region Control del Inventario
    private void initializeSlots()
    {
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.clearSlot();
            inventoryList.Add(slotUI);
        }
    }

    public void RefreshInventory()
    {
        foreach (var slotUI in inventoryList)
        {
            slotUI.clearSlot();
        }

        for (int i = 0; i < inventory.ItemList.Count && i < inventoryList.Count; i++)
        {
            var data = inventory.ItemList[i];
            inventoryList[i].setSlot(data.item._icon, data.quantity);
        }
    }
    #endregion

    #region Datos del Arma
    public void updateWeaponDisplay(string weaponName, int current, int max)
    {
        weaponText.text = weaponName;
        ammoTxt.text = $"{current} / {max}";
    }

    public void updateAmmoDisplay(int current, int max)
    {
        ammoTxt.text = $"{current} / {max}";
    }
    #endregion
}
