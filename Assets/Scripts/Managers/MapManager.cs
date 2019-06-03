using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {

    public class MapManager : MonoBehaviour
    {
        public GameObject foodSprites;
        public GameObject energySprites;
        public GameObject drillSprites;
        public GameObject tankSprites;
        public int interval = 5;
        int solarCount = 0;
        int fusionCount = 0;
        int biofuelCount = 0;
        int drillCount = 0;
        int tankCount = 0;
        int researchCount = 0;
        public GameObject researchSprites;
        public Animator shipAnimator;
        public Animator mask;

        public void LaunchShip(bool launch) {
            shipAnimator.SetBool("launch", launch);
        }

        public void AddCommodity(Commodity commodity) {
            if (commodity.data.resource == CommodityData.Resource.Food && commodity.Amount > 0) {
                if (commodity.data.commodityName == "Algae Farm") {
                    foodSprites.transform.GetChild(1).gameObject.SetActive(true);
                } else if (commodity.data.commodityName == "Potato Farm") {
                    foodSprites.transform.GetChild(0).gameObject.SetActive(true);
                } else if (commodity.data.commodityName == "Wheat Farm") {
                    foodSprites.transform.GetChild(2).gameObject.SetActive(true);
                }
            } else if (commodity.data.resource == CommodityData.Resource.Energy) {
                if (commodity.data.commodityName == "Solar Farm") {
                    AddMultiCommod(solarCount, commodity, energySprites.transform.GetChild(0).gameObject);
                } else if (commodity.data.commodityName == "Biofuel") {
                    AddMultiCommod(biofuelCount, commodity, energySprites.transform.GetChild(1).gameObject);
                } else if (commodity.data.commodityName == "Fusion") {
                    AddMultiCommod(fusionCount, commodity, energySprites.transform.GetChild(2).gameObject);
                }
            } else if (commodity.data.resource == CommodityData.Resource.Ore) {
                if (commodity.data.commodityName == "Drill") {
                    AddMultiCommod(drillCount, commodity, drillSprites);
                } else if (commodity.data.commodityName == "Small Ore Tank") {
                    AddMultiCommod(tankCount, commodity, tankSprites);
                }
            } else if (commodity.data.resource == CommodityData.Resource.Research) {
                AddMultiCommod(researchCount, commodity, researchSprites);
            }
        }
        
        void AddMultiCommod(int count, Commodity commodity, GameObject sprites) {
            count = commodity.Amount;
            int tempCount = 1;
            int childCount = 1;
            if (commodity.Amount > 0) {
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
