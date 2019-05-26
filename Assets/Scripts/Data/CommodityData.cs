using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {
    [CreateAssetMenu(fileName="New Commodity")]
    public class CommodityData : ScriptableObject
    {
        public string commodityName;
        public int cost;
        public int value;
        public int storage;
        public int startAmount;

        public string storageSucess;
        public string storageFailMoney;
        public string storageFailCap;

        public enum Type {Building, Resource}
        public Type type;

        public enum Resource {Food, Money, Energy, Ore, Population, Research}
        public Resource resource;

        [HideInInspector] public OreData OreData;
    }
}
