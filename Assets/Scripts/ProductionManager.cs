using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {

     public class ProductionManager : MonoBehaviour
     {
          public GameManager gameManager;
          [HideInInspector] public int energyMod;
          [HideInInspector] public int foodMod;

          public GameObject foodProductionLocation;
          public GameObject energyProductionLocation;
          public GameObject researchProductionLocation;
          public GameObject oreProductionLocation;

          [Header("Production")]
          public GameObject producionPrefab;
          Dictionary<string, int> productionCount;
          [HideInInspector] public List<Commodity> commodities;

          bool isAddingStaff;

          public void CreateDatabase() {
               productionCount = new Dictionary<string, int>();
               productionCount.Add("Food",0);
               productionCount.Add("Energy",0);
               productionCount.Add("Ore",0);
               productionCount.Add("Research",0);
               commodities = new List<Commodity>();
          }

          public Commodity GetCommodity(CommodityData commodityData) {
               Commodity commodity = null;
               for (int i = 0; i < commodities.Count; i++) {
                    if (commodityData.commodityName == commodities[i].commodityData.commodityName) { 
                         commodity = commodities[i];
                    }
               }
               if (commodity != null) {
                    return commodity;
               }
               return null;
               }

          public void StaffCheck(bool _isAddingStaff) {
               isAddingStaff = _isAddingStaff;
          }

          public void AddStaff(CommodityData commodity) {
               for (int i = 0; i < commodities.Count ;i++) {
                    if (commodities[i].commodityData.commodityName == commodity.commodityName) {
                         if (isAddingStaff && gameManager.popSpare > 0 && commodities[i].staffed < commodities[i].amount) {
                              commodities[i].staffed++;
                              gameManager.popSpare--;
                         } else if (!isAddingStaff && commodities[i].staffed > 0) {
                              commodities[i].staffed--;
                              gameManager.popSpare++;
                         }
                         commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = commodities[i].staffed.ToString();
                    }
               }
          }

          //takes in a ship item and loops through the list of commodities then adds the bought amount then clears the shipitem.
          public void AddCommodity(ShipItem shipItem) {
               for (int x = 0; x < commodities.Count; x++) {
                    if (commodities[x].commodityData.commodityName == shipItem.commodity.commodityName) {
                         commodities[x].amount += shipItem.amount;
                         shipItem.amount = 0;
                         if (commodities[x].commodityData.commodityType == CommodityData.CommodityType.Ore) {
                              gameManager.oreManager.AddMiningDrill(commodities[x].commodityData);
                         }
                         gameManager.mapManager.AddCommodity(commodities[x]);
                    }
               }
          }

          //Creates a Unique Commodity entity.
          public void CreateCommodity(CommodityData commodityData) {
               Commodity commodity = new Commodity(commodityData, commodityData.startAmount, 0,0);
               if (commodityData.commodityType == CommodityData.CommodityType.Food) {
                    commodity.productionLabel = Instantiate(producionPrefab, new Vector2(0,-productionCount["Food"]*20),Quaternion.identity);
                    commodity.productionLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = commodity.commodityData.name + ":";
                    commodity.productionLabel.transform.SetParent(foodProductionLocation.transform,false);
                    productionCount["Food"] += 1;

               } else if (commodityData.commodityType == CommodityData.CommodityType.Energy) {
                    commodity.productionLabel = Instantiate(producionPrefab, new Vector2(0,-productionCount["Energy"]*20),Quaternion.identity);
                    productionCount["Energy"] += 1;
                    commodity.productionLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = commodity.commodityData.name + ":";
                    commodity.productionLabel.transform.SetParent(energyProductionLocation.transform,false);

               } else if (commodityData.commodityType == CommodityData.CommodityType.Ore) {
                    commodity.productionLabel = oreProductionLocation;
                    productionCount["Ore"] += 1;

               } else if (commodityData.commodityType == CommodityData.CommodityType.Research) {
                    commodity.productionLabel = researchProductionLocation;
                    productionCount["Research"] += 1;
               }

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate() { StaffCheck(true); });
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate() { AddStaff(commodity.commodityData); });

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate() { StaffCheck(false); });
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate() { AddStaff(commodity.commodityData); });

               gameManager.mapManager.AddCommodity(commodity);
               commodities.Add(commodity);
          }

          public int CalculateProd(CommodityData.CommodityType commodityType) {
               int growth = 0;
               for (int i = 0; i < commodities.Count ;i++) {
                    if ((commodities[i].commodityData.commodityType == commodityType && !gameManager.inEnergyDebt) || commodities[i].commodityData.commodityType == commodityType && commodities[i].commodityData.commodityType == CommodityData.CommodityType.Energy) {
                         growth += (commodities[i].commodityData.value * commodities[i].staffed) + ((commodities[i].commodityData.value * commodities[i].staffed) * (commodities[i].modifier / 100));
                    }
               }
               return growth;
          }
          
          public void WriteValues() {
               for (int i = 0; i < commodities.Count ;i++) {
                    commodities[i].productionLabel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = commodities[i].amount.ToString();
               }
          
          }
     }

     public class Commodity
     {    
          public CommodityData commodityData;
          [HideInInspector] public int amount;
          [HideInInspector] public int staffed;
          [HideInInspector] public int modifier;
          public GameObject productionLabel;

          public Commodity (CommodityData _commodityData, int _amount, int _staffed, int _modifier) {
               commodityData = _commodityData;
               amount = _amount;
               staffed = _staffed;
               modifier = _modifier;
          }
     }
}