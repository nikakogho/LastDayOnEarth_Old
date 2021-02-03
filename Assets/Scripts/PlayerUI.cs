using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject targetUI;
    public Text defaultHealthText, levelText, foodText, drinkText, otherHealthText, targetNameText, targetHealthText;
    public Image defaultHealthImage, xpImage, targetHealthBar;

    float lastHealth;
    int lastLevel, lastXP;
    string lastTargetName;
    float lastTargetHealth;
    int lastFood, lastDrink;
    bool targetActive;

    Player player;

    void Start()
    {
        InvokeRepeating("UpdateUI", 0.1f, 1);

        player = Player.instance;

        lastHealth = -1;
        lastLevel = -1;
        lastXP = -1;

        lastFood = -1;
        lastDrink = -1;

        lastTargetName = "Jartozavrius -1290";

        lastTargetHealth = -1;

        targetActive = true;
    }

    void XPUpdate()
    {
        lastXP = player.xp;

        xpImage.fillAmount = (float)lastXP / player.requiredXP;
    }

    void UpdateUI()
    {
        #region Health
        if (lastHealth != player.health)
        {
            lastHealth = player.health;

            int health = (int)lastHealth;

            if (health == 0 && lastHealth > 0) health = 1;

            otherHealthText.text = defaultHealthText.text = health.ToString();
            defaultHealthImage.fillAmount = lastHealth / player.startHealth;
        }
        #endregion

        #region Level
        if (lastLevel != player.level)
        {
            lastLevel = player.level;

            levelText.text = "lvl " + lastLevel;

            XPUpdate();
        }
        #endregion

        if (lastXP != player.xp) XPUpdate();

        #region Food
        if (lastFood != (int)player.hunger)
        {
            lastFood = (int)player.hunger;

            foodText.text = lastFood.ToString();
            foodText.color = Color.Lerp(Color.red, Color.green, player.hunger / 100);
        }
        #endregion

        #region Drink
        if (lastDrink != (int)player.thirst)
        {
            lastDrink = (int)player.thirst;

            drinkText.text = lastDrink.ToString();
            drinkText.color = Color.Lerp(Color.red, Color.green, player.thirst / 100);
        }
        #endregion

        #region Target

        if(player.target == null)
        {
            if (targetActive)
            {
                targetActive = false;
                targetUI.SetActive(false);
            }
        }
        else
        {
            if (!targetActive)
            {
                targetActive = true;
                targetUI.SetActive(true);
            }

            if(lastTargetHealth != player.target.health)
            {
                lastTargetHealth = player.target.health;

                targetHealthText.text = lastTargetHealth + "/" + player.target.startHealth;
                targetHealthBar.fillAmount = lastTargetHealth / player.target.startHealth;
            }

            if(lastTargetName != player.target.name)
            {
                lastTargetName = player.target.name;

                targetNameText.text = lastTargetName;
            }
        }

        #endregion
    }
}
