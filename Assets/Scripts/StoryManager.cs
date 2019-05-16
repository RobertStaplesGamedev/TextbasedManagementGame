using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Colony {

    public class StoryManager : MonoBehaviour { 

        Event[] events;
        public GameManager gameManager;

        public GameObject techPanel;
        public GameObject shipPanel;
        public GameObject orePanel;
        public GameObject miningPanel;

        public EventData[] eventData;

        public void CreateDatabase() {
            events = new Event[eventData.Length];
            for (int i = 0; i < eventData.Length; i++) {
                events[i] = CreateEvent(eventData[i]);
            }


            for (int i = 0; i < events.Length; i++) {
                if (!events[i].dependancies) {
                    events[i].TriggerEvent(gameManager);
                }
            }
        }
        void FixedUpdate() {
            for (int i = 0; i < events.Length; i++) {
                if (events[i].dependancies) {
                    for (int j = 0; j < events.Length; j++) {
                        if (events[i].dependantEvent == events[j].eventData && events[j].triggered) {
                            events[i].TriggerEvent(gameManager);
                        }
                    }
                }
            }
            for (int i = 0; i < events.Length; i++) {
                if (!events[i].dependancies) {
                    events[i].TriggerEvent(gameManager);
                }
            }
        }
        Event CreateEvent(EventData eventData) {
            Event newEvent;
            if (eventData.triggerType == EventData.TriggerType.Time) {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType, eventData.day, eventData.month, eventData.year);
            } else if (eventData.triggerType == EventData.TriggerType.Resource) {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType, eventData.resource, eventData.greaterThan, eventData.resourceValue);
            } else if (eventData.triggerType == EventData.TriggerType.Research) {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType, eventData.technology);
            } else if (eventData.triggerType == EventData.TriggerType.ShipLocation) {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType, eventData.shipLocation);
            } else if (eventData.triggerType == EventData.TriggerType.Ore) {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType, eventData.greaterThan, eventData.resourceValue);
            }
             else {
                newEvent = new Event(eventData.dialougeText, eventData.triggerType);
            }
            newEvent.dependancies = eventData.dependancies;
            if (eventData.dependancies) {
                newEvent.dependantEvent = eventData.dependantEvent;
            }

            if (eventData.dialouge) {
                newEvent.dialougeText = eventData.dialougeText;
            }
            newEvent.activatePanel = eventData.activatePanel;
            if (eventData.activatePanel) {

                newEvent.panelID = eventData.panelID;
            }
            newEvent.eventData = eventData;
            return newEvent;
        }
        public void ActivatePanel(string panelID) {
            if (panelID == "Tech") {
                techPanel.SetActive(true);
            } else if (panelID == "Ore") {
                orePanel.SetActive(true);
            } else if (panelID == "Ship") {
                shipPanel.SetActive(true);
            } else if (panelID == "Mining Pane") {
                miningPanel.SetActive(true);
            }
        }


    }
    public class Event {
        public EventData eventData;
        public string dialougeText;
        public bool triggered = false;
        public bool dependancies;
        public EventData dependantEvent;
        public EventData.TriggerType triggerType;
        GameManager gameManager;
        
        [Header("Time")]
        int day;
        int month;
        int year;

        [Header("Resource")]
        public EventData.Resource resource;
        bool greaterThan;
        int resourceValue;

        [Header("Research")]
        TechnologyCard technology;

        int shipLocation;

        public bool dialouge = true;
        public bool activatePanel;
        public string panelID;

        public Event(string _dialougeText, EventData.TriggerType _triggerType) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
        }

        public Event(string _dialougeText, EventData.TriggerType _triggerType, int _day, int _month, int _year) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
            day = _day;
            month = _month;
            year = _year;
        }

        public Event(string _dialougeText, EventData.TriggerType _triggerType, EventData.Resource _resource, bool _greaterThan, int _resourceValue) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
            resource = _resource;
            greaterThan = _greaterThan;
            resourceValue = _resourceValue;
        }

        public Event(string _dialougeText, EventData.TriggerType _triggerType, TechnologyCard _technology) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
            technology = _technology;
        }

        public Event(string _dialougeText, EventData.TriggerType _triggerType, int _shipLocation) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
            shipLocation = _shipLocation;
        }
        public Event(string _dialougeText, EventData.TriggerType _triggerType, bool _greaterThan, int _value) {
            dialougeText = _dialougeText;
            triggerType = _triggerType;
            greaterThan = _greaterThan;
            resourceValue = _value;
        }

        public void TriggerEvent(GameManager gameManager) {
            if (!triggered) {
                if (triggerType == EventData.TriggerType.None) {
                    Trigger(gameManager);
                } else if (EventData.TriggerType.Time == triggerType && day == gameManager.day && month == gameManager.month && year == gameManager.year) {
                    //Debug.Log("Time");
                    Trigger(gameManager);
                } else if (EventData.TriggerType.Resource == triggerType) {
                    int triggerValue = 0;
                    if (resource == EventData.Resource.Food) {
                        triggerValue = gameManager.food;
                    } else if (resource == EventData.Resource.Energy) {
                        triggerValue = gameManager.energy;
                    } else if (resource == EventData.Resource.Money) {
                        triggerValue = gameManager.money;
                    } else if (resource == EventData.Resource.Population) {
                        triggerValue = gameManager.popAmount;
                    }
                    if (greaterThan && triggerValue > resourceValue) {
                        Trigger(gameManager);
                    } else if (!greaterThan && triggerValue <= resourceValue) {
                        Trigger(gameManager);
                    }
                } else if (triggerType == EventData.TriggerType.Research) {
                    if (gameManager.techManager.technologies == null) {
                        gameManager.techManager.CreateDatabase();
                    }
                    for (int i = 0; i < gameManager.techManager.technologies.Length; i++) {
                        if (gameManager.techManager.technologies[i].techCard == technology) {
                            //Debug.Log(gameManager.techManager.technologies[i].researched);
                            if (gameManager.techManager.technologies[i].researched) {
                                //Debug.Log(eventData);
                                Trigger(gameManager);
                            }
                            break;
                        }
                    }
                } else if (triggerType == EventData.TriggerType.ShipLocation) {
                    if (shipLocation == gameManager.shipManager.shipStatus) {
                        Trigger(gameManager);
                    }
                } else if (triggerType == EventData.TriggerType.Ore) {
                    Ore ore = gameManager.oreManager.GetOre(eventData.oreData);
                    if (greaterThan && ore.Amount > resourceValue) {
                        Trigger(gameManager);
                    } else if (!greaterThan && ore.Amount <= resourceValue) {
                        Trigger(gameManager);
                    }
                }
                
            }
        }

        void Trigger(GameManager gameManager) {
            if (activatePanel) {
                gameManager.storyManager.ActivatePanel(panelID);
            }
            gameManager.textManager.SendMessageToChat(dialougeText, Message.MessageType.dialouge);
            triggered = true;
        }
    }
}