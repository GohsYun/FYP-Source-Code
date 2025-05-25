using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //added to access UI elements 
using System.IO.Ports;
using System;
using System.Threading;

public class CylinderTest : MonoBehaviour
{
    private static readonly SerialPort serialPort = new SerialPort("COM5", 57600);
    private Thread serialThread;
    private bool isRunning = true;  // Used to stop the thread when the program ends
    public GameObject cylinderParent; //Game object that we are manipulating 
    public Renderer cylinderRenderer; //The renderer of that object so that its color can be changed 
    public Text textElement; //Text Element to display 
    public Color currentColor; //Current color of the cylinder, represents the pH
    public string textInputMolarityAcid;
    public string textInputMolarityBase;
    public string textInputVolumeAcid;
    public string textInputWeightBeaker;
    public InputField molarityAcidInputField;
    public InputField molarityBaseInputField;
    public InputField volumeAcidInputField;
    public InputField weightBeakerInputField;
    public Button updateParametersButton;
    public string strReceived = ""; //recieved data from the load cell storing into a string 
    public float load_cell_value = 0; //Converting that into a float
    public float volumeAcid = 0; //Pre-designated amount of acid in the beaker
    public float molarityAcid = 0; //Pre-designated amount of concentration of the acid
    public float molarityBase = 0; //Pre-designated amount of concentration of the base
    public float weightBeaker = 0;
    public float volumeMillilitres = 0;
    public MeshRenderer meshrender;


    //Get color by using HSV value obtained from a color picker and then multiplying by 1/360 due to the range of H being from 0 to 1 in unity.
    //S and V are in percentages so can be converted into decimal with ease. 
    //Declaring all the HSV values for the pH scale.
    //Forms the Universal Indicator that will be used

    private Color ph0 = Color.HSVToRGB(1,1,1); //Red
    private Color ph1 = Color.HSVToRGB(0.072f,1,1); //Orange pH1
    private Color ph2 = Color.HSVToRGB(0.120f,1,1); //Light orange pH2
    private Color ph3 = Color.HSVToRGB(0.166f,1,1); //Yellow pH3
    private Color ph4 = Color.HSVToRGB(0.208f,1,1); //Light green pH4
    private Color ph5 = Color.HSVToRGB(0.237f,1,1); //green pH5
    private Color ph6 = Color.HSVToRGB(0.264f,1,1); //green pH6
    private Color ph7 = Color.HSVToRGB(0.333f,1,1); //green pH7
    private Color ph8 = Color.HSVToRGB(0.403f,1,1); //green pH8
    private Color ph9 = Color.HSVToRGB(0.472f,1,1); //teal pH9
    private Color ph10 = Color.HSVToRGB(0.538f,1,1); //blue pH10
    private Color ph11 = Color.HSVToRGB(0.640f,1,1); //Orange pH11
    private Color ph12 = Color.HSVToRGB(0.701f,1,1); //Orange pH12
    private Color ph13 = Color.HSVToRGB(0.737f,1,1); //Orange pH13
    private Color ph14 = Color.HSVToRGB(0.737f,1,0.33f); //Orange pH14

    //private IEnumerator ReadDataFromArduino()
    //{   
    //while (true)
    //    {
    //    string strReceived = stream.ReadLine();
    //    if (!string.IsNullOrEmpty(strReceived))
    //    {
    //        load_cell_value = float.Parse(strReceived); //convert the value into a float (Value would be in grams)
    //    }
    //    yield return new WaitForSeconds(2); // Adjust the delay as needed
    //    }
    //}

    IEnumerator ReadSerialData()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                // Check if data is available before attempting to read
                if (serialPort.BytesToRead > 0)
                {
                    string arduinoData = serialPort.ReadLine();
                    if (float.TryParse(arduinoData, out float loadCellValue))
                    {
                        load_cell_value = loadCellValue; // Update load cell value
                        Debug.Log("Data Updated Successfully.");
                    }
                }
            }
            catch (TimeoutException)
            {
                // Catch the timeout but do nothing, allowing the loop to continue
                Debug.Log("Timeout");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading from serial port: " + e.Message);
            }

            // Wait for a short period before reading again
            yield return new WaitForSeconds(1); // Adjust the delay as needed
        }
    }

    // Read data from the serial port in a separate thread
    //private ReadDataFromArduino()
    //{
    //    try
    //    {
    //        serialPort.Open(); // Open the serial port in a separate thread to avoid hanging Unity
    //        Debug.Log("Serial port opened successfully.");

    //        while (isRunning && serialPort.IsOpen)
    //        {
    //            try
    //            {
    //                strReceived = serialPort.ReadLine(); // Read data from Arduino
    //                if (!string.IsNullOrEmpty(strReceived))
    //                {
    //                    print(strReceived);
    //                    float.TryParse(strReceived, out load_cell_value); // Convert to float safely
    //                    Debug.Log("Data Received from Arduino.");
    //                }
    //                else
    //                {
    //                    Debug.Log("No Data Received from Arduino.");
    //                }
    //            }
    //            catch (TimeoutException) { }  // Handle timeout without crashing
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("Failed to open port: " + ex.Message);
    //    }
    //}

    void Start()
    {
        cylinderRenderer = GetComponent<Renderer>();  //Setting the renderer
        meshrender = cylinderRenderer.GetComponent<MeshRenderer>();

        // Start the thread for reading data from Arduino
        //serialThread = new Thread(ReadDataFromArduino);
        //serialThread.Start();
        if (!serialPort.IsOpen)
        {
            serialPort.DtrEnable = true; //Setting Data Terminal Ready to true, if connection issue occur, try disabling
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
            serialPort.ReadTimeout = 2000; // Optional: set a timeout for reading
        }
        StartCoroutine(ReadSerialData());

        //Setting the transparency to 50% so that the flask can be seen at the same time
        ph0.a = 0.50f;
        ph1.a = 0.50f;
        ph2.a = 0.50f;
        ph3.a = 0.50f;
        ph4.a = 0.50f;
        ph5.a = 0.50f;
        ph6.a = 0.50f;
        ph7.a = 0.50f;
        ph8.a = 0.50f;
        ph9.a = 0.50f;
        ph10.a = 0.50f;
        ph11.a = 0.50f;
        ph12.a = 0.50f;
        ph13.a = 0.50f;
        ph14.a = 0.50f;
    }

    // Update is called once per frame
    void Update()
    {
        float molAcid = 0;
        float molBase = 0;
        float volumeBase = 0;
        float molDifference = 0;
        float pH = 0;
        float ionValue = 0;
        float updatedVolume = 0;
        float pOH = 0;
        float OHMolarity = 0;
        float roundedPH = 0;

        //pH Calculation with volume calculation
        volumeBase = (load_cell_value / 1000) - volumeAcid - weightBeaker; //it needs to be divided by 1000 as it is currently reading in ml 0.02f is the weight of the beaker (20 grams)
        if (volumeBase < 0)
        {
            volumeBase = 0;
        }
        updatedVolume = volumeAcid + volumeBase;

        molAcid = molarityAcid * volumeAcid; //MOLARITY = MOL/VOLUME so MOL = VOLUME * MOLARITY
        molBase = molarityBase * volumeBase; //Same thing for the base 

        molDifference = molAcid - molBase; //Difference of mols

        if (molDifference == 0)
        { //equivalence point
            pH = 7;
        }
        else if (molDifference > 0)
        { //more acid than base
            ionValue = molDifference / updatedVolume;
            pH = -(Mathf.Log((ionValue), 10));
        }
        else if (molDifference < 0)
        { //more base than acid
            molDifference = -molDifference; //making it positive again 
            OHMolarity = molDifference / updatedVolume;
            pOH = -(Mathf.Log((OHMolarity), 10));
            pH = 14 - pOH; //pH + pOH = 14
        }

        //Colour change depending on pH
        roundedPH = Mathf.Floor(pH);
        switch (roundedPH)
        {
            case 0: currentColor = ph0; break;
            case 1: currentColor = ph1; break;
            case 2: currentColor = ph2; break;
            case 3: currentColor = ph3; break;
            case 4: currentColor = ph4; break;
            case 5: currentColor = ph5; break;
            case 6: currentColor = ph6; break;
            case 7: currentColor = ph7; break;
            case 8: currentColor = ph8; break;
            case 9: currentColor = ph9; break;
            case 10: currentColor = ph10; break;
            case 11: currentColor = ph11; break;
            case 12: currentColor = ph12; break;
            case 13: currentColor = ph13; break;
            case 14: currentColor = ph14; break;
            default: break;
        }

        cylinderRenderer.material.color = Color.Lerp(cylinderRenderer.material.color, currentColor, 0.5f);
        //string displayText = "Titration of Hydrochrolic Acid and Sodium Hydroxide\nThe amount of liquid in the flask is: " + strReceived + " ml";

        float iniHeight = 1f;
        float maxVol = 80f;
        volumeMillilitres = updatedVolume * 1000;
        //float scaleFactor = 0;
        //if (updatedVolume > maxVol)
        //{
        //    scaleFactor = 1;
        //}
        //else if (updatedVolume < maxVol) 
        //{
        //    scaleFactor = updatedVolume / maxVol;
        //}
        float scaleFactor = Math.Clamp(updatedVolume, 0, 1);

        // Update the height (Y-axis scale) of the cylinder based on the acid volume
        Vector3 currentScale = cylinderParent.transform.localScale;
        currentScale.y = iniHeight * scaleFactor;
        cylinderParent.transform.localScale = currentScale;

        
        string displayText = "Total Volume in beaker: " + volumeMillilitres + "ml" + " \n Ph of solution: " + pH;
        //string displayText = textInputMolarityAcid;
        textElement.text = displayText;
    }

    // Function to stop the serial thread
    private void OnApplicationQuit()
    {
        isRunning = false;
        if (serialPort.IsOpen)
        {
            serialPort.Close(); // Ensure the port is closed
        }
        serialThread.Join(); // Wait for the thread to finish
    }

    public void updateParameters()
    {
        // Check if the input fields are not empty, if not program will crash
        if (string.IsNullOrEmpty(molarityAcidInputField.text) ||
            string.IsNullOrEmpty(molarityBaseInputField.text) ||
            string.IsNullOrEmpty(volumeAcidInputField.text) ||
            string.IsNullOrEmpty(weightBeakerInputField.text))
        {
            Debug.LogError("One or more fields are empty. Please enter valid values.");
            return; // Exit the method if any field is empty
        }

        // Parsing the input values to floats
        bool validMolarityAcid = float.TryParse(molarityAcidInputField.text, out float newMolarityAcid);
        bool validMolarityBase = float.TryParse(molarityBaseInputField.text, out float newMolarityBase);
        bool validVolumeAcid = float.TryParse(volumeAcidInputField.text, out float newVolumeAcid);
        bool validWeightBeaker = float.TryParse(weightBeakerInputField.text, out float newWeightBeaker);

        // Check if all inputs are valid numbers and not strings
        if (!validMolarityAcid || !validMolarityBase || !validVolumeAcid || !validWeightBeaker)
        {
            Debug.LogError("Invalid input. Please enter valid numbers in all fields.");
            return; // Exit the method if any value is not a valid number
        }
        else
        {
            molarityAcid = newMolarityAcid;
            molarityBase = newMolarityBase;
            volumeAcid = newVolumeAcid / 1000 ;
            weightBeaker = newWeightBeaker / 1000;
            Debug.Log("Parameters Updated Succesfully");
        }
    }

    public void toggleIndicator()
    {
        if (cylinderRenderer != null)
        {
            if (meshrender != null)
            {
            meshrender.enabled = !meshrender.enabled;
            Debug.Log("Success! MeshRenderer toggled.");
            }
            else
            {
            Debug.LogError("MeshRenderer component not found.");
            }
        }
    }
}
