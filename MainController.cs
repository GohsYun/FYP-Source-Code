using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using System.Transactions;

public class MainController : MonoBehaviour
{
    #region Singleton design pattern initialisation for MainController, and Awake Function, DO NOT TOUCH THIS REGION
    private static MainController _instance;
    public static MainController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MainController");
                go.AddComponent<MainController>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    #region State handling fields/variables 
    public int NumberOfExperiment;
    private int _currentExperiment;
    public int CurrentExperiment
    {
        get
        {
            return _currentExperiment;
        }
        set
        {
            _currentExperiment = value;

            for (int i = 0; i < NumberOfExperiment; i++)
            {
                if (i == value)
                {
                    UIManager.Instance.LandscapeModeExperimentButtonCollection[i].SetActive(true);
                    UIManager.Instance.PotraitModeExperimentButtonCollection[i].SetActive(true);
                }
                else
                {
                    UIManager.Instance.LandscapeModeExperimentButtonCollection[i].SetActive(false);
                    UIManager.Instance.PotraitModeExperimentButtonCollection[i].SetActive(false);
                }
            }

        }

    }

    private int _currentState;
    public int CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            StateHandlerDictionary[value]();
        }
    }
    public int PreviousState;

    public Dictionary<int, Action> StateHandlerDictionary;
    #endregion

    #region Panels GameObject
    public Text TopPanelText;
    public GameObject BottomPanel;
    public GameObject CommonPanel;
    public Text CommonPanelText;
    public GameObject SubMenu;
    public GameObject SubButton_1;
    public GameObject SubButton_2;
    public GameObject SubButton_3;
    public GameObject SubButton_4;
    #endregion

    #region MainScene fields/variables
    private ARPrefabBehaviour currentARPrefab;
    #endregion

    public void ButtonInstantiation()
    {
        //Edit this for different number of button set (start from 1)
        //Make sure it's compatible with the number of set needed for the buttons
        NumberOfExperiment = 1;

        //ButtonGenerator class recieve 3 Parameters in this specific order: 
        //experiment number (start from 0),
        //button sequence number for its corresponding experiment (start from 0), 
        //and name of the button that appears in the UI (\n for new line)
        GenerateButton(0, 0, "Operate");
        GenerateButton(0, 1, "Motor");
        GenerateButton(0, 2, "Belt");
        GenerateButton(0, 3, "Rollers");
        //GenerateButton(0, 4, "Button\nE");
        //GenerateButton(0, 5, "Button\nF");

        //GenerateButton(1, 0, "Button\nG");
        //GenerateButton(1, 1, "Button\nH");
        //GenerateButton(1, 2, "Button\nI");
        //GenerateButton(1, 3, "Button\nJ");

        //GenerateButton(2, 0, "Button\nO");
        //GenerateButton(2, 1, "Button\nP");
        //GenerateButton(2, 2, "Button\nQ");
        //GenerateButton(2, 3, "Button\nR");
        //GenerateButton(2, 4, "Button\nS");
        //GenerateButton(2, 5, "Button\nT");
        //GenerateButton(2, 6, "Button\nU");
        //GenerateButton(2, 7, "Button\nV");
    }
    public void ButtonOnClicked(Button button)
    {
        switch (button.name)
        {
            case "ButtonExp0Seq0": CurrentState = 1; break;
            case "ButtonExp0Seq1": CurrentState = 2; break;
            case "ButtonExp0Seq2": CurrentState = 3; break;
            case "ButtonExp0Seq3": CurrentState = 4; break;
            case "ButtonExp0Seq4": CurrentState = 5; break;
            case "ButtonExp0Seq5": CurrentState = 6; break;
            case "ButtonExp1Seq0": CurrentState = 7; break;
            case "ButtonExp1Seq1": CurrentState = 8; break;
            case "ButtonExp1Seq2": CurrentState = 0; break;
            case "ButtonExp1Seq3": CurrentState = 0; break;
            case "ButtonExp2Seq0": CurrentState = 0; break;
            case "ButtonExp2Seq1": CurrentState = 0; break;
            case "ButtonExp2Seq2": CurrentState = 0; break;
            case "ButtonExp2Seq3": CurrentState = 0; break;
            case "ButtonExp2Seq4": CurrentState = 0; break;
            case "ButtonExp2Seq5": CurrentState = 0; break;
            case "ButtonExp2Seq6": CurrentState = 0; break;
            case "ButtonExp2Seq7": CurrentState = 0; break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulatingStateHandlerDictionary();
        CurrentExperiment = 0;
        CurrentState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //CurrentState = stateNumber;
    }

    public void BackToPreviousStateButtonHandler()
    {
        switch (CurrentState)
        {
            case 0: CurrentState = 0; break;
            case 1: CurrentState = 0; break;
            case 2: CurrentState = 0; break;
            case 3: CurrentState = 0; break;
            case 4: CurrentState = 0; break;
            case 5: CurrentState = 0; break;
            case 6: CurrentState = 0; break;
            case 7: CurrentState = 0; break;
            case 8: CurrentState = 0; break;
            case 9: CurrentState = 0; break;
            case 10: CurrentState = 0; break;
        }
    }
    public void StateHandler0()
    {
        //CurrentExperiment = 0;
        //DeactivateAllButtonsExcept(0);
        //DeactivateAllCentrePanel();
        BottomPanel.SetActive(false);
        SubMenu.SetActive(false);
        Debug.Log("State Handler 0");
    }
    public void StateHandler1()
    {
        // Animate
        if (currentARPrefab != null)
            currentARPrefab.ToggleBeltsAndInfo();
        Debug.Log("State Handler 1");
    }
    public void StateHandler2()
    {
        // Motor
        if (currentARPrefab != null)
            currentARPrefab.SetSectionTransparency("Motor");
        Debug.Log("State Handler 2");
    }
    public void StateHandler3()
    {
        // Belt
        if (currentARPrefab != null)
            currentARPrefab.SetSectionTransparency("Belt");
        Debug.Log("State Handler 3");
    }
    public void StateHandler4()
    {
        // Rollers
        if (currentARPrefab != null)
            currentARPrefab.SetSectionTransparency("Rollers");
        Debug.Log("State Handler 4");
    }
    public void StateHandler5()
    {
        Debug.Log("State Handler 5");
    }
    public void StateHandler6()
    {
        Debug.Log("State Handler 6");

    }
    public void StateHandler7()
    {
        Debug.Log("State Handler 7");
    }
    public void StateHandler8()
    {

    }
    public void StateHandler9()
    {

    }
    public void StateHandler10()
    {

    }
    public void StateHandler11()
    {

    }
    public void StateHandler12()
    {

    }
    public void SetCurrentARPrefab(ARPrefabBehaviour prefab)
        {
            currentARPrefab = prefab;
        }
    private void PopulatingStateHandlerDictionary()
    {
        StateHandlerDictionary = new Dictionary<int, Action>();
        //StateHandlerDictionary will be used in StateFlagGenerator in order to call the StateHandler methode each time the state value updated
        MethodInfo[] methods = typeof(MainController).GetMethods();

        foreach (var method in methods)
        {
            if (method.Name.Contains("StateHandler") && method.Name != ("PopulatingStateHandlerDictionary"))
            {
                StateHandlerDictionary.Add(Int32.Parse(method.Name.Remove(0, 12)), () => method.Invoke(MainController.Instance, null));
            }
        }
    }
    private void GenerateButton(int ExperimentNumber, int SequenceNumber, string ButtonName)
    {
        if (ExperimentNumber > NumberOfExperiment - 1)
        {
            throw new System.ArgumentException("Experiment number exceeding the Number of Experiment", "ExperimentNumber");
        }
        else
            UIManager.Instance.ButtonCollection.Add(new UIManager.ButtonGenerator(ExperimentNumber, SequenceNumber, ButtonName));
    }
    private void DeactivateAllButtonsExcept(int ExceptionButtonSequenceNumber)
    {
        foreach (var EachButton in UIManager.Instance.ButtonCollection)
        {
            if (EachButton.experimentNumber == CurrentExperiment)
            {
                if (EachButton.buttonSequenceNumber == ExceptionButtonSequenceNumber)
                    EachButton.button.GetComponent<Button>().interactable = true;
                else
                    EachButton.button.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void DeactivateAllButtons()
    {
        foreach (var EachButton in UIManager.Instance.ButtonCollection)
        {
            if (EachButton.experimentNumber == CurrentExperiment)
            {
                EachButton.button.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void DeactivateAllCentrePanel()
    {
        foreach (var EachPanel in UIManager.Instance.CentrePanelList)
        {
            EachPanel.SetActive(false);
        }
    }


    /*Unused Methods
    private void SetupSubMenuUIButtonGroup(List<GameObject> buttons, List<string> texts, List<UnityAction> actions)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null) continue;

            buttons[i].SetActive(true);

            var button = buttons[i].GetComponent<Button>();
            var label = buttons[i].GetComponentInChildren<Text>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                if (i < actions.Count) button.onClick.AddListener(actions[i]);
            }

            if (label != null && i < texts.Count)
            {
                label.text = texts[i];
            }
        }

        // Show SubMenu itself
        if (SubMenu != null)
            SubMenu.SetActive(true);
    }
    private void ShowTransparencyOptions()
    {
        var buttons = new List<GameObject> { SubButton_1, SubButton_2, SubButton_3};
        var texts = new List<string> {
            "Motor",
            "Main Belt",
            "Rollers",
        };
        var actions = new List<UnityAction>
        {
            () => currentARPrefab.SetSectionTransparency("Motor"),
            () => currentARPrefab.SetSectionTransparency("Belt"),
            () => currentARPrefab.SetSectionTransparency("Rollers"),
        };

        SetupSubMenuUIButtonGroup(buttons, texts, actions);
    }
    private void ShowScaleOptions()
    {
        var scales = currentARPrefab.GetScaleFactors();
        var buttons = new List<GameObject> { SubButton_1, SubButton_2, SubButton_3, SubButton_4 };
        var texts = new List<string>
        {
            $"({scales[0]}x)",
            $"({scales[1]}x)",
            $"({scales[2]}x)",
            $"({scales[3]}x)"
        };
        var actions = new List<UnityAction>
        {
            () => currentARPrefab.SetScaleByIndex(0),
            () => currentARPrefab.SetScaleByIndex(1),
            () => currentARPrefab.SetScaleByIndex(2)
        };

        SetupSubMenuUIButtonGroup(buttons, texts, actions);
    }
    */
}

    


