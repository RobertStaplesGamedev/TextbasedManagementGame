using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colony {

    [CreateAssetMenu(fileName="New Ore", menuName="Ore")]
    public class OreData : ScriptableObject
    {
        public string title;
        public Sprite panelImage;
        public float rateCap = 1.0f;
        public float rateStart = 1.0f;
        public float RateBottom = 0.01f;
    }
}