using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core;
using KompasWrapper;

namespace PluginUI
{
    /// <summary>
    /// Класс хранящий и обрабатывающий пользовательский интерфейс плагина
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Объект класса построителя
        /// </summary>
        private ScrewBuilder _screwBuilder;

        /// <summary>
        /// Объект класса с параметрами
        /// </summary>
        private ScrewParameters _screwParameters =
            new ScrewParameters();

        /// <summary>
        /// Словарь содержащий пары (Текстбоксы, имя параметра)
        /// </summary>
        private Dictionary<TextBox, ParameterNames> _textBoxesDictionary;

        /// <summary>
        /// Словарь содержащие пары (Текстбокс, корректное ли значение в нём)
        /// </summary>
        private Dictionary<TextBox, bool> _isValueInTextBoxCorrect;

        public MainForm()
        {
            InitializeComponent();

            _textBoxesDictionary = new Dictionary<TextBox, ParameterNames>
            {
                {ScrewLengthTextBox, ParameterNames.ScrewLength},
                {SliteLengthTextBox, ParameterNames.SlitLength},
                {FilletRadiusTextBox, ParameterNames.FilletRadius},
                {HeadDiameterTextBox, ParameterNames.HeadDiameter},
                {BaseDiameterTextBox, ParameterNames.BaseDiameter},
                {IndentLengthTextBox, ParameterNames.IndentLength}
            };

            _isValueInTextBoxCorrect = new Dictionary<TextBox, bool>
            {
                {ScrewLengthTextBox, true},
                {SliteLengthTextBox, true},
                {FilletRadiusTextBox, true},
                {HeadDiameterTextBox, true},
                {BaseDiameterTextBox, true},
                {IndentLengthTextBox, true}
            };

            foreach (var textBox in _textBoxesDictionary)
            {
                textBox.Key.Text = _screwParameters
                    .GetParameterValueByName(textBox.Value).ToString();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Построить"
        /// </summary>
        private void BuildButton_Click(object sender, EventArgs e)
        {
            var connector = new KompasConnector();
            _screwBuilder =
                new ScrewBuilder(_screwParameters, connector);

            _screwBuilder.BuildScrew();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            Validating(textBox);
        }


        /// <summary>
        /// Общий метод валидации текстбокса
        /// </summary>
        private void Validating(TextBox textBox)
        {
            try
            {
                _textBoxesDictionary.TryGetValue(textBox,
                    out var parameterInTextBoxName);
                _screwParameters.SetParameterByName(parameterInTextBoxName,
                    int.Parse(textBox.Text));

                if (textBox == SliteLengthTextBox)
                {
                    HeadDiameterTextBox.Text =
                        _screwParameters.HeadDiameter.ToString();
                }
                else if (textBox == HeadDiameterTextBox)
                {
                    SliteLengthTextBox.Text =
                        _screwParameters.SliteLength.ToString();
                }

                //Значение в текстбоксе правильное
                _isValueInTextBoxCorrect[textBox] = true;
                bool isTextBoxesValuesCorrect = true;

                foreach (var isValueCorrect in _isValueInTextBoxCorrect)
                {
                    isTextBoxesValuesCorrect &= isValueCorrect.Value;
                }

                //Проверяем, можно ли активировать кнопку
                if (isTextBoxesValuesCorrect)
                {
                    BuildButton.Enabled = true;
                }
                textBox.BackColor = Color.White;
                toolTip.Active = false;
            }
            catch (Exception exception)
            {
                //Значение в текстбоксе неправильное
                BuildButton.Enabled = false;
                textBox.BackColor = Color.LightSalmon;
                toolTip.Active = true;
                toolTip.SetToolTip(textBox, exception.Message);
                _isValueInTextBoxCorrect[textBox] = false;
            }
        }
    }
}
