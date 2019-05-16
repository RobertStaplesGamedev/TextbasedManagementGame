using UnityEngine;
using System;

namespace Colony {

    [CreateAssetMenu(fileName="TechnologyData", menuName="Technology/Data")]
    public class TechnologyData : ScriptableObject
    {
        public TechnologyCard[] Technologies;
        public CommodityData[] commodities;
        public OreData[] ore;
    }
}