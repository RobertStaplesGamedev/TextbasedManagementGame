using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Commodity")]
public class Commodity : ScriptableObject
{
    public string commodityName;
    public int cost;
    public int value;
    public int storage;
    public int startAmount;

    public string storageSucess;
    public string storageFailMoney;
    public string storageFailCap;

    public enum CommodityType {Food, Money, Energy, Ore, Population, Research}
    public CommodityType commodityType;
}
