using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using NodeEditorFramework.Standard;

namespace Colony {

    public class EventWindow : EditorWindow
    {
        TechnologyData database;

        int toolbarInt;
        string[] toolbarStrings = {"Settings"};
        public TechnologyCard[] technologies;
        bool test;

        int baseToolbar;
        string[] baseToolberStrings = {"Technologies", "Settings"};


        public Rect windowRect = new Rect(100, 100, 200, 200);

        //Tech Code
        string[] techNames;
        string[] descrptions;
        Sprite[] sprites;
        int[] researchCosts;
        TechnologyCard.TechType[] techTypes;

        //Commodity Code
        CommodityData[] commodities;
        string[] commodityNames;
        int[] commodityCosts;
        int[] commodityValues;
        int[] commodityStorage;
        int[] commodityStartAmount;
        string[] commoditySuccess;
        string[] commodityMoneyFail;
        string[] commodityStorageFail;
        CommodityData.Resource[] commodityTypes;
        int[] techIndex;
        int[] commodIndex;

        //Commodity Modifier
        int[] modifiers;
        TechnologyCard[,] depTechnologies;

        float buttonWidth = 150;
        float propertyLength = 500;

        void Awake() {
            database = (TechnologyData)AssetDatabase.LoadAssetAtPath("Assets/Data/Technology/TechnologyDatabase.asset", typeof(TechnologyData));
            technologies = database.Technologies;
            PopulateSelectionGrid();
        }

        [MenuItem("Window/Event Manger")]
        static void Init()
        {
            EventWindow window = (EventWindow)EditorWindow.GetWindow(typeof(EventWindow));
            window.Show();
        }

        void OnGUI()
        {
            baseToolbar = GUILayout.Toolbar(baseToolbar, baseToolberStrings);
            if (baseToolbar == 0) {
                BeginWindows();
                //windowRect = GUILayout.Window(1, windowRect, NodeEditorWindow.Ope, "Hi There");
                EndWindows();
            }
            else {
                // GUILayout.BeginArea();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                toolbarInt = GUILayout.SelectionGrid(toolbarInt, toolbarStrings,1, GUILayout.Width(buttonWidth + 20));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                if (toolbarInt == 0) {
                    EditorGUI.BeginChangeCheck();
                        database = (TechnologyData)EditorGUILayout.ObjectField("Database:", database, typeof(TechnologyData), allowSceneObjects: false, GUILayout.Width(propertyLength));
                    if (EditorGUI.EndChangeCheck()) {
                        technologies = database.Technologies;
                    }
                    ScriptableObject target = this;
                    SerializedObject so = new SerializedObject(target);
                    SerializedProperty stringsProperty = so.FindProperty("technologies");
                    EditorGUILayout.PropertyField(stringsProperty, true, GUILayout.Width(propertyLength));
                    so.ApplyModifiedProperties();

                    EditorGUILayout.BeginHorizontal();
                        // if (GUILayout.Button("Load")) {
                        //     technologies = database.Technologies;
                        // }
                        if (GUILayout.Button("Save And Update", GUILayout.Width(buttonWidth))) {
                            database.Technologies = technologies;
                            PopulateSelectionGrid();
                        }
                        if (GUILayout.Button("Undo", GUILayout.Width(buttonWidth))) {
                            technologies = database.Technologies;
                        }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("Update Toolbar", GUILayout.Width(buttonWidth * 2 + 5))) {
                        PopulateSelectionGrid();
                    }
                } else {
                    EditorStyles.textField.wordWrap = true;
                    for (int i = 0; i < technologies.Length; i++) {
                        PopulateTechnologies(i);
                    } 
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            void PopulateTechnologies(int i) {
                if (i + 1 == toolbarInt) {
                            techNames[i] = EditorGUILayout.TextField("Technology Name:", techNames[i], GUILayout.Width(propertyLength));
                            descrptions[i] = EditorGUILayout.TextField("Description:", descrptions[i], GUILayout.Width(propertyLength), GUILayout.MaxHeight(50));
                            sprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite:", sprites[i], typeof(Sprite), allowSceneObjects: true, GUILayout.Width(propertyLength));
                            researchCosts[i] = EditorGUILayout.IntField("Research Cost:", researchCosts[i], GUILayout.Width(propertyLength));
                            techTypes[i] = TechGUI(i);
                            
                            // Commod
                            if (techTypes[i] == TechnologyCard.TechType.Commodity) {
                                commodities[i] = (CommodityData)EditorGUILayout.ObjectField("Commodity:", commodities[i], typeof(CommodityData), allowSceneObjects: true, GUILayout.Width(propertyLength));
                                if (commodities[i] != null) {
                                    PopulateCommodities(i);
                                }
                            } else if (techTypes[i] == TechnologyCard.TechType.CommodityModifier) {
                                PopulateCommodities(i);
                                modifiers[i] = EditorGUILayout.IntField("Efficency Modifier:", modifiers[i], GUILayout.Width(propertyLength));
                            }


                            //Commod name
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Save", GUILayout.Width(248))) {
                                technologies[i].techName = techNames[i];
                            }
                            if (GUILayout.Button("Undo", GUILayout.Width(248))) {
                                AssignToTempVariables(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
            }
        }
        void PopulateCommodities(int i) {
            commodityNames[i] = EditorGUILayout.TextField("Commodity Name:", commodityNames[i], GUILayout.Width(propertyLength));
            commodityTypes[i] = CommodityGUI(i);
            commodityCosts[i] = EditorGUILayout.IntField("Commodity Cost:", commodityCosts[i], GUILayout.Width(propertyLength));
            commodityValues[i] = EditorGUILayout.IntField("Commodity Value:", commodityValues[i], GUILayout.Width(propertyLength));
            commodityStorage[i] = EditorGUILayout.IntField("Commodity Storage:", commodityStorage[i], GUILayout.Width(propertyLength));
            commodityStartAmount[i] = EditorGUILayout.IntField("Commodity Start Amount:", commodityStartAmount[i], GUILayout.Width(propertyLength));
            commoditySuccess[i] = EditorGUILayout.TextField("Success Message:", commoditySuccess[i], GUILayout.Width(propertyLength));
            commodityMoneyFail[i] = EditorGUILayout.TextField("Fail Message:", commodityMoneyFail[i], GUILayout.Width(propertyLength));
            commodityStorageFail[i] = EditorGUILayout.TextField("Fail Message:", commodityStorageFail[i], GUILayout.Width(propertyLength));
        }


        void PopulateSelectionGrid() {
            string temp = toolbarStrings[0];
            toolbarStrings = new string[technologies.Length + 1];

            techNames = new string[technologies.Length];
            descrptions = new string[technologies.Length];
            sprites = new Sprite[technologies.Length];
            researchCosts = new int[technologies.Length];
            techTypes = new TechnologyCard.TechType[technologies.Length];

            techIndex = new int[technologies.Length];
            commodIndex = new int[technologies.Length];

            commodities = new CommodityData[technologies.Length];
            commodityNames = new string[technologies.Length];
            commodityCosts = new int[technologies.Length];
            commodityValues = new int[technologies.Length];
            commodityStorage = new int[technologies.Length];
            commodityStartAmount = new int[technologies.Length];
            commoditySuccess = new string[technologies.Length];
            commodityMoneyFail = new string[technologies.Length];
            commodityStorageFail = new string[technologies.Length];
            commodityTypes = new CommodityData.Resource[technologies.Length];
            
            modifiers = new int[technologies.Length];
            depTechnologies = new TechnologyCard[technologies.Length, 10];

            toolbarStrings[0] = "Settings";
            for (int i = 0; i < technologies.Length; i++) {
                toolbarStrings[i+1] = technologies[i].techName;
                AssignToTempVariables(i);
            }
        }

        void Save(int i) {
            techNames[i] = technologies[i].techName;
            descrptions[i] = technologies[i].descrption;
            sprites[i] = technologies[i].techSprite;
            researchCosts[i] = technologies[i].researchCost;
            techIndex[i] = (int)technologies[i].techType;
            techTypes[i] = technologies[i].techType;

            if (technologies[i].commodity != null) {
                commodities[i] = technologies[i].commodity;
                commodityNames[i] = technologies[i].commodity.commodityName;
                commodityCosts[i] = technologies[i].commodity.cost;
                commodityValues[i] = technologies[i].commodity.value;
                commodityStorage[i] = technologies[i].commodity.storage;
                commodityStartAmount[i] = technologies[i].commodity.startAmount;
                commoditySuccess[i] = technologies[i].commodity.storageSucess;
                commodityMoneyFail[i] = technologies[i].commodity.storageFailMoney;
                commodityStorageFail[i] = technologies[i].commodity.storageFailCap;
                commodIndex[i] = (int)technologies[i].commodity.resource;
                commodityTypes[i] = technologies[i].commodity.resource;
            }

            //Commodity Modifier
            modifiers[i] = technologies[i].efficencymod;
            for (int x = 0; x < technologies[i].DependantTech.Length; x++) {
                depTechnologies[i,x] = technologies[i].DependantTech[x];
            }
        }

        void AssignToTempVariables(int i) {
            techNames[i] = technologies[i].techName;
            descrptions[i] = technologies[i].descrption;
            sprites[i] = technologies[i].techSprite;
            researchCosts[i] = technologies[i].researchCost;
            techIndex[i] = (int)technologies[i].techType;
            techTypes[i] = technologies[i].techType;

            if (technologies[i].commodity != null) {
                commodities[i] = technologies[i].commodity;
                commodityNames[i] = technologies[i].commodity.commodityName;
                commodityCosts[i] = technologies[i].commodity.cost;
                commodityValues[i] = technologies[i].commodity.value;
                commodityStorage[i] = technologies[i].commodity.storage;
                commodityStartAmount[i] = technologies[i].commodity.startAmount;
                commoditySuccess[i] = technologies[i].commodity.storageSucess;
                commodityMoneyFail[i] = technologies[i].commodity.storageFailMoney;
                commodityStorageFail[i] = technologies[i].commodity.storageFailCap;
                commodIndex[i] = (int)technologies[i].commodity.resource;
                commodityTypes[i] = technologies[i].commodity.resource;
            }

            //Commodity Modifier
            modifiers[i] = technologies[i].efficencymod;
            for (int x = 0; x < technologies[i].DependantTech.Length; x++) {
                depTechnologies[i,x] = technologies[i].DependantTech[x];
            }
        }

        TechnologyCard.TechType TechGUI (int i) {
            string[] options = {"Commodity","CommodityModifier","RescourceModifier", "Event"};
            
            TechnologyCard.TechType techType;

            techIndex[i] = EditorGUILayout.Popup(techIndex[i], options, GUILayout.Width(propertyLength));
            switch (techIndex[i])
            {
                case 0:
                    techType = TechnologyCard.TechType.Commodity;
                    break;
                case 1:
                    techType = TechnologyCard.TechType.CommodityModifier;
                    break;
                case 2:
                    techType = TechnologyCard.TechType.RescourceModifier;
                    break;
                case 3:
                    techType = TechnologyCard.TechType.Event;
                    break;
                default:
                    techType = TechnologyCard.TechType.Commodity;
                    break;
            }
            return techType;

        }

        CommodityData.Resource CommodityGUI (int i) {
            string[] options = {"Food","Money","Energy", "Ore", "Population", "Research"};
            CommodityData.Resource commType;

            commodIndex[i] = EditorGUILayout.Popup(commodIndex[i], options, GUILayout.Width(propertyLength));
            switch (commodIndex[i])
            {
                case 0:
                    commType = CommodityData.Resource.Food;
                    break;
                case 1:
                    commType = CommodityData.Resource.Money;
                    break;
                case 2:
                    commType = CommodityData.Resource.Energy;
                    break;
                case 3:
                    commType = CommodityData.Resource.Ore;
                    break;
                case 4:
                    commType = CommodityData.Resource.Population;
                    break;
                case 5:
                    commType = CommodityData.Resource.Research;
                    break;
                default:
                    commType = CommodityData.Resource.Food;
                    break;
            }

            return commType;

        }
    }
}