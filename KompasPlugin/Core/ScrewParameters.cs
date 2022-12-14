namespace Core
{
    /// <summary>
    /// Класс хранящий параметры винта
    /// </summary>
    public class ScrewParameters
    {
        /// <summary>
        /// Общая длина винта
        /// </summary>
        private static Parameter _screwLength =
            new(ParameterNames.ScrewLength,
                MAX_SCREW_LENGTH, MIN_SCREW_LENGTH);

        /// <summary>
        /// Длина шлица
        /// </summary>
        private static Parameter _sliteLength =
            new(ParameterNames.SlitLength,
                MAX_SLITE_LENGTH, MIN_SLITE_LENGTH);

        /// <summary>
        /// Радиус скругления
        /// </summary>
        private static Parameter _filletRadius =
            new(ParameterNames.FilletRadius,
                MAX_FILLET_RADIUS, MIN_FILLET_RADIUS);

        /// <summary>
        /// Диаметр головки
        /// </summary>
        private static Parameter _headDiameter =
            new(ParameterNames.HeadDiameter,
                MAX_HEAD_DIAMETER, MIN_HEAD_DIAMETER);

        /// <summary>
        /// Диаметр основанмя стержня
        /// </summary>
        private static Parameter _baseDiameter =
            new(ParameterNames.BaseDiameter,
                MAX_BASE_DIAMETER, MIN_BASE_DIAMETER);

        /// <summary>
        /// Длина отсупа
        /// </summary>
        private static Parameter _indentLength =
            new(ParameterNames.IndentLength,
                MAX_INDENT_LENGTH, MIN_INDENT_LENGTH);

        /// <summary>
        /// Возвращает и задает отверстие под отвёртку
        /// </summary>
        public ScrewdriverTypes ScrewdriverType { get; set; }

        /// <summary>
        /// Словарь содержащий пары (Имя параметра, указатель на него)
        /// </summary>
        private Dictionary<ParameterNames, Parameter>
            _parametersDictionary =
                new()
                {
                    {_screwLength.Name, _screwLength},
                    {_sliteLength.Name, _sliteLength},
                    {_filletRadius.Name, _filletRadius},
                    {_headDiameter.Name, _headDiameter},
                    {_baseDiameter.Name, _baseDiameter},
                    {_indentLength.Name, _indentLength}
                };

        /// <summary>
        /// Конастанты минимальных и максимальных значений параметров в мм
        /// Минимальные значения являются дефолтными
        /// </summary>
        public const int MIN_SCREW_LENGTH = 22;
        public const int MAX_SCREW_LENGTH = 26;

        public const int MIN_SLITE_LENGTH = 10;
        public const int MAX_SLITE_LENGTH = 13;

        public const int MIN_FILLET_RADIUS = 1;
        public const int MAX_FILLET_RADIUS = 2;

        public const int MIN_HEAD_DIAMETER = 12;
        public const int MAX_HEAD_DIAMETER = 15;

        public const int MIN_BASE_DIAMETER = 5;
        public const int MAX_BASE_DIAMETER = 6;

        public const int MIN_INDENT_LENGTH = 2;
        public const int MAX_INDENT_LENGTH = 3;

        /// <summary>
        /// Константы ограничений для параметров
        /// </summary>
        public const int SLITE_HEAD_DIFFERENCE = 2;

        /// <summary>
        /// Задаёт или возвращает общую длину винта
        /// </summary>
        public int ScrewLength
        {
            get => _screwLength.Value;
            set => _screwLength.Value = value;
        }

        /// <summary>
        /// Задаёт или возвращает длину шлица
        /// </summary>
        public int SliteLength
        {
            get => _sliteLength.Value;
            set
            {
                _sliteLength.Value = value;
                _headDiameter.Value = value + SLITE_HEAD_DIFFERENCE;
            }
        }

        /// <summary>
        /// Задаёт или возвращает радиус скругления
        /// </summary>
        public int FilletRadius
        {
            get => _filletRadius.Value;
            set => _filletRadius.Value = value;
        }

        /// <summary>
        /// Задаёт или возвращает диаметр головки
        /// </summary>
        public int HeadDiameter
        {
            get => _headDiameter.Value;
            set
            {
                _headDiameter.Value = value;
                _sliteLength.Value = value - SLITE_HEAD_DIFFERENCE;
            }
        }

        /// <summary>
        /// Задаёт или возвращает диаметр основанмя стержня
        /// </summary>
        public int BaseDiameter
        {
            get => _baseDiameter.Value;
            set => _baseDiameter.Value = value;
        }

        /// <summary>
        /// Задаёт или возвращает размер длину отсупа
        /// </summary>
        public int IndentLength
        {
            get => _indentLength.Value;
            set => _indentLength.Value = value;
        }

        /// <summary>
        /// Конструктор класса с минимальными значенми по умолчанию
        /// </summary>
        public ScrewParameters()
        {
            ScrewLength = MIN_SCREW_LENGTH;
            SliteLength = MIN_SLITE_LENGTH;
            FilletRadius = MIN_FILLET_RADIUS;
            HeadDiameter = MIN_HEAD_DIAMETER;
            BaseDiameter = MIN_BASE_DIAMETER;
            IndentLength = MIN_INDENT_LENGTH;
            ScrewdriverType = ScrewdriverTypes.Cross;
        }

        /// <summary>
        /// Метод передающй значение в сеттер параметра по имени
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Значение</param>
        public void SetParameterByName(ParameterNames name, int value)
        {
            if (_parametersDictionary.ContainsKey(name))
            {
                switch (name)
                {
                    case ParameterNames.SlitLength:
                        SliteLength = value;
                        break;
                    case ParameterNames.HeadDiameter:
                        HeadDiameter = value;
                        break;
                    default:
                        _parametersDictionary.TryGetValue(name,
                            out var parameter);
                        parameter.Value = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Метод возвращающий значение параметра по имени
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns>Значение</returns>
        public int GetParameterValueByName(ParameterNames name)
        {
            _parametersDictionary.TryGetValue(name, out var parameter);
            return parameter.Value;
        }
    }
}
