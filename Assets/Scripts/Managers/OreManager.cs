﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {
    public class OreManager : MonoBehaviour {

        public TechnologyData technologyData;
        public int rateAmountInterval;
        List<Ore> ores;

        public GameManager gameManager;

        [Header("Ore")]
        public int capacity;
        [HideInInspector] public int storage;

        int upgradeStorageCost;
        public CommodityData drillData;
        int drillSpare;
        public CommodityData tankData;

        [Header("UI")]
        public GameObject tutorialPanel;
        public GameObject tutorialDial_Obj;
        public TMP_Text tutorialOreRate;
        public TMP_Text tutorialStorage_Txt;
        public TMP_Text tutorialCapacity_Txt;
        
        public TMP_Text miningDrills_Txt;
        public TMP_Text drillsSpare_Txt;

        public GameObject dial_Obj;
        public TMP_Text storage_Txt;
        public TMP_Text capacity_Txt;

        public OreData siliconOre;
        public OreData ironOre;
        public OreData titainumOre;

        public GameObject leftCard;
        Ore leftOre;
        public GameObject middleCard;
        Ore middleOre;
        public GameObject rightCard;
        Ore rightOre;

        bool isAddingStaff;

        public void StartOre() {
            upgradeStorageCost = capacity / 10;

            drillSpare = gameManager.productionManager.GetCommodity(drillData).Amount;
            WriteValues();
        }

        public void CreateDatabase() {
            ores = new List<Ore>();
            // for (int i = 0; i < technologyData.ore.Length; i++) {
            //     CreateOre(technologyData.ore[i]);
            // }
        }

        public void WriteValues() {
            //Debug.Log();
            if (tutorialPanel.activeSelf) {
                tutorialPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = gameManager.productionManager.GetCommodity(drillData).Amount.ToString();
                tutorialPanel.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (gameManager.productionManager.GetCommodity(drillData).Amount - drillSpare).ToString();
                tutorialOreRate.text = "$" + GetOre(siliconOre).Rate.ToString("F2");
                float f = Mathf.InverseLerp(0,capacity,storage);
                f = Mathf.Lerp(-34f,34f,f);
                tutorialDial_Obj.GetComponent<RectTransform>().eulerAngles = new Vector3(0,0,-f);

                tutorialStorage_Txt.text = storage.ToString();
                tutorialCapacity_Txt.text = "/" + capacity.ToString();
            } else {
                float f = Mathf.InverseLerp(0,capacity,storage);
                f = Mathf.Lerp(-34f,34f,f);
                dial_Obj.GetComponent<RectTransform>().eulerAngles = new Vector3(0,0,-f);

                storage_Txt.text = storage.ToString();
                capacity_Txt.text = "/" + capacity.ToString();
                drillsSpare_Txt.text = drillSpare.ToString();
            }
            if (leftCard.activeSelf) {
                SetCardValues(leftCard, siliconOre); 
            }
            if (middleCard.activeSelf) {
                SetCardValues(middleCard, ironOre);
            } else if (rightCard.activeSelf) {
                SetCardValues(rightCard, titainumOre);
            }
        }

        void SetCardValues(GameObject card, OreData oreData) {
            card.transform.GetChild(0).GetComponent<TMP_Text>().text = oreData.title;
            Ore ore = GetOre(oreData);
            card.transform.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = ore.Staffed.ToString();
            card.transform.GetChild(3).GetComponent<TMP_Text>().text = ore.Amount.ToString();
            card.transform.GetChild(4).GetComponent<TMP_Text>().text = "$" + ore.Rate.ToString("F2");
        }

        public void AddMiningCard(OreData oreData) {
            GameObject card = leftCard;
            if (oreData.title == siliconOre.title) {
                card = leftCard;
                //leftOre = GetOre(oreData);
            } else if (oreData.title == ironOre.title) {
                card = middleCard;
            } else if (oreData.title == titainumOre.title) {
                card = rightCard;
            }
            card.SetActive(true);
            card.transform.GetChild(0).GetComponent<TMP_Text>().text = oreData.title;
            card.transform.GetChild(1).GetComponent<Image>().sprite = oreData.panelImage;

            card.transform.GetChild(5).GetComponent<Button>().onClick.RemoveAllListeners();
            card.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(delegate() { MineButton(oreData); });
            SetCardValues(card, oreData);

        }

        public void AddOreBuilding(CommodityData commodityData, int amount) {
            if (commodityData == drillData) {
                drillSpare++;
            } else if (commodityData == tankData) {
                UpgradeStorage(commodityData);
            }
        }

        public void IncrementOreRate() {
            for (int i = 0; i < ores.Count; i++) {
                ores[i].IncreaseRate();
            }
        }

        public void MineButton(OreData oreData) {
            Mine(oreData, 1);
        }

        public void Mine(OreData oreData, int growth) {
            Ore ore = GetOre(oreData);
            if (storage + growth <= capacity) {
                storage += growth;
                ore.MineOre(growth);
            } else if(storage + growth > capacity) {
                ore.MineOre(capacity - storage);
                capacity = storage;
            }
        }

        public void MineAll() {
            for (int i = 0; i < ores.Count; i++) {
                if (storage == capacity) {
                    break;
                } else {
                    int growth = ores[i].CalculateProd(gameManager.productionManager, drillData);
                    Mine(ores[i].data, growth);
                }
            }
        }

        public void UpgradeStorage(CommodityData commodity) {
                capacity = capacity + commodity.value;
        }

        public Ore GetOre(OreData oreData) {
            Ore ore = null;
            for (int i = 0; i < ores.Count; i++) {
                if (oreData.title == ores[i].data.title) { 
                    ore = ores[i];
                }
            }
            if (ore != null) {
                return ore;
            }
            return null;
        }

        public void StaffCheck(bool _isAddingStaff) {
               isAddingStaff = _isAddingStaff;
        }

        public void AddStaff(OreData oreData) {
            Ore ore = GetOre(oreData);
            if (isAddingStaff && drillSpare > 0 && gameManager.popSpare > 0) {
                ore.AddStaff(true);
                drillSpare--;
                gameManager.popSpare--;
            } else if (!isAddingStaff && ore.Staffed > 0) {
                ore.AddStaff(false);
                drillSpare++;
                gameManager.popSpare++;
            }
        }

        public (List<ShipItem>,int) LoadOre(OreData oreData, List<ShipItem> shipInventory, int shipSpace){
            for (int i = 0; i < shipInventory.Count; i++) {
                if (shipInventory[i].data.resource == CommodityData.Resource.Ore && shipInventory[i].data.type == CommodityData.Type.Resource) {
                    if (oreData == null || shipInventory[i].data.OreData == oreData) {
                        Ore ore = GetOre(shipInventory[i].data.OreData);
                        if (ore != null) {
                            if (shipSpace <= 0) {
                                break;
                            } else if (ore.Amount <= shipSpace) {
                                shipSpace -= ore.Amount;
                                shipInventory[i].AddAmount(ore.Amount);
                                storage -= ore.Amount;
                                gameManager.textManager.SendMessageToChat(ore.Amount+ " " + ore.data.title +" Loaded into ship",Message.MessageType.info);
                                ore.LoadOre(ore.Amount);
                            } else {
                                shipInventory[i].AddAmount(shipSpace);
                                storage -= shipSpace;
                                gameManager.textManager.SendMessageToChat(shipSpace+ " " + ore.data.title +" Loaded into ship",Message.MessageType.info);
                                ore.LoadOre(shipSpace);
                                shipSpace = 0;
                                break;
                            }
                        }
                    }
                }
            }
            return (shipInventory, shipSpace);
        }

        public void SellOre(OreData oreData, int amount) {
            gameManager.money += Mathf.RoundToInt(GetOre(oreData).SellOre(rateAmountInterval, amount, gameManager.textManager));
        }

        public void CreateOre(OreData oreData) {
            Ore newOre = new Ore(oreData);
            ores.Add(newOre);
        }
    }

    public class Ore {
        public OreData data;

        public float Rate { get { return rate; } private set { rate = value; } }
        private float rate;
        public float RateCap { get { return rateCap; } private set { rateCap = value; } }
        private float rateCap;
        public float RateBottom { get { return rateBottom; } private set { rateBottom = value; } }
        private float rateBottom;


        public int Amount { get { return amount; } private set { amount = value; } }
        private int amount;
        public int Staffed { get { return staffed; } private set { staffed = value; } }
        private int staffed;

        public Ore (OreData _oreData) {
            data = _oreData;
            rate = _oreData.rateStart;
            rateCap = data.rateCap;
            rateBottom = data.RateBottom;
            amount = 0;
            staffed = 0;
        }

        public void IncreaseRate() {
            if (rate < RateCap) {
                rate += 0.01f;
            } else {
                rate = RateCap;
            }
            rate = (float)Math.Round(rate, 2);
        }

        public void AdjustOreCap(float oreCap) {
            rateCap += oreCap;
        }

        public int CalculateProd(ProductionManager productionManager, CommodityData drillData) {
            int growth = productionManager.CalculateProd(drillData,staffed);
            return growth;
        }

        public void AddStaff(bool add) {
            if (add) {
                staffed++;
            } else {
                staffed--;
            }
        }

        public void MineOre(int _amount) {
            amount += _amount;
        }

        public void LoadOre(int _amount) {
            amount -= _amount;
        }

        public float SellOre(int rateAmountInterval, int oreAmount, TextManager textManager) {
            float profit = oreAmount * rate;
            string message = "$" + profit + " made selling " + oreAmount + " " + data.title + " sold at $" + rate;
            textManager.SendMessageToChat(message,Message.MessageType.info);
            for (int i = 0; i < oreAmount / rateAmountInterval; i++) {
                rate -= 0.01f;
            }

            if (rate < RateBottom) {
                rate = RateBottom;
            }
            rate = (float)Math.Round(rate, 2);
            return profit;
        }
    }
}
