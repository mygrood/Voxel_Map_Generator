using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class SliderInputSync : MonoBehaviour
{
    public List<Slider> sliders; 
    public List<TMP_InputField> inputFields; 

    public List<int> basicSettings;

    void Start()
    {
        // ��������� �������� �� ������ ��������� �������� ����� �����
        for (int i = 0; i < inputFields.Count; i++)
        {
            if (i < sliders.Count)
            {
                if (int.TryParse(inputFields[i].text, out int result))
                {
                    
                    result = Mathf.Clamp(result, (int)sliders[i].minValue, (int)sliders[i].maxValue);
                    sliders[i].value = result;
                }
                else
                {
                    // ���� �������� �����������, ������������� �������� �� ���������
                    sliders[i].value = (int)sliders[i].value;
                }
            }
        }

        // �������� �� ������� ��� ����� �����
        for (int i = 0; i < inputFields.Count; i++)
        {
            int index = i; 
            inputFields[i].onEndEdit.AddListener(value => OnInputFieldValueChanged(value, index));
        }

        // �������� �� ������� ��� ���������
        for (int i = 0; i < sliders.Count; i++)
        {
            int index = i;
            sliders[i].onValueChanged.AddListener(value => OnSliderValueChanged(value, index));
        }

    }



    void OnSliderValueChanged(float value, int sliderIndex)
    {
        // ��������� �������� ���� �����, ���������� � ���������� ���������
        if (sliderIndex < inputFields.Count)
        {
            inputFields[sliderIndex].text = ((int)value).ToString();
        }
    }

    void OnInputFieldValueChanged(string value, int inputFieldIndex)
    {
        if (inputFieldIndex < sliders.Count)
        {
            // �������� � ��������� �������� � �������� ��������� ��������
            if (int.TryParse(value, out int result))
            {
                result = Mathf.Clamp(result, (int)sliders[inputFieldIndex].minValue, (int)sliders[inputFieldIndex].maxValue);
                sliders[inputFieldIndex].value = result;
            }
            else
            {
                // ��������� ������������ ��������
                inputFields[inputFieldIndex].text = ((int)sliders[inputFieldIndex].value).ToString();
            }
        }
    }

    public void OnResetButtonClick()
    {
        for (int i = 0; i < sliders.Count; i++)
        {
            sliders[i].value = basicSettings[i];
        }

    }
}
