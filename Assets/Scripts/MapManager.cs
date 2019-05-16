using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {

    public class MapManager : MonoBehaviour
    {
        public GameObject foodSprites;
        public GameObject energySprites;
        public GameObject oreSprites;
        public int interval = 5;
        int solarCount = 0;
        int fusionCount = 0;
        int biofuelCount = 0;
        int oreCount = 0;
        int researchCount = 0;
        public GameObject researchSprites;
        public Animator shipAnimator;
        public Animator mask;

        public void LaunchShip(bool launch) {
            shipAnimator.SetBool("launch", launch);
        }
        public void AddCommodity(Commodity commodity) {
            if (commodity.commodityData.commodityType == CommodityData.CommodityType.Food && commodity.amount > 0) {
                if (commodity.commodityData.commodityName == "Algae Farm") {
                    foodSprites.transform.GetChild(1).gameObject.SetActive(true);
                } else if (commodity.commodityData.commodityName == "Potato Farm") {
                    foodSprites.transform.GetChild(0).gameObject.SetActive(true);
                } else if (commodity.commodityData.commodityName == "Wheat Farm") {
                    foodSprites.transform.GetChild(2).gameObject.SetActive(true);
                }
            } else if (commodity.commodityData.commodityType == CommodityData.CommodityType.Energy) {
                if (commodity.commodityData.commodityName == "Solar Farm") {
                    AddMultiCommod(solarCount, commodity, energySprites.transform.GetChild(0).gameObject);
                } else if (commodity.commodityData.commodityName == "Biofuel") {
                    AddMultiCommod(biofuelCount, commodity, energySprites.transform.GetChild(1).gameObject);
                } else if (commodity.commodityData.commodityName == "Fusion") {
                    AddMultiCommod(fusionCount, commodity, energySprites.transform.GetChild(2).gameObject);
                }
            } else if (commodity.commodityData.commodityType == CommodityData.CommodityType.Ore) {
                AddMultiCommod(oreCount, commodity, oreSprites); 
            } else if (commodity.commodityData.commodityType == CommodityData.CommodityType.Research) {
                AddMultiCommod(researchCount, commodity, researchSprites);
            }
        }
        void AddMultiCommod(int count, Commodity commodity, GameObject sprites) {
            count = commodity.amount;
            int tempCount = 1;
            int childCount = 1;
            if (commodity.amount > 0) {
                sprites.transform.GetChild(0).gameObject.SetActive(true);
            }
            for (int i = 0; i < count; i++) {
                if (childCount >= sprites.transform.childCount) {
                    break;
                } else if (tempCount == interval) {
                    tempCount = 1;
                    sprites.transform.GetChild(childCount).gameObject.SetActive(true);
                    childCount++;
                } else {
                    tempCount++;
                }
            }
        }
    }
}
