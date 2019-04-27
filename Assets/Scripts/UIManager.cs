using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    
    public TMP_Text foodNum;
    public TMP_Text moneyNum;
    public TMP_Text popNum;
    public TMP_Text energyNum;
    public TMP_Text popSpareNum;


    public GameObject oreDial;
    public TMP_Text oreNum;
    public TMP_Text oreCapNum;

    public TMP_Text dateNum;

    public void WriteValues() {
        if (gameManager.foodGrowth < 0) {
            foodNum.text = gameManager.food.ToString() + " (" + gameManager.foodGrowth + ")";
        } else {
            foodNum.text = gameManager.food.ToString() + " (+" + gameManager.foodGrowth + ")";
        }
        if (gameManager.energyGrowth < 0) {
            energyNum.text = gameManager.energy.ToString() + " (" + gameManager.energyGrowth + ")";
        } else {
            energyNum.text = gameManager.energy.ToString() + " (+" + gameManager.energyGrowth + ")";
        }

        popNum.text = gameManager.popAmount.ToString();
        popSpareNum.text = gameManager.popSpare.ToString();

        if (gameManager.income < 0) {
            moneyNum.text = gameManager.money.ToString() + " (" + gameManager.income + ")";
        } else {
            moneyNum.text = gameManager.money.ToString() + " (+" + gameManager.income + ")";
        }

        UpdateOreValues(gameManager.oreCap);
        dateNum.text = gameManager.day.ToString() + "/" + gameManager.month.ToString() + "/" + gameManager.year.ToString();
        if (gameManager.money < 0) {
            moneyNum.color = Color.red;
        } else {
            moneyNum.color = Color.black;
        }
    }

    public void UpdateOreValues(int oreCap) {
        float f = Mathf.InverseLerp(0,oreCap,gameManager.oreAmount);
        f = Mathf.Lerp(-34f,34f,f);
        oreDial.GetComponent<RectTransform>().eulerAngles = new Vector3(0,0,-f);

        oreNum.text = gameManager.oreAmount.ToString();
        oreCapNum.text = "/" + oreCap.ToString();

    }

}
