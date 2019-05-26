using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {

    public class TechManager : MonoBehaviour
    {
        public GameManager gameManger;

        [Header("cost")]
        public int costToShuffle;

        [Header("Cards")]
        public TechnologyData database;
        public Technology[] technologies;

        public Color selectedCard;
        public Color unselectedCard;

        [Header("Left Card")]
        public GameObject leftCard;
        public Technology leftTech;
        //public TMP_Text leftCardResearchAmountNum;

        [Header("Middle Card")]
        public GameObject middleCard;
        public Technology middleTech;

        [Header("Right Card")]
        public GameObject rightCard;
        public Technology rightTech;

        Technology currentTech;

        public void TechStart (){
            SetCanResearch();
            ShuffleDeck();
        }

        public void StartingTech() {
            for (int i = 0; i< technologies.Length; i++) {
                if (technologies[i].techCard.startingTechnology) {
                    currentTech = technologies[i];
                    ReseachFinished();
                }
            }
        }

        public void CreateDatabase() {
            technologies = new Technology[database.Technologies.Length];
            for (int i = 0; i < database.Technologies.Length; i++) {
                technologies[i] = new Technology(database.Technologies[i]);
            }
        }

        public void ClickShuffle() {
            if (gameManger.money >= costToShuffle) {
                gameManger.money -= costToShuffle;
                ShuffleDeck();
            } else {
                gameManger.textManager.SendMessageToChat("Not enough money to shuffle", Message.MessageType.warning);
            }
        }

        void ShuffleDeck() {

            for (int i = 0; i< technologies.Length; i++) {
                technologies[i].isSetAsCard = false;
            }

            for(int i = technologies.Length - 1; i > 1; i--)
            {
                // Pick an entry no later in the deck, or i itself.
                int j = Random.Range(0, i + 1);

                // Swap the order of the two entries.
                Technology swap = technologies[j];
                technologies[j] = technologies[i];
                technologies[i] = swap;
            }
            SetCard(leftCard, 0);
            SetCard(middleCard, 1);
            SetCard(rightCard, 2);
        }

        Technology GetCard() {
            int count = 0;
            while (count < technologies.Length) {
                if (technologies[count].canResearch && !technologies[count].researched && !technologies[count].isSetAsCard && !technologies[count].isStartingTech) {
                    technologies[count].isSetAsCard = true;
                    return technologies[count];
                }
                count++;
            }
            return null;
        }

        void SetCanResearch() {
            bool researchCheck = true;
            for (int i = 0; i < technologies.Length; i++) {
                if (technologies[i].techCard.DependantTech == null || technologies[i].techCard.DependantTech.Length == 0) {
                    technologies[i].canResearch = true;
                } else {
                    researchCheck = true;
                    for (int x = 0; x < technologies.Length; x++) {
                        for (int j = 0; j < technologies[i].techCard.DependantTech.Length; j++) {
                            if (technologies[i].techCard.DependantTech[j] == technologies[x].techCard && !technologies[x].researched) {
                                // //Debug.Log(j);
                                // Debug.Log(technologies[x].techCard.name);
                                researchCheck = false;
                                //technologies[i].canResearch = true;
                            }
                        }
                    }
                    if (researchCheck) {
                        technologies[i].canResearch = true;
                    }
                }
            }
        }

        // Setting the left card to current value
        void SetCard(GameObject card, int cardNum) {
            Technology techCard = GetCard();
            if (techCard == null) {
                card.SetActive(false);
            } else {
                card.SetActive(true);
                if (cardNum == 0) {
                    leftTech = techCard;
                } else if (cardNum == 1) {
                    middleTech = techCard;
                } else if (cardNum == 2) {
                    rightTech = techCard;
                } 
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = techCard.techName;
                card.transform.GetChild(1).GetComponent<Image>().sprite = techCard.techCard.techSprite;
                card.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = techCard.descrption;
                card.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = techCard.researchAmount.ToString();
                card.transform.GetChild(3).GetChild(2).GetComponent<TMP_Text>().text = " /" + techCard.researchCost.ToString();
            }
        }

        public void ResearchChoose(int card) {
            if (card == 0) {
                leftCard.transform.GetComponent<Image>().color = selectedCard;
                leftCard.transform.GetChild(2).GetComponent<Image>().color = selectedCard;
                middleCard.transform.GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                if (currentTech == null) { 
                    currentTech = leftTech;
                    currentTech.isResearching = true;
                } else if (currentTech != leftTech) {
                    currentTech.isResearching = false;
                    currentTech = leftTech;
                    currentTech.isResearching = true;
                }
                Research();
            
            } else if (card == 1) {
                leftCard.transform.GetComponent<Image>().color = unselectedCard;
                leftCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetComponent<Image>().color = selectedCard;
                middleCard.transform.GetChild(2).GetComponent<Image>().color = selectedCard;
                rightCard.transform.GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                if (currentTech == null) { 
                    currentTech = middleTech;
                    currentTech.isResearching = true;
                } else if (currentTech != middleTech) {
                    currentTech.isResearching = false;
                    currentTech = middleTech;
                    currentTech.isResearching = true;
                } 
                Research();

            } else if (card == 2) {
                leftCard.transform.GetComponent<Image>().color = unselectedCard;
                leftCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetComponent<Image>().color = selectedCard;
                rightCard.transform.GetChild(2).GetComponent<Image>().color = selectedCard;
                if (currentTech == null) { 
                    currentTech = rightTech;
                    currentTech.isResearching = true;
                } else if (currentTech != rightTech) {
                    currentTech.isResearching = false;
                    currentTech = rightTech;
                    currentTech.isResearching = true;
                } 
                Research();
            } 
        }

        void Research() {
            if (currentTech != null) {
                if (currentTech.isResearching && currentTech.researchAmount < currentTech.researchCost) {
                    currentTech.researchAmount++;
                    UpdateResearchValues();
                } else {
                    ReseachFinished();
                }
            }
        }

        public void UpdateResearchValues() {
            if (leftTech == currentTech) {
                float f = Mathf.InverseLerp(0,currentTech.researchCost,currentTech.researchAmount);
                f = Mathf.Lerp(0,140,f);
                leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(f, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                leftCard.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = currentTech.researchAmount.ToString();
            } else if (middleTech == currentTech) {
                float f = Mathf.InverseLerp(0,currentTech.researchCost,currentTech.researchAmount);
                f = Mathf.Lerp(0,140,f);
                middleCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(f, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                middleCard.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = currentTech.researchAmount.ToString();
            } else if (rightTech == currentTech) {
                float f = Mathf.InverseLerp(0,currentTech.researchCost,currentTech.researchAmount);
                f = Mathf.Lerp(0,140,f);
                rightCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(f, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                rightCard.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = currentTech.researchAmount.ToString();
            }
        }


        public void UpdateResearch(float growth) {
            if (currentTech != null) {
                if (currentTech.isResearching) {
                    if (currentTech.researchAmount < currentTech.researchCost && currentTech.researchAmount + growth < currentTech.researchCost) {
                        currentTech.researchAmount += Mathf.RoundToInt(growth);
                        UpdateResearchValues();
                    } else {
                        ReseachFinished();
                        UpdateResearchValues();
                    }
                }
            }
        }

        void ReseachFinished() {
            currentTech.researched = true;
            currentTech.canResearch = false;
            if (!currentTech.techCard.startingTechnology) {
                gameManger.textManager.SendMessageToChat(currentTech.techName + " Researched", Message.MessageType.info);
            }
            if (currentTech.techtype == TechnologyCard.TechType.Commodity) {
                for (int i = 0; i < gameManger.shipManager.shipInventory.Count; i++) {
                    if (currentTech.techCommodity == gameManger.shipManager.shipInventory[i].commodity) {
                        gameManger.shipManager.shipInventory[i].commodObject.SetActive(true);
                    }
                }
                if (currentTech.techCommodity.resource == CommodityData.Resource.Ore && currentTech.techCommodity.type != CommodityData.Type.Building) {
                    gameManger.oreManager.CreateOre(currentTech.techCommodity.OreData);
                    gameManger.oreManager.AddMiningCard(currentTech.techCommodity.OreData);
                } else {
                    gameManger.productionManager.CreateCommodity(currentTech.techCommodity);
                }
                
            } else if (currentTech.techtype == TechnologyCard.TechType.CommodityModifier) {
                for (int i = 0; i < gameManger.productionManager.commodities.Count; i++) {
                    if (gameManger.productionManager.commodities[i].commodityData == currentTech.techCommodity) {
                        gameManger.productionManager.commodities[i].modifier += currentTech.techCard.efficencymod;
                    }
                }
            }

            if (currentTech == leftTech) {
                leftCard.transform.GetComponent<Image>().color = unselectedCard;
                leftCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                SetCanResearch();
                ShuffleDeck();
            } else if (currentTech == middleTech) {
                middleCard.transform.GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                middleCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                SetCanResearch();
                ShuffleDeck();
            } else if (currentTech == rightTech) {
                rightCard.transform.GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetChild(2).GetComponent<Image>().color = unselectedCard;
                rightCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, leftCard.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                SetCanResearch();
                ShuffleDeck();
            }
            currentTech = null;
        }
    }

    public class Technology
    {
        public TechnologyCard techCard;
        public CommodityData techCommodity;

        public string techName;
        public string descrption;
        public int researchCost;

        public bool isStartingTech;
        public bool canResearch;
        public bool researched;
        public bool isResearching;
        public bool isSetAsCard;
        public int researchAmount;

        public TechnologyCard.TechType techtype;

        public Technology(TechnologyCard _techCard) {
            techCard =_techCard;
            techName = _techCard.techName;
            descrption = _techCard.descrption;
            researchCost = _techCard.researchCost;
            canResearch = false;
            researched = false;
            isResearching = false;
            isSetAsCard = false;
            isStartingTech = _techCard.startingTechnology;
            techCommodity = _techCard.commodity;
            techtype = _techCard.techType;
        }
    }
}