using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Colony {

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

        [Header("Population")]
        public CommodityData pop;
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
        public MapManager mapManager;
        public ShipManager shipManager;
        public TechManager techManager;
        public StoryManager storyManager;
        public OreManager oreManager;

        void Start() {
            techManager.CreateDatabase();
            productionManager.CreateDatabase();
            oreManager.CreateDatabase();
            storyManager.CreateDatabase();

            techManager.StartingTech();
            techManager.TechStart();
            oreManager.StartOre();

            popAmount = pop.startAmount;
            popSpare = popAmount;
        }

        void FixedUpdate()
        {
            shipManager.CheckShipStatus();
            shipManager.UpdateShipValues();
            productionManager.WriteValues();
            oreManager.WriteValues();
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

        void CalculateGrowth() {
            foodGrowth = productionManager.CalculateProd(CommodityData.CommodityType.Food) - (hunger * popAmount);
            energyGrowth = productionManager.CalculateProd(CommodityData.CommodityType.Energy) - (energysuck * popAmount);
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
                    for (int i = 0; i < productionManager.commodities.Count; i++) {
                        if (productionManager.commodities[i].staffed > 0) {
                            if (productionManager.commodities[i].staffed >= poplossCounter) {
                                productionManager.commodities[i].staffed -= poplossCounter;
                                popAmount -= popLoss;
                                poplossCounter = 0;
                                textManager.SendMessageToChat("PERSON HAS DIED DUE TO LACK OF FOOD", Message.MessageType.warning);
                                productionManager.commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = productionManager.commodities[i].staffed.ToString();
                            } else if (productionManager.commodities[i].staffed < poplossCounter) {
                                productionManager.commodities[i].staffed = 0;
                                poplossCounter -= productionManager.commodities[i].staffed;
                                popAmount -= popLoss;
                                textManager.SendMessageToChat("PERSON HAS DIED DUE TO LACK OF FOOD", Message.MessageType.warning);
                                productionManager.commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = productionManager.commodities[i].staffed.ToString();
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
                oreManager.MineAll();
                oreManager.IncrementOreRate();
                techManager.UpdateResearch(productionManager.CalculateProd(CommodityData.CommodityType.Research));
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
}