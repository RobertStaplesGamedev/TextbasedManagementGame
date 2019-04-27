using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Resources")]
    public int food;
    public int foodCost;
    public int foodCap;
    [HideInInspector] public int foodBought;
    public int energy;
    public int energyCost;
    public int energyCap;
    [HideInInspector] public int energyBought;

    public int money;
    public int popLoss = 1;

    bool gameLost = false;

    [Header("Debt")]
    bool inDebt = false;
    int debtDayCheck;
    int debtMonthCheck;
    int debtYearCheck;

    [Header("Expenses")]
    public int hunger;
    public int energysuck;

    [HideInInspector] public float energyGrowth;
    [HideInInspector] public float foodGrowth;
    [HideInInspector] public float researchGrowth;
    [HideInInspector] public float income;
    [HideInInspector] public bool inEnergyDebt = false;
    [HideInInspector] public bool energyDrained = false;
    bool inFoodDebt = false;
    bool foodStarved = false;
    int starveDayCheck;
    int starveMonthCheck;
    int starveYearCheck;

    [Header("Ore")]
    public Commodity ore;
    public int oreCap;
    [HideInInspector] public int oreAmount;
    [HideInInspector] public int oreOnShip;
    public float oreRate;

    int upgradeStorageCost;

    [Header("Population")]
    public Commodity pop;
    [HideInInspector] public int popAmount;
    [HideInInspector] public int popBought;
    [HideInInspector] public int popSpare;

    [Header("Time")]
    public int month;
    public int day;
    public int year;
    public float timeInDay;
    private float timeInDayDelta;

    public TextManager textManager;
    public ProductionManager productionManager;
    public UIManager uiManager;
    public ShipManager shipManager;
    public TechManager techManager;

    void Start() {
        oreAmount = ore.startAmount;
        upgradeStorageCost = oreCap / 10;

        popAmount = pop.startAmount;
        popSpare = popAmount;

        uiManager.UpdateOreValues(oreCap);
    }

    void FixedUpdate()
    {
        shipManager.CheckShipStatus();
        shipManager.UpdateShipValues();
        productionManager.WriteValues();
        CalculateGrowth();
        IncrementTime();
        uiManager.WriteValues();
        GameLossCheck();
    }

    void GameLossCheck() {
        if (gameLost) {
            SceneManager.LoadScene("Game Over Screen");
        }
    }

    public void MineOre(){
        if (oreAmount < oreCap) {
            oreAmount ++;
        }
    }


    public void LoadOre() {
        if (shipManager.shipStatus == 2 && shipManager.shipStorage < shipManager.shipCap) {
            if (oreAmount <= (shipManager.shipCap - shipManager.shipStorage) && oreAmount > 0) {
                textManager.SendMessageToChat(ore.storageSucess, Message.MessageType.info);
                oreOnShip = oreAmount;
                shipManager.shipStorage += oreAmount;
                oreAmount = 0;
            } else if (oreAmount > (shipManager.shipCap - shipManager.shipStorage) && oreAmount > 0) {
                oreOnShip = oreAmount;
                oreAmount -= (shipManager.shipCap - shipManager.shipStorage);
                shipManager.shipStorage = shipManager.shipCap;
                textManager.SendMessageToChat(ore.storageSucess, Message.MessageType.info);
            } else if (shipManager.shipCap == shipManager.shipStorage) {
                textManager.SendMessageToChat(ore.storageFailMoney, Message.MessageType.info);
            } else if (oreAmount <= 0) {
                textManager.SendMessageToChat("No ore to transfer", Message.MessageType.info);
            }
        } else if (shipManager.shipStatus != 2) {
            textManager.SendMessageToChat("Ship not on mars", Message.MessageType.info);
        }
    }

    void CalculateGrowth() {
        foodGrowth = productionManager.CalculateProd(Commodity.CommodityType.Food) - (hunger * popAmount);
        energyGrowth = productionManager.CalculateProd(Commodity.CommodityType.Energy) - (energysuck * popAmount);
        income = -(pop.cost * (popAmount - 1));
    }

    void ChangeValues() {
        food += Mathf.RoundToInt(foodGrowth);
        energy += Mathf.RoundToInt(energyGrowth);
        money += Mathf.RoundToInt(income);

        if (food <= 0) {
            food = 0;
        }
        if (energy <= 0 && !inEnergyDebt) {
            energy = 0;
            inEnergyDebt = true;
            textManager.SendMessageToChat("RUN OUT OF POWER. ALL BUILDINGS HAVE STOPPED PRODUCTION", Message.MessageType.warning);
        } else if (energy > 0 && inEnergyDebt) {
            inEnergyDebt = false;
        }
    }

    public void UpgradeStorage() {
        if (money >= upgradeStorageCost) {
            oreCap = oreCap + 1000;
            money -= upgradeStorageCost;
            textManager.SendMessageToChat("Storage Upgraded", Message.MessageType.info);
            upgradeStorageCost = oreCap / 10;
        } else {
            textManager.SendMessageToChat("Not Enough Money", Message.MessageType.info);
        }
    }

    void DebtCheck() {
        if (money < 0 && !inDebt) {
            inDebt = true;
            debtDayCheck = day;
            if (month > 6) {
                debtMonthCheck = (month + 6) % 12;
                debtYearCheck = year + 1;
            } else {
                debtMonthCheck = month + 6;
                debtYearCheck = year + 0;
            }
            string loseString = "YOU ARE IN DEBT. Your colony will be SHUT DOWN on: " + debtDayCheck + "/" + debtMonthCheck + "/" + debtYearCheck;
            textManager.SendMessageToChat(loseString, Message.MessageType.warning);
        } else if (money >= 0 && inDebt) {
            inDebt = false;
            debtDayCheck = 0;
            debtMonthCheck = 0;
            debtYearCheck = 0;
        } else if (money < 0 && inDebt && debtDayCheck == day && debtMonthCheck == month && debtYearCheck == year) {
            gameLost = true;
        }
    }

    void StarveCheck() {
        if (food <= 0 && !inFoodDebt) {
            inFoodDebt = true;
            starveDayCheck = day;
            if (month == 12) {
                starveMonthCheck = 1;
                starveYearCheck = year + 1;
            } else {
                starveMonthCheck = month + 1;
                starveYearCheck = year + 0;
            }
            string loseString = "YOU HAVE RUN OUT OF FOOD. On: " + starveDayCheck + "/" + starveMonthCheck + "/" + starveYearCheck + " your population will start starving";
            textManager.SendMessageToChat(loseString, Message.MessageType.warning);
        } else if (food > 0 && inFoodDebt) {
            inDebt = false;
            foodStarved = false;
            starveDayCheck = 0;
            starveMonthCheck = 0;
            starveYearCheck = 0;
        } else if (food <= 0 && inFoodDebt && starveDayCheck == day && starveMonthCheck == month && starveYearCheck == year) {
            foodStarved = true;
        }
        if (food <= 0 && foodStarved) {
            if (popSpare > 0 ) {
                popSpare -= popLoss;
                popAmount -= popLoss;
                textManager.SendMessageToChat("PERSON HAS DIED DUE TO LACK OF FOOD", Message.MessageType.warning);
            } else {
                int poplossCounter = popLoss;
                for (int i = 0; i < productionManager.commodities.Length; i++) {
                    if (productionManager.commodities[i].commodityStaffed > 0) {
                        if (productionManager.commodities[i].commodityStaffed >= poplossCounter) {
                            productionManager.commodities[i].commodityStaffed -= poplossCounter;
                            popAmount -= popLoss;
                            poplossCounter = 0;
                            textManager.SendMessageToChat("PERSON HAS DIED DUE TO LACK OF FOOD", Message.MessageType.warning);
                            productionManager.commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = productionManager.commodities[i].commodityStaffed.ToString();
                        } else if (productionManager.commodities[i].commodityStaffed < poplossCounter) {
                            productionManager.commodities[i].commodityStaffed = 0;
                            poplossCounter -= productionManager.commodities[i].commodityStaffed;
                            popAmount -= popLoss;
                            textManager.SendMessageToChat("PERSON HAS DIED DUE TO LACK OF FOOD", Message.MessageType.warning);
                            productionManager.commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = productionManager.commodities[i].commodityStaffed.ToString();
                        }
                    }
                    if (poplossCounter <= 0) {
                        break;
                    }
                }
            }
        }
        if (popAmount == 0) {
            gameLost = true;
        }
    }

    void IncrementTime() {
        if (timeInDay < timeInDayDelta) {
            day++;
            timeInDayDelta = 0;
            if (oreAmount < oreCap) {
                oreAmount += productionManager.CalculateProd(Commodity.CommodityType.Ore);
            }
            techManager.UpdateResearch(productionManager.CalculateProd(Commodity.CommodityType.Research));
            //Check for fail Conditions
            DebtCheck();
            StarveCheck();
        } else {
            timeInDayDelta += Time.deltaTime;
        }

        if (day > 30) {
            day = 1;
            month++;
            ChangeValues();
            if (month > 12) {
                month = 1;
                year++;
            }
        }
    }
}
