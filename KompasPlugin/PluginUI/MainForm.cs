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

        /// <summary>
        /// Устанавливает стиль для проверенного значения
        /// </summary>
        /// <param name="sender">Текстбокс</param>
        private void TextBox_Validated(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                BuildButton.Enabled = true;
                textBox.BackColor = Color.White;
                toolTip.Active = false;
            }
        }

        /// <summary>
        /// Общий метод валидации текстбокса
        /// </summary>
        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

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
            }
            catch (Exception exception)
            {
                BuildButton.Enabled = false;
                textBox.BackColor = Color.LightSalmon;
                toolTip.Active = true;
                toolTip.SetToolTip(textBox, exception.Message);
                e.Cancel = true;
            }
        }
    }
}
