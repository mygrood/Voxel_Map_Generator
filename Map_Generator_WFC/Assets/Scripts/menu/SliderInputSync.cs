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
        // Обновляем слайдеры на основе начальных значений полей ввода
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
                    // Если значение некорректно, устанавливаем значение по умолчанию
                    sliders[i].value = (int)sliders[i].value;
                }
            }
        }

        // Подписка на события для полей ввода
        for (int i = 0; i < inputFields.Count; i++)
        {
            int index = i; 
            inputFields[i].onEndEdit.AddListener(value => OnInputFieldValueChanged(value, index));
        }

        // Подписка на события для слайдеров
        for (int i = 0; i < sliders.Count; i++)
        {
            int index = i;
            sliders[i].onValueChanged.AddListener(value => OnSliderValueChanged(value, index));
        }

    }



    void OnSliderValueChanged(float value, int sliderIndex)
    {
        // Обновляем значение поля ввода, связанного с измененным слайдером
        if (sliderIndex < inputFields.Count)
        {
            inputFields[sliderIndex].text = ((int)value).ToString();
        }
    }

    void OnInputFieldValueChanged(string value, int inputFieldIndex)
    {
        if (inputFieldIndex < sliders.Count)
        {
            // Проверка и установка значения в пределах диапазона слайдера
            if (int.TryParse(value, out int result))
            {
                result = Mathf.Clamp(result, (int)sliders[inputFieldIndex].minValue, (int)sliders[inputFieldIndex].maxValue);
                sliders[inputFieldIndex].value = result;
            }
            else
            {
                // Обработка некорректных значений
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
