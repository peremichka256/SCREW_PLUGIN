using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainForm()
        {
            InitializeComponent();
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
    }
}
