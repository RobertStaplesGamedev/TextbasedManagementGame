using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {

    public class Panel : MonoBehaviour
    {
        Transform selectedPanel;

        public void SwitchTab(string tab) {
            for(int i =0; i < this.transform.childCount; i++) {
                if (tab == this.transform.GetChild(i).gameObject.name) {
                    this.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    selectedPanel = this.transform.GetChild(i);
                } else {
                    this.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                }
            }
            selectedPanel.SetAsLastSibling();
        }
    }
}
