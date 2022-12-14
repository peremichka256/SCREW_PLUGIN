using System;
using Core;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ScrewParametersTests
    {
        /// <summary>
        /// Свойство возвращающее новый обект класса MalletParameters
        /// </summary>
        private ScrewParameters DefaultParameters =>
            new();

        /// <summary>
        /// Словарь имён и максимальных значений параметров
        /// </summary>
        private readonly Dictionary<ParameterNames, int>
            _maxValuesOfParameterDictionary =
                new()
                {
                    {
                        ParameterNames.ScrewLength,
                        ScrewParameters.MAX_SCREW_LENGTH
                    },
                    {
                        ParameterNames.SlitLength,
                        ScrewParameters.MAX_SLITE_LENGTH
                    },
                    {
                        ParameterNames.FilletRadius,
                        ScrewParameters.MAX_FILLET_RADIUS
                    },
                    {
                        ParameterNames.HeadDiameter,
                        ScrewParameters.MAX_HEAD_DIAMETER
                    },
                    {
                        ParameterNames.BaseDiameter,
                        ScrewParameters.MAX_BASE_DIAMETER
                    },
                    {
                        ParameterNames.IndentLength,
                        ScrewParameters.MAX_INDENT_LENGTH
                    },
                };

        [Test(Description = "Тест метода передающий значение "
                            + "в сеттер параметра по его имени")]
        public void TestSetParameterByName()
        {
            var testScrewParameters = DefaultParameters;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                testScrewParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            int errorCounter = 0;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                if (testScrewParameters.GetParameterValueByName(
                        parameterMaxValue.Key) != parameterMaxValue.Value)
                {
                    errorCounter++;
                }
            }

            Assert.That(errorCounter, Is.Zero,
                "Значения не были помещены в сеттеры параметров");
        }

        [Test(Description = "Тест на геттер значения параметра по имени")]
        public void TestGetParameterByName()
        {
            var testScrewParameters = DefaultParameters;

            var newValue = (ScrewParameters.MIN_SCREW_LENGTH
                            + ScrewParameters.MIN_SCREW_LENGTH) / 2;
            ParameterNames testParameterName =
                ParameterNames.ScrewLength;
            testScrewParameters
                .SetParameterByName(testParameterName, newValue);

            Assert.That(testScrewParameters
                    .GetParameterValueByName(testParameterName), Is.EqualTo(newValue),
                "Из геттера вернулось неверное значение");
        }

        [Test(Description = "Позитивный тест на геттеры параметров")]
        public void TestParameterGet()
        {
            var testScrewParameters = DefaultParameters;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                testScrewParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            Assert.That(testScrewParameters.ScrewLength
                          == ScrewParameters.MAX_SCREW_LENGTH
                          && testScrewParameters.FilletRadius
                          == ScrewParameters.MAX_FILLET_RADIUS
                          && testScrewParameters.BaseDiameter
                          == ScrewParameters.MAX_BASE_DIAMETER
                          && testScrewParameters.IndentLength
                          == ScrewParameters.MAX_INDENT_LENGTH, Is.True,
                "Возникает, если геттер вернул не то значение");
        }

        [Test(Description = "Тест на сеттер диаметра головки")]
        public void TestHeadDiameter_Set()
        {
            var testScrewParameters = DefaultParameters;

            testScrewParameters.HeadDiameter = ScrewParameters.MAX_HEAD_DIAMETER;
            testScrewParameters.SliteLength = ScrewParameters.MAX_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE;
            testScrewParameters.SliteLength = ScrewParameters.MIN_SLITE_LENGTH;

            Assert.That(testScrewParameters.HeadDiameter, Is.EqualTo(ScrewParameters.MIN_SLITE_LENGTH
                + ScrewParameters.SLITE_HEAD_DIFFERENCE),
                "Сеттер не поменял знаечние зависимого параметра");
        }

        [Test(Description = "Тест на сеттер длины шлица")]
        public void TestSlitLength_Set()
        {
            var testScrewParameters = DefaultParameters;

            testScrewParameters.HeadDiameter = ScrewParameters.MAX_HEAD_DIAMETER;
            testScrewParameters.SliteLength = ScrewParameters.MAX_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE;
            testScrewParameters.HeadDiameter = ScrewParameters.MIN_HEAD_DIAMETER;

            Assert.That(testScrewParameters.SliteLength, Is.EqualTo(ScrewParameters.MIN_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE),
                "Сеттер не поменял знаечние зависимого параметра");
        }

        [TestCase(TestName = "Проверка корректного возврата" +
                             " у свойства ScrewdriverType")]
        public void TestScrewdriverType_CorrectGet()
        {
            var detailParameters = DefaultParameters;

            var expected = ScrewdriverTypes.Cross;

            detailParameters.ScrewdriverType = expected;

            var actual = detailParameters.ScrewdriverType;

            Assert.That(actual, Is.EqualTo(expected),
                "Возвращенное значение не равно ожидаемому");
        }

        [TestCase(TestName = "Проверка корректного присвоения" +
                             " значения свойству ScrewdriverType")]
        public void TestScrewdriverType_CorrectSet()
        {
            var detailParameters = DefaultParameters;

            var value = ScrewdriverTypes.Cross;

            Assert.DoesNotThrow(() => detailParameters.ScrewdriverType = value,
                "Не удалось присвоить корректное значение");
        }
    }
}