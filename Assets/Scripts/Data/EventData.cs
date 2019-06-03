using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {
    [CreateAssetMenu(menuName="Story/Event")]
    public class EventData : ScriptableObject {
        public EventData dependantEvent;
        public enum TriggerType {None, Time, Resource, Research, ShipLocation, Ore}
        public TriggerType triggerType;
        GameManager gameManager;
        
        [Header("Time")]
        public int day;
        public int month;
        public int year;

        [Header("Ore")]
        public OreData oreData;

        public enum Resource {Food,Energy,Money,Population}
        [Header("Resource")]
        public Resource resource;
        public bool greaterThan;
        public int resourceValue;

        [Header("Research")]
        public TechnologyCard technology;

        public int shipLocation;

        public List<EffectData> effects;

        [System.Serializable]
        public class EffectData {

            public enum EffectType {Dialouge, EfficencyMod, ActivatePanel, DeactivatePanel, Sickness, AddCommodityToShip, RemoveCommodityFromShip, ChangeOreCap}
            public EffectType effectType;

            public string dialougeText;
            public Message.MessageType messageType;
            public string panelID;
            
            public CommodityData commodityModified;
            public int efficencyMod;

            public OreData oredata;
            public float oreCap;

            public bool hasDuration;
            public int durationInDays;


        }        
    }
}
