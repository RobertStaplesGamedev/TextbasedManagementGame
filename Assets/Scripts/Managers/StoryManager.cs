using System;
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
        public GameObject stormPanel;

        public EventData[] eventData;
        [HideInInspector] public List<Effect> activeEffects;

        void FixedUpdate() {
            for (int i = 0; i < events.Length; i++) {
                if (events[i].data.dependantEvent != null) {
                    for (int j = 0; j < events.Length; j++) {
                        if (events[i].data.dependantEvent == events[j].data && events[j].triggered) {
                            events[i].TriggerEvent(gameManager);
                        }
                    }
                } else {
                    events[i].TriggerEvent(gameManager);
                }
            }
            EndEffectCheck();
        }

        public void CreateDatabase() {
            events = new Event[eventData.Length];
            for (int i = 0; i < eventData.Length; i++) {
                events[i] = CreateEvent(eventData[i]);
            }
            activeEffects = new List<Effect>();

            for (int i = 0; i < events.Length; i++) {
                if (events[i].data.dependantEvent == null) {
                    events[i].TriggerEvent(gameManager);
                }
            }
        }

        Event CreateEvent(EventData eventData) {
            Event newEvent;
            newEvent = new Event(eventData);

            return newEvent;
        }
        public void ActivatePanel(string panelID, bool setActive) {
            if (panelID == "Tech") {
                techPanel.SetActive(setActive);
            } else if (panelID == "Ore") {
                orePanel.SetActive(setActive);
            } else if (panelID == "Ship") {
                shipPanel.SetActive(setActive);
            } else if (panelID == "Mining Pane") {
                miningPanel.SetActive(setActive);
            } else if (panelID == "Storm Panel") {
                stormPanel.SetActive(setActive);
            }
        }

        public void EndEffectCheck() {
            for (int i = 0; i < activeEffects.Count; i++) {
                if (activeEffects[i].endDay == gameManager.day && activeEffects[i].endMonth == gameManager.month && activeEffects[i].endYear == gameManager.year) {
                    activeEffects[i].EndEvent(gameManager);
                }
            }
        }

    }
    public class Event {

        public EventData data;
        public bool triggered = false;
        GameManager gameManager;

        public Event(EventData _data) {
            data = _data;
        }

        public void TriggerEvent(GameManager gameManager) {
            if (!triggered) {
                if (data.triggerType == EventData.TriggerType.None) {
                    Trigger(gameManager);
                } else if (EventData.TriggerType.Time == data.triggerType && data.day == gameManager.day && data.month == gameManager.month && data.year == gameManager.year) {
                    Trigger(gameManager);
                } else if (EventData.TriggerType.Resource == data.triggerType) {
                    int triggerValue = 0;
                    if (data.resource == EventData.Resource.Food) {
                        triggerValue = gameManager.food;
                    } else if (data.resource == EventData.Resource.Energy) {
                        triggerValue = gameManager.energy;
                    } else if (data.resource == EventData.Resource.Money) {
                        triggerValue = gameManager.money;
                    } else if (data.resource == EventData.Resource.Population) {
                        triggerValue = gameManager.popAmount;
                    }
                    if (data.greaterThan && triggerValue > data.resourceValue) {
                        Trigger(gameManager);
                    } else if (!data.greaterThan && triggerValue <= data.resourceValue) {
                        Trigger(gameManager);
                    }
                } else if (data.triggerType == EventData.TriggerType.Research) {
                    if (gameManager.techManager.technologies == null) {
                        gameManager.techManager.CreateDatabase();
                    }
                    for (int i = 0; i < gameManager.techManager.technologies.Length; i++) {
                        if (gameManager.techManager.technologies[i].techCard == data.technology) {
                            //Debug.Log(gameManager.techManager.technologies[i].researched);
                            if (gameManager.techManager.technologies[i].researched) {
                                //Debug.Log(eventData);
                                Trigger(gameManager);
                            }
                            break;
                        }
                    }
                } else if (data.triggerType == EventData.TriggerType.ShipLocation) {
                    if (data.shipLocation == gameManager.shipManager.shipStatus) {
                        Trigger(gameManager);
                    }
                } else if (data.triggerType == EventData.TriggerType.Ore) {
                    Ore ore = gameManager.oreManager.GetOre(data.oreData);
                    if (data.greaterThan && ore.Amount > data.resourceValue) {
                        Trigger(gameManager);
                    } else if (!data.greaterThan && ore.Amount <= data.resourceValue) {
                        Trigger(gameManager);
                    }
                }
                
            }
        }

        void Trigger(GameManager gameManager) {
            for (int i = 0; i < data.effects.Count; i++) {
                new Effect(gameManager, data.effects[i]);
            }
            triggered = true;
        }
        public void DurationCheck() {

        }
    }
    public class Effect {
        public int endDay = 0;
        public int endMonth = 0;
        public int endYear = 0;
        public EventData.EffectData data;


        public EventData.EffectData.EffectType effectType;

        public Effect(GameManager gameManager, EventData.EffectData _data) {
            data = _data;
            if (data.hasDuration) {
                SetEndDate(gameManager, data.durationInDays);
                gameManager.storyManager.activeEffects.Add(this);
            }

            if (data.effectType == EventData.EffectData.EffectType.Dialouge) {
                gameManager.textManager.SendMessageToChat(data.dialougeText, data.messageType);
            }
            if (data.effectType == EventData.EffectData.EffectType.EfficencyMod) {
                gameManager.productionManager.ApplyCommodityMod(data.commodityModified, data.efficencyMod);
            } 
            else if (data.effectType == EventData.EffectData.EffectType.ActivatePanel) {
                gameManager.storyManager.ActivatePanel(data.panelID, true);
            } 
            else if (data.effectType == EventData.EffectData.EffectType.DeactivatePanel) {
                gameManager.storyManager.ActivatePanel(data.panelID, false);
            } else if (data.effectType == EventData.EffectData.EffectType.ChangeOreCap) {
                gameManager.oreManager.GetOre(data.oredata).AdjustOreCap(data.oreCap);
            }
        }
        public void EndEvent(GameManager gameManager) {
            gameManager.storyManager.activeEffects.Remove(this);
            if (data.effectType == EventData.EffectData.EffectType.EfficencyMod) {
                Debug.Log(-data.efficencyMod);
                gameManager.productionManager.ApplyCommodityMod(data.commodityModified, -data.efficencyMod);
            } 
            else if (data.effectType == EventData.EffectData.EffectType.ActivatePanel) {
                gameManager.storyManager.ActivatePanel(data.panelID, false);
            } 
            else if (data.effectType == EventData.EffectData.EffectType.DeactivatePanel) {
                gameManager.storyManager.ActivatePanel(data.panelID, true);
            } else if (data.effectType == EventData.EffectData.EffectType.ChangeOreCap) {
                gameManager.oreManager.GetOre(data.oredata).AdjustOreCap(-data.oreCap);
            }
        }

        void SetEndDate(GameManager gameManager, int duration) {
            int day = gameManager.day;
            int month = gameManager.month;
            int year = gameManager.year;

            for (int i = 0; i < duration; i++) {
                day++;
                if (day == 30) {
                    day = 1;
                    month++;
                }
                if (month == 12) {
                    month = 1;
                    year++;
                }
            }

            endDay = day;
            endMonth = month;
            endYear = year;
        }
    }
}