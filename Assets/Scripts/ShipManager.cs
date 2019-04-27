using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipManager : MonoBehaviour {

    public GameManager gameManager;

    ProductionManager productionManager;
    TextManager textManager;

    [Header("Status")]
    [HideInInspector] public int shipStatus;
    [HideInInspector] public int shipDayCheck;
    [HideInInspector] public int shipMonthCheck;
    [HideInInspector] public int shipYearCheck;
    [HideInInspector] public int shipStorage;
    public int shipCap;

    [HideInInspector] public int foodBought;
    public GameObject foodBoughtObject;
    [HideInInspector] public int energyBought;
    public GameObject energyBoughtObject;
    [HideInInspector] public int popBought;
    public GameObject popBoughtObject;
    [HideInInspector] public int popSold;
    public GameObject popSoldObject;

    [Header("Ship Status Panels")]
    public GameObject shipTransport;
    public GameObject OnMarsButtons;
    public GameObject OnEarthButtons;
    

    [Header("Ship Capacity")]
    public GameObject dial;
    public TMP_Text shipStorageNum;
    public TMP_Text shipCapNum;

    int upgradeStorageCost;

    public TMP_Text shipStatusTxt;
    public GameObject shipBtnObject;

    public ShipInventory[] shipInventory;
    int lastIncrement;

    void Start() {
        productionManager = gameManager.productionManager;
        textManager = gameManager.textManager;
        
        upgradeStorageCost = shipCap / 10;

        shipStatus = 2;
        UpdateShipValues();
    }

    public void UpdateShipValues() {
        //Write Ship Values to UI
        if (shipStatus == 0) {
            shipStatusTxt.text = "On Earth";
            shipTransport.SetActive(false);
            OnMarsButtons.SetActive(false);
            OnEarthButtons.SetActive(true);
        } else if (shipStatus == 1) {
            shipStatusTxt.text = "Mars Bound";
            shipTransport.transform.rotation = new Quaternion(0,180,0,0);
            shipTransport.SetActive(true);
            OnMarsButtons.SetActive(false);
            OnEarthButtons.SetActive(false);
        } else if (shipStatus == 2) {
            shipStatusTxt.text = "On Mars";
            shipTransport.SetActive(false);
            OnMarsButtons.SetActive(true);
            OnEarthButtons.SetActive(false);
        } else {
            shipStatusTxt.text = "Earth Bound";
            shipTransport.SetActive(true);
            OnMarsButtons.SetActive(false);
            OnEarthButtons.SetActive(false);
            shipTransport.transform.rotation = new Quaternion(0,0,0,0);
        }
        IncrementshipCap();
        shipStorageNum.text = shipStorage.ToString();
        shipCapNum.text = "/" + shipCap.ToString();
    }

    public void CheckShipStatus() {      
        if (shipStatus == 1 || shipStatus == 3) {
            if (shipDayCheck == gameManager.day && shipMonthCheck == gameManager.month && shipYearCheck == gameManager.year) {
                if (shipStatus == 1) {
                    shipStatus = 2;
                    gameManager.food += foodBought;
                    foodBought = 0;
                    gameManager.energy += energyBought;
                    energyBought = 0;
                    gameManager.popAmount += popBought;
                    gameManager.popSpare +=popBought;
                    popBought = 0;
                    for (int i = 0; i < shipInventory.Length ;i++) {
                        for (int x = 0; x < productionManager.commodities.Length ;x++) {
                            if (productionManager.commodities[x].commodity.commodityName == shipInventory[i].commodity.commodityName) {
                                productionManager.commodities[x].commodityAmount += shipInventory[i].itemBought;
                                shipInventory[i].itemBought = 0;
                            }
                        }
                    }
                    shipStorage = 0;
                    textManager.SendMessageToChat("The ship has arrived on mars", Message.MessageType.info);
                    shipBtnObject.SetActive(true);
                } else {
                    shipStatus = 0;
                    textManager.SendMessageToChat("The ship has arrived on Earth", Message.MessageType.info);
                    gameManager.popAmount -= popSold;
                    popSold = 0;
                    if (gameManager.oreOnShip > 0) {
                        textManager.SendMessageToChat(gameManager.oreOnShip + " ore sold at $" + gameManager.oreRate, Message.MessageType.info);
                    }
                    gameManager.money += Mathf.RoundToInt(gameManager.oreOnShip * gameManager.oreRate);
                    gameManager.oreOnShip = 0;
                    shipStorage = 0;
                    shipBtnObject.SetActive(true);
                }
            }
        }
        for (int i = 0; i < shipInventory.Length;i++) {
            shipInventory[i].itemBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = shipInventory[i].itemBought.ToString();
        }
        foodBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = foodBought.ToString();
        energyBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = energyBought.ToString();
        popBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = popBought.ToString();
        popSoldObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = popSold.ToString();
    }
    
    public void BuyCommodity(Commodity commodity) {
        if (shipStatus == 0) {
            for (int i = 0; i < shipInventory.Length ;i++) {
                if (commodity.commodityName == shipInventory[i].commodity.commodityName) {
                    if (shipInventory[i].commodity.cost <= gameManager.money && shipInventory[i].commodity.storage <= (shipCap - shipStorage)) {
                        shipInventory[i].itemBought++;
                        gameManager.money -= shipInventory[i].commodity.cost;
                        shipStorage += shipInventory[i].commodity.storage;
                    if (textManager.LastMessageInChat() == lastIncrement +" "+shipInventory[i].commodity.storageSucess) {
                        lastIncrement++;
                        textManager.ReplaceMessageInChat(lastIncrement +" "+shipInventory[i].commodity.storageSucess);
                    } else {
                        textManager.SendMessageToChat(1 +" "+shipInventory[i].commodity.storageSucess, Message.MessageType.info);
                        lastIncrement = 1;
                    }
                    } else if (shipInventory[i].commodity.cost > gameManager.money) {
                        gameManager.textManager.SendMessageToChat(shipInventory[i].commodity.storageFailMoney, Message.MessageType.info);
                    } else if (shipInventory[i].commodity.storage > (shipCap - shipStorage)) {
                        gameManager.textManager.SendMessageToChat(shipInventory[i].commodity.storageFailCap, Message.MessageType.info);
                    }
                    shipInventory[i].itemBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = shipInventory[i].itemBought.ToString();
                }
            }
        }
    }

    public void BuyFood() {
        if (shipStatus == 0) {
            if (gameManager.foodCost <= gameManager.money && gameManager.foodCost <= (shipCap - shipStorage)) {
                foodBought++;
                gameManager.money -= gameManager.foodCost;
                shipStorage += gameManager.foodCap;
                if (textManager.LastMessageInChat() == "Bought "+ lastIncrement +" Food") {
                    lastIncrement++;
                    textManager.ReplaceMessageInChat("Bought "+ lastIncrement +" Food");
                } else {
                    textManager.SendMessageToChat("Bought "+ 1 +" Food", Message.MessageType.info);
                    lastIncrement = 1;
                }
            } else if (gameManager.foodCost > gameManager.money) {
                textManager.SendMessageToChat("Not Enough Money", Message.MessageType.info);
            } else if (gameManager.foodCap > (shipCap - shipStorage)) {
                textManager.SendMessageToChat("Not Enough Space", Message.MessageType.info);
            }
        }
        foodBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = foodBought.ToString();
    }

    public void BuyEnergy() {
        if (shipStatus == 0) {
            if (gameManager.energyCost <= gameManager.money && gameManager.energyCost <= (shipCap - shipStorage)) {
                energyBought++;
                gameManager.money -= gameManager.energyCost;
                shipStorage += gameManager.energyCap;
                if (textManager.LastMessageInChat() == "Bought "+ lastIncrement +" Energy") {
                    lastIncrement++;
                    textManager.ReplaceMessageInChat("Bought "+ lastIncrement +" Energy");
                } else {
                    textManager.SendMessageToChat("Bought "+ 1 +" Energy", Message.MessageType.info);
                    lastIncrement = 1;
                }
            } else if (gameManager.energyCap > gameManager.money) {
                textManager.SendMessageToChat("Not Enough Money", Message.MessageType.info);
            } else if (gameManager.energyCap > (shipCap - shipStorage)) {
                textManager.SendMessageToChat("Not Enough Space", Message.MessageType.info);
            }
        }
        energyBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = energyBought.ToString();
    }

    public void HireStaff(bool isHiring) {
        if (shipStatus == 0 || shipStatus == 2) {
            if (gameManager.pop.storage <= (shipCap - shipStorage)) {
                if (isHiring) {
                    popBought++;
                    shipStorage += gameManager.pop.storage;
                if (textManager.LastMessageInChat() == lastIncrement +" Staff Hired") {
                    lastIncrement++;
                    textManager.ReplaceMessageInChat(lastIncrement +" Staff Hired");
                } else {
                    textManager.SendMessageToChat(1 +" Staff Hired", Message.MessageType.info);
                    lastIncrement = 1;
                }
                } else if (!isHiring && gameManager.popSpare > 0 && gameManager.popAmount > 1) {
                    textManager.SendMessageToChat(gameManager.pop.storageFailMoney, Message.MessageType.info);
                    popSold++;
                    shipStorage += gameManager.pop.storage;
                } else if (!isHiring && gameManager.popSpare <= 0) {
                    textManager.SendMessageToChat("No spare population", Message.MessageType.info);
                } else if (!isHiring && gameManager.popAmount <= 1) {
                    textManager.SendMessageToChat("You can't fire yourself", Message.MessageType.info);
                }
            } else if (gameManager.pop.storage > (shipCap - shipStorage)) {
                textManager.SendMessageToChat(gameManager.pop.storageFailCap, Message.MessageType.info);
            }
        }
        popBoughtObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = popBought.ToString();
    }

    void IncrementshipCap() {
        int tempcap = shipCap / 52;
        if (shipStorage > 0) {
            float f = Mathf.InverseLerp(0,shipCap,shipStorage);
            f = Mathf.Lerp(0,104,f);
            dial.GetComponent<RectTransform>().anchoredPosition = new Vector2(f + 1.5f, dial.GetComponent<RectTransform>().anchoredPosition.y);
        } else { 
            dial.GetComponent<RectTransform>().anchoredPosition = new Vector2(1.5f, dial.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    public void UpgradeStorage() {
        if (gameManager.money >= upgradeStorageCost) {
            shipCap = shipCap + 1000;
            gameManager.money -= upgradeStorageCost;
            textManager.SendMessageToChat("Storage Upgraded", Message.MessageType.info);
            upgradeStorageCost = shipCap / 10;
        } else {
            textManager.SendMessageToChat("Not Enough Money", Message.MessageType.info);
        }
    }

    public void GetShip() {
        if (shipStatus == 0) {
            shipStatus = 1;
            ShipDateToArrive();
            string shipstring = "Ship on its way to mars it will arrive on " + shipDayCheck + "/" + shipMonthCheck + "/" + shipYearCheck;
            textManager.SendMessageToChat(shipstring, Message.MessageType.info);
            shipBtnObject.SetActive(false);
        } else if (shipStatus == 2) {
            shipStatus = 3;
            ShipDateToArrive();
            string shipstring = "Ship on its way to earth it will arrive on " + shipDayCheck + "/" + shipMonthCheck + "/" + shipYearCheck;
            textManager.SendMessageToChat(shipstring, Message.MessageType.info);
            shipBtnObject.SetActive(false);
        }
    }

    void ShipDateToArrive() {
        shipDayCheck = gameManager.day;
        if (gameManager.month > 9) {
            shipMonthCheck = (gameManager.month + 3) % 12;
            shipYearCheck = gameManager.year + 1;
        } else {
            shipMonthCheck = gameManager.month + 3;
            shipYearCheck = gameManager.year + 0;
        }
    }
    void EnableButtons(bool enable) {
        for (int i = 0; i < shipInventory.Length ;i++) {
                shipInventory[i].itemBoughtObject.GetComponent<Button>().interactable = enable;
        }
        foodBoughtObject.GetComponent<Button>().interactable = enable;
        energyBoughtObject.GetComponent<Button>().interactable = enable;
        popBoughtObject.GetComponent<Button>().interactable = enable;
    }
}

[System.Serializable]
public struct ShipInventory {
    public string Name;
    public Commodity commodity;
    [HideInInspector] public int itemBought;
    public GameObject itemBoughtObject;
}