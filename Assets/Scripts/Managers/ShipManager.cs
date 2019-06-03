using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {

    public class ShipManager : MonoBehaviour {

        public GameManager gameManager;

        TextManager textManager;

        [Header("Status")]
        [HideInInspector] public int shipStatus;
        [HideInInspector] public int shipDayCheck;
        [HideInInspector] public int shipMonthCheck;
        [HideInInspector] public int shipYearCheck;
        [HideInInspector] public int shipStorage;
        public int shipCap;

        [HideInInspector] public int foodBought;
        [HideInInspector] public int energyBought;
        [HideInInspector] public int popBought;
        [HideInInspector] public int popSold;
        public GameObject popSoldObject;

        [Header("Earth")]
        public GameObject foodObjects;
        int foodObjectCount = 1;
        public GameObject energyObjects;
        int energyObjectCount = 1;
        public GameObject earthMiscObjects;
        int earthMiscObjectCount = 1;

        [Header("Mars")]
        public GameObject oreObjects;
        int oreObjectsCount = 1;
        public GameObject marsMiscObjects;
        int marsMiscObjectsCount = 1;

        [Header("Ship Screen")]
        public GameObject shipScreen;
        public GameObject itemPrefab;
        public int itemCount;

        [Header("Ship Status Panels")]
        public GameObject shipTransport;
        public GameObject OnMarsButtons;
        public GameObject OnEarthButtons;
        public GameObject mask;

        [Header("Ship Capacity")]
        public GameObject dial;
        public TMP_Text shipStorageNum;
        public TMP_Text shipCapNum;

        int upgradeStorageCost;

        public TMP_Text shipStatusTxt;
        public GameObject shipBtnObject;

        public List<ShipItem> shipInventory;
        int lastIncrement;

        public GameObject shipButtonPrefab;

        public void CreateDatabase() {
            upgradeStorageCost = shipCap / 10;
            textManager = gameManager.textManager;


            shipStatus = 2;
            UpdateShipValues();
            shipInventory = new List<ShipItem>();
        }

        public void CheckShipStatus() {      
            if (shipStatus == 1 || shipStatus == 3) {
                if (shipDayCheck == gameManager.day && shipMonthCheck == gameManager.month && shipYearCheck == gameManager.year) {
                    if (shipStatus == 1) {
                        ShipLandedOnMars();
                    } else {
                        ShipLandedOnEarth();
                    }
                }
            }
        }

        void SetShipDateToArrive() {
            shipDayCheck = gameManager.day;
            if (gameManager.month > 9) {
                shipMonthCheck = (gameManager.month + 3) % 12;
                shipYearCheck = gameManager.year + 1;
            } else {
                shipMonthCheck = gameManager.month + 3;
                shipYearCheck = gameManager.year + 0;
            }
        }

        void ShipLandedOnEarth() {
            shipStatus = 0;
            textManager.SendMessageToChat("The ship has arrived on Earth", Message.MessageType.info);
            gameManager.popAmount -= popSold;
            popSold = 0;
            SellOre();
            shipStorage = 0;
            shipBtnObject.SetActive(true);
            ClearScreen();
        }

        void ShipLandedOnMars() {
            shipStatus = 2;
            gameManager.mapManager.LaunchShip(false);
            gameManager.food += foodBought;
            foodBought = 0;
            gameManager.energy += energyBought;
            energyBought = 0;
            gameManager.popAmount += popBought;
            gameManager.popSpare +=popBought;
            popBought = 0;
            for (int i = 0; i < shipInventory.Count ;i++) {
                if (shipInventory[i].Amount > 0 && shipInventory[i].data.type == CommodityData.Type.Building) {
                    gameManager.productionManager.AddCommodity(shipInventory[i]);
                }
            }
            shipStorage = 0;
            textManager.SendMessageToChat("The ship has arrived on mars", Message.MessageType.info);
            shipBtnObject.SetActive(true);
            ClearScreen();
        }

         public void Launch() {
            if (shipStatus == 0) {
                shipStatus = 1;
                SetShipDateToArrive();
                SetShipFade(true);
                string shipstring = "Ship on its way to mars it will arrive on " + shipDayCheck + "/" + shipMonthCheck + "/" + shipYearCheck;
                textManager.SendMessageToChat(shipstring, Message.MessageType.info);
                shipBtnObject.SetActive(false);
            } else if (shipStatus == 2) {
                shipStatus = 3;
                gameManager.mapManager.LaunchShip(true);
                SetShipFade(true);
                SetShipDateToArrive();
                string shipstring = "Ship on its way to earth it will arrive on " + shipDayCheck + "/" + shipMonthCheck + "/" + shipYearCheck;
                textManager.SendMessageToChat(shipstring, Message.MessageType.info);
                shipBtnObject.SetActive(false);
            }
        }

        public void CreateShipItem(CommodityData commodityData) {
            ShipItem item = new ShipItem(commodityData);
            if (item.data.OreData == null) {
                if (item.data.resource == CommodityData.Resource.Food) {
                    item.commodObject = Instantiate(shipButtonPrefab, foodObjects.transform);
                    item.commodObject.transform.position = new Vector2(foodObjects.transform.position.x, foodObjects.transform.position.y + (foodObjectCount * -30));
                    foodObjectCount++;
                } else if (item.data.resource == CommodityData.Resource.Energy) {
                    item.commodObject = Instantiate(shipButtonPrefab, energyObjects.transform);
                    item.commodObject.transform.position = new Vector2(energyObjects.transform.position.x, energyObjects.transform.position.y + (energyObjectCount * -30));
                    energyObjectCount++;
                } else {
                    item.commodObject = Instantiate(shipButtonPrefab, earthMiscObjects.transform);
                    item.commodObject.transform.position = new Vector2(earthMiscObjects.transform.position.x, earthMiscObjects.transform.position.y + (earthMiscObjectCount * -30));
                    earthMiscObjectCount++;
                }

                item.commodObject.GetComponent<Button>().onClick.RemoveAllListeners();
                item.commodObject.GetComponent<Button>().onClick.AddListener(delegate() { BuyCommodity(item.data); });
                item.commodObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Buy " + item.data.commodityName;
            } else {
                if (item.data.resource == CommodityData.Resource.Ore && item.data.type != CommodityData.Type.Building) {
                    item.commodObject = Instantiate(shipButtonPrefab, oreObjects.transform);
                    item.commodObject.transform.position = new Vector2(oreObjects.transform.position.x, oreObjects.transform.position.y + (oreObjectsCount * -30));
                    oreObjectsCount++;

                    item.commodObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    item.commodObject.GetComponent<Button>().onClick.AddListener(delegate() { LoadOre(item.data.OreData); });
                    item.commodObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Load " + item.data.commodityName;
                } else {
                    item.commodObject = Instantiate(shipButtonPrefab, marsMiscObjects.transform);
                    item.commodObject.transform.position = new Vector2(marsMiscObjects.transform.position.x, marsMiscObjects.transform.position.y + (marsMiscObjectsCount * -30));
                    marsMiscObjectsCount++;
                }
            }




            shipInventory.Add(item);
        }

        public ShipItem GetShipItem(CommodityData commodityData) {
            ShipItem shipItem = null;
            for (int i = 0; i < shipInventory.Count; i++) {
                if (commodityData == shipInventory[i].data) {
                    shipItem = shipInventory[i];
                }
            }
            return shipItem;
        }

        public void BuyCommodity(CommodityData commodity) {
            if (shipStatus == 0) {
                ShipItem item = GetShipItem(commodity);
                if (item.data.cost <= gameManager.money && item.data.storage <= (shipCap - shipStorage)) {
                    item.AddAmount(1);
                    gameManager.money -= item.data.cost;
                    shipStorage += item.data.storage;
                    if (textManager.LastMessageInChat() == lastIncrement +" "+item.data.storageSucess) {
                        lastIncrement++;
                        textManager.ReplaceMessageInChat(lastIncrement +" "+item.data.storageSucess);
                    } else {
                        textManager.SendMessageToChat(1 +" "+item.data.storageSucess, Message.MessageType.info);
                        lastIncrement = 1;
                    }
                } else if (item.data.cost > gameManager.money) {
                    textManager.SendMessageToChat(item.data.storageFailMoney, Message.MessageType.info);
                } else if (item.data.storage > (shipCap - shipStorage)) {
                    textManager.SendMessageToChat(item.data.storageFailCap, Message.MessageType.info);
                }
                //item.commodObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = item.Amount.ToString();
                ClearScreen();
                WriteToScreen();
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
            ClearScreen();
            WriteToScreen();
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
            ClearScreen();
            WriteToScreen();
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
            ClearScreen();
            WriteToScreen();
        }

        void WriteToScreen() {
            int count = 0;
            for (int i = 0; i < shipInventory.Count; i++) {
                if (shipInventory[i].Amount > 0) {
                    GameObject item = Instantiate(itemPrefab, shipScreen.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x, (-15 + count * -11));
                    item.transform.GetComponent<TMP_Text>().text = shipInventory[i].data.commodityName;
                    item.transform.GetChild(0).GetComponent<TMP_Text>().text = shipInventory[i].Amount.ToString();
                    count++;
                }
            }
        }

        void ClearScreen() {
            foreach (Transform child in shipScreen.transform) {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void LoadOreAllButton() {
            LoadOre(null);
        }

        public void LoadOreButton(OreData oreData) {
            LoadOre(oreData);
        }

        void LoadOre(OreData oreData) {
            if (shipStatus == 2 && shipStorage < shipCap) {
                int shipSpace = shipCap - shipStorage;
                (shipInventory, shipSpace) = gameManager.oreManager.LoadOre(oreData, shipInventory, shipSpace);
                shipStorage = shipCap - shipSpace;
            } else if (shipStatus != 2) {
                gameManager.textManager.SendMessageToChat("Ship not on mars", Message.MessageType.info);
            }
            ClearScreen();
            WriteToScreen();
        }

        public void SellOre() {
            for (int i = 0; i < shipInventory.Count ;i++) {
                if (shipInventory[i].data.resource == CommodityData.Resource.Ore && shipInventory[i].data.type != CommodityData.Type.Building && shipInventory[i].commodObject.activeSelf) {
                    if (shipInventory[i].Amount > 0) {
                        gameManager.oreManager.SellOre(shipInventory[i].data.OreData, shipInventory[i].Amount);
                    }
                    shipInventory[i].ClearItem();
                }
            }
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

        void SetShipFade(bool launched)
        {
            //mask.GetComponent<Animator>().SetBool("launch", launched);
            //Debug.Log(mask);
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
    }

    public class ShipItem {
        public CommodityData data;
        public int Amount { get { return amount; } private set { amount = value; } }
        int amount;
        public GameObject commodObject;

        public ShipItem (CommodityData _commodityData) {
            data = _commodityData;
            amount = 0;
        }


        public void ClearItem() {
            amount = 0;
        }

        public void AddAmount(int _amount) {
            amount += _amount;
        }

        public void SubtractAmount(int _amount) {
            amount -= _amount;
        }
    }
}