using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {
    [CreateAssetMenu(menuName="Story/Event")]
    public class EventData : ScriptableObject {
        public string dialougeText;
        public bool dependancies;
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


        [Header("Effects")]
        public bool dialouge = true;
        public bool activatePanel;
        public string panelID;


    }
}
