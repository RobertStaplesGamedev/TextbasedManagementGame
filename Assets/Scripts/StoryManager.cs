using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour { 
    
    public Event[] events;
    public GameManager gameManager;

    void Start() {
        for (int i = 0; i < events.Length; i++) {
            events[i].SetGameManager(gameManager);
            if (!events[i].dependancies) {
                events[i].TriggerEvent();
            }
        }
    }
    void FixedUpdate() {
        for (int i = 0; i < events.Length; i++) {
            if (events[i].dependancies && events[events[i].dependantEvent].triggered) {
                events[i].TriggerEvent();
            }
        }
    }

    [System.Serializable]
    public class Event {
        public string name;
        public string dialougeText;
        [HideInInspector] public bool triggered = false;
        public bool dependancies;
        public int dependantEvent;
        public enum TriggerType {None, Time, Rescource, Research, ShipLocation}
        public TriggerType triggerType;
        GameManager gameManager;
        
        [Header("Time")]
        public int day;
        public int month;
        public int year;

        public enum Resource {Food,Energy,Money,Population,Ore}
        [Header("Resource")]
        public Resource resource;
        public bool greaterThan;
        public int resourceValue;

        [Header("Research")]
        public TechnologyCard technology;

        public int shipLocation;

        public GameObject panel;

        public void SetGameManager(GameManager _gameManager) {
            gameManager = _gameManager;
        }

        public void TriggerEvent() {
            //Check Pre-reqs
            if (!triggered) {
                if (triggerType == TriggerType.None) {
                    Trigger();
                } else if (TriggerType.Time == triggerType && day == gameManager.day && month == gameManager.month && year == gameManager.year) {
                    Trigger();
                } else if (TriggerType.Rescource == triggerType) {
                    int triggerValue = 0;
                    if (resource == Resource.Food) {
                        triggerValue = gameManager.food;
                    } else if (resource == Resource.Energy) {
                        triggerValue = gameManager.energy;
                    } else if (resource == Resource.Money) {
                        triggerValue = gameManager.money;
                    } else if (resource == Resource.Population) {
                        triggerValue = gameManager.popAmount;
                    } else if (resource == Resource.Ore) {
                        triggerValue = gameManager.oreAmount;
                    }
                    if (greaterThan && triggerValue > resourceValue) {
                        Trigger();
                    } else if (!greaterThan && triggerValue <= resourceValue) {
                        Trigger();
                    }
                } else if (triggerType == TriggerType.Research) {
                    for (int i = 0; i < gameManager.techManager.technologies.Length; i++) {
                        if (gameManager.techManager.technologies[i].techCard == technology) {
                            if (gameManager.techManager.technologies[i].researched) {
                                Trigger();
                            }
                            break;
                        }
                    }
                } else if (triggerType == TriggerType.ShipLocation) {
                    if (shipLocation == gameManager.shipManager.shipStatus) {
                        Trigger();
                    }
                }
                
            }
        }

        void Trigger() {
            if (panel != null) {
                panel.SetActive(true);
            }
            gameManager.textManager.SendMessageToChat(dialougeText, Message.MessageType.dialouge);
            triggered = true;
        }
    }
}