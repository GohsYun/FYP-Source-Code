using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;

public class UIManager : MonoBehaviour
{
    #region Singleton design pattern initialisation for UIManager, and Awake function, DO NOT TOUCH THIS REGION
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UIManager");
                go.AddComponent<UIManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        ComponentsInstantiation();
    }

    #endregion

    public RectTransform TopPanelRectTransform;
    public RectTransform BottomPanelRectTransform;
    public GameObject TopPanel;
    public GameObject BottomPanel;
    public GameObject CentrePanelCollection;
    public List<GameObject> CentrePanelList;
    public RectTransform Canvas;
    private float TopPanelWidth;
    private float TopPanelXPosition;
    private float lastScreenWidth;

    public ScrollRect LandscapeModeScrollViewScrollRect;
    public ScrollRect PotraitModeScrollViewScrollRect;
    public GameObject LandscapeModeScrollView;
    public GameObject PotraitModeScrollView;
    public GameObject LandscapeModeViewport;
    public GameObject PotraitModeViewport;
    public GameObject LandscapeModeExperimentButtonCollectionTemplate;
    public GameObject PotraitModeExperimentButtonCollectionTemplate;
    public GameObject ButtonTemplate;
    public Scrollbar LandscapeModeScrollbar;
    public Scrollbar PotraitModeScrollbar;

    public GameObject MenuPanel;
    public GameObject ResetPanel;
    public GameObject QuitPanel;
    public Button MenuButton;

    public GameObject[] LandscapeModeExperimentButtonCollection;
    public GameObject[] PotraitModeExperimentButtonCollection;

    public List<ButtonGenerator> ButtonCollection;
    // Start is called before the first frame update
    void Start()
    {
        //We need to add one so it goes through the outer if statement in the Update() atleast once
        lastScreenWidth = Screen.width + 1;

        TopPanelWidth = (Screen.width < Screen.height) ? Canvas.sizeDelta.x - 170f : Canvas.sizeDelta.y - 170f;
        TopPanelXPosition = (Screen.width < Screen.height) ? 140f : Canvas.sizeDelta.x / 2 - TopPanelWidth / 2;
        TopPanelRectTransform.sizeDelta = new Vector2(TopPanelWidth, 80f);
        TopPanelRectTransform.anchoredPosition = new Vector2(TopPanelXPosition, -110);

        CentrePanelList = new List<GameObject>();

        foreach (Transform EachPanel in CentrePanelCollection.transform)
        {
            CentrePanelList.Add(EachPanel.gameObject);
            EachPanel.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (lastScreenWidth != Screen.width)
        {
            lastScreenWidth = Screen.width;
            if (Screen.width < Screen.height) //Potrait
            {
                TopPanelWidth = (Screen.width < Screen.height) ? Canvas.sizeDelta.x - 170f : Canvas.sizeDelta.y - 170f;
                TopPanelXPosition = (Screen.width < Screen.height) ? 140f : Canvas.sizeDelta.x / 2 - TopPanelWidth / 2;
                TopPanelRectTransform.sizeDelta = new Vector2(TopPanelWidth, 80f);
                TopPanelRectTransform.anchoredPosition = new Vector2(TopPanelXPosition, -110);

                BottomPanelRectTransform.anchoredPosition = new Vector2(0, 165);

                PotraitModeScrollView.SetActive(true);
                LandscapeModeScrollView.SetActive(false);

                PotraitModeScrollbar.value = 1 - LandscapeModeScrollbar.value;

                for (int i = 0; i < MainController.Instance.NumberOfExperiment; i++)
                {
                    foreach (var EachButton in ButtonCollection)
                    {
                        if (EachButton.experimentNumber == i)
                        {
                            EachButton.button.transform.SetParent(PotraitModeExperimentButtonCollection[i].transform);
                        }
                    }
                }
            }
            else //Landscape
            {
                TopPanelWidth = (Screen.width < Screen.height) ? Canvas.sizeDelta.x - 170f : Canvas.sizeDelta.y - 170f;
                TopPanelXPosition = (Screen.width < Screen.height) ? 140f : Canvas.sizeDelta.x / 2 - TopPanelWidth / 2;
                TopPanelRectTransform.sizeDelta = new Vector2(TopPanelWidth, 80f);
                TopPanelRectTransform.anchoredPosition = new Vector2(TopPanelXPosition, -110);

                BottomPanelRectTransform.anchoredPosition = new Vector2(0, 30);

                PotraitModeScrollView.SetActive(false);
                LandscapeModeScrollView.SetActive(true);

                LandscapeModeScrollbar.value = 1 - PotraitModeScrollbar.value;

                for (int i = 0; i < MainController.Instance.NumberOfExperiment; i++)
                {
                    foreach (var EachButton in UIManager.Instance.ButtonCollection)
                    {
                        if (EachButton.experimentNumber == i)
                        {
                            EachButton.button.transform.SetParent(LandscapeModeExperimentButtonCollection[i].transform);
                        }
                    }
                }

            }
        }
    }

    #region Menu Panel and Its Button handler
    public void MenuButtonHandler()
    {
        MenuButton.interactable = false;
        MenuPanel.SetActive(true);

        foreach (var EachButton in ButtonCollection)
        {
            EachButton.button.GetComponent<Button>().interactable = false;
        }

        TopPanel.SetActive(false);
        BottomPanel.SetActive(false);
        CentrePanelCollection.SetActive(false);
    }
    public void MenuPanelCancelButtonHandler()
    {
        MenuPanel.SetActive(false);
        MenuButton.interactable = true;

        TopPanel.SetActive(true);
        CentrePanelCollection.SetActive(true);
        MainController.Instance.CurrentState = MainController.Instance.CurrentState;
    }
    public void MenuPanelResetButtonHandler()
    {
        MenuPanel.SetActive(false);
        ResetPanel.SetActive(true);
    }
    public void ResetPanelYesButtonHandler()
    {
        TopPanel.SetActive(true);
        CentrePanelCollection.SetActive(true);
        MainController.Instance.CurrentState = 0;

        MenuButton.interactable = true;
        ResetPanel.SetActive(false);
    }
    public void ResetPanelNoButtonHandler()
    {
        ResetPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }
    public void MenuPanelQuitButtonHandler()
    {
        MenuPanel.SetActive(false);
        QuitPanel.SetActive(true);
    }
    public void QuitPanelYesButtonHandler()
    {
        Application.Quit();
    }
    public void QuitPanelNoButtonHandler()
    {
        QuitPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

    #endregion

    public void ComponentsInstantiation()
    {
        //Creating an instance(allocating memory) of a list for ButtonCollection
        ButtonCollection = new List<ButtonGenerator>();

        //Calling ButtonInitialisation() to populate the button
        MainController.Instance.ButtonInstantiation();

        //Creating an instance(allocating memory) of a list for each mode of ExperimentButtonCollection
        LandscapeModeExperimentButtonCollection = new GameObject[MainController.Instance.NumberOfExperiment];
        PotraitModeExperimentButtonCollection = new GameObject[MainController.Instance.NumberOfExperiment];

        var initialPosition = new Vector3(0, 0, 0);
        var initialScale = new Vector3(1, 1, 1);

        //Creating Button Collection for each experiment in both Landscape and Potrait mode Viewport
        for (int i = 0; i < MainController.Instance.NumberOfExperiment; i++)
        {
            //Initialisation for landscape mode
            //It started with duplicating the template
            LandscapeModeExperimentButtonCollection[i] = Instantiate(LandscapeModeExperimentButtonCollectionTemplate, LandscapeModeViewport.transform);
            LandscapeModeExperimentButtonCollection[i].name = "LandscapeModeExperiment" + i + "ButtonCollection";
            LandscapeModeExperimentButtonCollection[i].GetComponent<RectTransform>().localPosition = initialPosition;
            LandscapeModeExperimentButtonCollection[i].GetComponent<RectTransform>().localScale = initialScale;

            //Count variable used to count how many button in each experiment, 
            //it will be used to calculate the proper height and width for the Button Collection
            int count = 0;

            //Set the parent for each button to the their matching experiment
            foreach (var EachButton in ButtonCollection)
            {
                if (EachButton.experimentNumber == i)
                {
                    count++;
                    EachButton.button.transform.SetParent(LandscapeModeExperimentButtonCollection[i].transform);
                    EachButton.button.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            LandscapeModeExperimentButtonCollection[i].GetComponent<RectTransform>().sizeDelta = new Vector2(0, (count * 90 + 10));

            //Initialisation for potrait mode
            PotraitModeExperimentButtonCollection[i] = Instantiate(PotraitModeExperimentButtonCollectionTemplate, PotraitModeViewport.transform);
            PotraitModeExperimentButtonCollection[i].name = "PotraitModeExperiment" + i + "ButtonCollection";
            PotraitModeExperimentButtonCollection[i].GetComponent<RectTransform>().localPosition = initialPosition;
            PotraitModeExperimentButtonCollection[i].GetComponent<RectTransform>().localScale = initialScale;
            PotraitModeExperimentButtonCollection[i].GetComponent<RectTransform>().sizeDelta = new Vector2((count * 90 + 10), 0);
        }

        //Set Experiment one as the first button collection
        LandscapeModeScrollViewScrollRect.content = LandscapeModeExperimentButtonCollection[0].GetComponent<RectTransform>();
        PotraitModeScrollViewScrollRect.content = PotraitModeExperimentButtonCollection[0].GetComponent<RectTransform>();

        //The template destroyed after creating the button collection
        Destroy(LandscapeModeExperimentButtonCollectionTemplate.gameObject);
        Destroy(PotraitModeExperimentButtonCollectionTemplate.gameObject);
        Destroy(ButtonTemplate.gameObject);
    }
    public class ButtonGenerator
    {
        public GameObject button { get; set; }
        public int experimentNumber { get; set; }
        public int buttonSequenceNumber { get; set; }

        public ButtonGenerator(int experimentNumber, int buttonSequenceNumber, string buttonText)
        {
            this.experimentNumber = experimentNumber;
            this.buttonSequenceNumber = buttonSequenceNumber;
            this.button = Instantiate(UIManager.Instance.ButtonTemplate);
            this.button.name = "ButtonExp" + this.experimentNumber + "Seq" + this.buttonSequenceNumber;

            this.button.GetComponentInChildren<Text>().text = buttonText;
            this.button.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}

