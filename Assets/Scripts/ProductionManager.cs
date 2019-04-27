using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductionManager : MonoBehaviour
{
     public GameManager gameManager;
     public int energyMod;
     public int foodMod;

     [Header("Production")]
     public CommodityStruct[] commodities;

     bool isAddingStaff;

     void Start() {
          for (int i = 0; i < commodities.Length ;i++) {
               commodities[i].commodityAmount = commodities[i].commodity.startAmount;
          }
     }

     public void StaffCheck(bool _isAddingStaff) {
          isAddingStaff = _isAddingStaff;
     }

     public void AddStaff(Commodity commodity) {
          for (int i = 0; i < commodities.Length ;i++) {
               if (commodities[i].commodity.commodityName == commodity.commodityName) {
                    if (isAddingStaff && gameManager.popSpare > 0 && commodities[i].commodityStaffed < commodities[i].commodityAmount) {
                         commodities[i].commodityStaffed++;
                         gameManager.popSpare--;
                    } else if (!isAddingStaff && commodities[i].commodityStaffed > 0) {
                         commodities[i].commodityStaffed--;
                         gameManager.popSpare++;
                    }
                    commodities[i].productionLabel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = commodities[i].commodityStaffed.ToString();
               }
          }
     }

     public int CalculateProd(Commodity.CommodityType commodityType) {
          int growth = 0;
          for (int i = 0; i < commodities.Length ;i++) {
               if ((commodities[i].commodity.commodityType == commodityType && !gameManager.inEnergyDebt) || commodities[i].commodity.commodityType == commodityType && commodities[i].commodity.commodityType == Commodity.CommodityType.Energy) {
                    growth += (commodities[i].commodity.value * commodities[i].commodityStaffed) + ((commodities[i].commodity.value * commodities[i].commodityStaffed) * (commodities[i].commodityMod / 100));
               }
          }
          return growth;
     }
     
     public void WriteValues() {
          for (int i = 0; i < commodities.Length ;i++) {
               commodities[i].productionLabel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = commodities[i].commodityAmount.ToString();
          }
     }
}

[System.Serializable]
public struct CommodityStruct
{    
     public string Name;
     public Commodity commodity;
     [HideInInspector] public int commodityMod;
     [HideInInspector] public int commodityAmount;
     [HideInInspector] public int commodityStaffed;
     public GameObject productionLabel;
}