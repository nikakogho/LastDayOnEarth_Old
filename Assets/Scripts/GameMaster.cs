using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public GameObject playerDefaultUI;
    public GameObject playerUI;
    public GameObject playerEquipmentUI, interactableInventoryUI;
    public Inventory interactableInventory;

    public Sprite[] defaultPlayerEquipmentIcons = new Sprite[System.Enum.GetNames(typeof(EquipmentSlot.SlotType)).Length];

    public Text openedUITitle;

    public Sprite defaultInventorySlotIcon;

    void Awake()
    {
        instance = this;
    }

    [HideInInspector]public GameObject openedUI = null;

    public void OpenUI(GameObject ui)
    {
        playerUI.SetActive(true);
        ui.SetActive(true);

        if (openedUI != null) openedUI.SetActive(false);
        openedUI = ui;

        openedUITitle.text = ui.name;

        playerDefaultUI.SetActive(false);
    }

    public void CloseUI()
    {
        playerUI.SetActive(false);

        if (openedUI != null)
        {
            openedUI.SetActive(false);
            openedUI = null;
        }

        playerDefaultUI.SetActive(true);
    }
}
