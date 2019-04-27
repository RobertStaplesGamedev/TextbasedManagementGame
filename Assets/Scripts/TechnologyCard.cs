using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="New Technology")]
public class TechnologyCard : ScriptableObject
{
    public string techName;
    public string descrption;
    public Sprite techSprite;
    public int researchCost;

    public enum TechType {Commodity,CommodityModifier,RescourceModifier, Event}
    public TechType techType;
    public Commodity commodity;
    public int efficencymod;
    public TechnologyCard[] DependantTech;

}