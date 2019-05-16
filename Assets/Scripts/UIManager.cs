using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {

    public class UIManager : MonoBehaviour
    {
        public GameManager gameManager;
        
        public TMP_Text dateNum;

        public GameObject foodLabel;
        public GameObject energyLabel;
        public GameObject moneyLabel;
        public GameObject populationLabel;
        public GameObject populationSpareLabel;

        public Color positiveAmount, negativeAmount;
        public Color positiveGrowth, negativeGrowth, neutralGrwoth;

        void Start() {
            //gameManager = this.transform.parent.GetComponent<GameManager>();
        }

        public void WriteValues() {
            dateNum.text = gameManager.day.ToString() + "/" + gameManager.month.ToString() + "/" + gameManager.year.ToString();

            WriteGrowth(foodLabel, gameManager.food, gameManager.foodGrowth);
            WriteGrowth(energyLabel, gameManager.energy, gameManager.energyGrowth);
            WriteGrowth(moneyLabel, gameManager.money, gameManager.income);

            populationLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = gameManager.popAmount.ToString();
            populationSpareLabel.transform.GetChild(0).GetComponent<TMP_Text>().text = gameManager.popSpare.ToString();
        }

        void WriteGrowth(GameObject label, int amount, float growth) {
            if (amount > 0) {
                label.transform.GetChild(0).GetComponent<TMP_Text>().color = positiveAmount;
            } else {
                label.transform.GetChild(0).GetComponent<TMP_Text>().color = negativeAmount;
            }
            label.transform.GetChild(0).GetComponent<TMP_Text>().text = amount.ToString();
            if (growth > 0) {
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = positiveGrowth;
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = " (+" + growth + ")";

            } else if(growth < 0) {
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = negativeGrowth;
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = " (-" + growth + ")";
            } else {
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = neutralGrwoth;
                label.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = " (+" + growth + ")";
            }
        }
    }
}