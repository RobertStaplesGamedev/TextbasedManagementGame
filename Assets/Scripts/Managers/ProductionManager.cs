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
                    if (commodityData.commodityName == commodities[i].data.commodityName) { 
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

          public void AddStaff(CommodityData commodityData) {
               Commodity commodity = GetCommodity(commodityData);
               if (isAddingStaff && gameManager.popSpare > 0 && commodity.Staffed < commodity.Amount) {
                    commodity.ChangeStaff(1);
                    gameManager.popSpare--;
               } else if (!isAddingStaff && commodity.Staffed > 0) {
                    commodity.ChangeStaff(-1);
                    gameManager.popSpare++;
               }
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = commodity.Staffed.ToString();
          }

          //takes in a ship item and loops through the list of commodities then adds the bought amount then clears the shipitem.
          public void AddCommodity(ShipItem shipItem) {
               Commodity commodity = GetCommodity(shipItem.data);
               commodity.ChangeAmount(shipItem.Amount);
               shipItem.ClearItem();
               if (commodity.data.resource == CommodityData.Resource.Ore && commodity.data.type == CommodityData.Type.Building) {
                    gameManager.oreManager.AddOreBuilding(commodity.data, commodity.Amount);
               }
               gameManager.mapManager.AddCommodity(commodity);
          }

          //Creates a Unique Commodity entity.
          public void CreateCommodity(CommodityData commodityData) {
               Commodity commodity = new Commodity(commodityData);
               if (commodityData.resource == CommodityData.Resource.Food) {
                    commodity.productionLabel = Instantiate(producionPrefab, new Vector2(0,-productionCount["Food"]*20),Quaternion.identity);
                    commodity.productionLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = commodity.data.name + ":";
                    commodity.productionLabel.transform.SetParent(foodProductionLocation.transform,false);
                    productionCount["Food"] += 1;

               } else if (commodityData.resource == CommodityData.Resource.Energy) {
                    commodity.productionLabel = Instantiate(producionPrefab, new Vector2(0,-productionCount["Energy"]*20),Quaternion.identity);
                    productionCount["Energy"] += 1;
                    commodity.productionLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = commodity.data.name + ":";
                    commodity.productionLabel.transform.SetParent(energyProductionLocation.transform,false);

               } else if (commodityData.resource == CommodityData.Resource.Ore) {
                    commodity.productionLabel = oreProductionLocation;
                    productionCount["Ore"] += 1;

               } else if (commodityData.resource == CommodityData.Resource.Research) {
                    commodity.productionLabel = researchProductionLocation;
                    productionCount["Research"] += 1;
               }

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate() { StaffCheck(true); });
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate() { AddStaff(commodity.data); });

               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate() { StaffCheck(false); });
               commodity.productionLabel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate() { AddStaff(commodity.data); });

               gameManager.mapManager.AddCommodity(commodity);
               commodities.Add(commodity);
          }

          public int CalculateProd(CommodityData commodityData, int staffed) {
               return GetCommodity(commodityData).CalculateProduction(gameManager, staffed);

          } public int CalculateProd(CommodityData.Resource resource) {
               int growth = 0;
               for (int i = 0; i < commodities.Count ;i++) {
                    if (commodities[i].data.resource == resource) {
                         growth += commodities[i].CalculateProduction(gameManager);
                    }
               }
               return growth;
          }
          
          public void ApplyCommodityMod(CommodityData data, int modifier) {
               Commodity commodity = GetCommodity(data);
               commodity.AddModifier(modifier);
          }

          public void WriteValues() {
               for (int i = 0; i < commodities.Count ;i++) {
                    commodities[i].productionLabel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = commodities[i].Amount.ToString();
               }
          
          }
     }

     public class Commodity
     {    
          public CommodityData data;

          public int Production { get { return production; } private set { production = value; } }
          int production;

          public int Amount { get { return amount; } private set { amount = value; } }
          int amount;

          public int Staffed { get { return staffed; } private set { staffed = value; } }
          int staffed;

          public float Modifier { get { return modifier; } private set { modifier = value; } }
          float modifier;

          public GameObject productionLabel;

          public Commodity (CommodityData _commodityData) {
               data = _commodityData;
               amount = data.startAmount;
               production = data.value;
               staffed = 0;
               modifier = 100;
          }

          public int CalculateProduction(GameManager gameManager, int _staffed) {
               return CalculateProd(gameManager, _staffed);
          } public int CalculateProduction(GameManager gameManager) {
               return CalculateProd(gameManager, Staffed);
          }

          int CalculateProd(GameManager gameManager, int _staffed) {
               float growth = 0;
               if (!gameManager.inEnergyDebt || data.resource == CommodityData.Resource.Energy) {
                    growth += (Production * Staffed) * (modifier / 100);
               }
               return Mathf.RoundToInt(growth);
          }

          public void IncreaseProduction() {
               production += data.value;
          }
          public void ChangeAmount(int amountAdded) {
               amount += amountAdded;
          }
          public void ChangeStaff(int staff) {
               staffed += staff;
          }
          public void ClearStaff() {
               staffed = 0;
          }
          public void AddModifier(float _modifier) {
               modifier += _modifier;
          }
     }
}