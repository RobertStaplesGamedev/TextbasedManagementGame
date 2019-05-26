using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Colony {
    [CreateAssetMenu(fileName="New Technology", menuName="Technology/Card")]
    public class TechnologyCard : ScriptableObject
    {
        public string techName;
        public string descrption;
        public Sprite techSprite;
        public bool startingTechnology;
        public int researchCost;

        public TechnologyCard[] DependantTech;

        public enum TechType {Commodity,CommodityModifier,RescourceModifier, Event}
        public TechType techType;

        [HideInInspector]public int efficencymod;
        [HideInInspector]public CommodityData commodity;
    }
}