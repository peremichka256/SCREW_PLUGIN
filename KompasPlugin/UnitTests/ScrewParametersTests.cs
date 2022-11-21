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
            var testMalletParameters = DefaultParameters;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                testMalletParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            int errorCounter = 0;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                if (testMalletParameters.GetParameterValueByName(
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
            var testMalletParameters = DefaultParameters;

            var newValue = (ScrewParameters.MIN_SCREW_LENGTH
                            + ScrewParameters.MIN_SCREW_LENGTH) / 2;
            ParameterNames testParameterName =
                ParameterNames.ScrewLength;
            testMalletParameters
                .SetParameterByName(testParameterName, newValue);

            Assert.That(testMalletParameters
                    .GetParameterValueByName(testParameterName), Is.EqualTo(newValue),
                "Из геттера вернулось неверное значение");
        }

        [Test(Description = "Позитивный тест на геттеры параметров")]
        public void TestParameterGet()
        {
            var testMalletParameters = DefaultParameters;

            foreach (var parameterMaxValue
                     in _maxValuesOfParameterDictionary)
            {
                testMalletParameters.SetParameterByName(
                    parameterMaxValue.Key, parameterMaxValue.Value);
            }

            Assert.That(testMalletParameters.ScrewLength
                          == ScrewParameters.MAX_SCREW_LENGTH
                          && testMalletParameters.FilletRadius
                          == ScrewParameters.MAX_FILLET_RADIUS
                          && testMalletParameters.BaseDiameter
                          == ScrewParameters.MAX_BASE_DIAMETER
                          && testMalletParameters.IndentLength
                          == ScrewParameters.MAX_INDENT_LENGTH, Is.True,
                "Возникает, если геттер вернул не то значение");
        }

        [Test(Description = "Тест на сеттер диаметра головки")]
        public void TestHeadDiameter_Set()
        {
            var testMalletParameters = DefaultParameters;

            testMalletParameters.HeadDiameter = ScrewParameters.MAX_HEAD_DIAMETER;
            testMalletParameters.SliteLength = ScrewParameters.MAX_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE;
            testMalletParameters.SliteLength = ScrewParameters.MIN_SLITE_LENGTH;

            Assert.That(testMalletParameters.HeadDiameter, Is.EqualTo(ScrewParameters.MIN_SLITE_LENGTH
                + ScrewParameters.SLITE_HEAD_DIFFERENCE),
                "Сеттер не поменял знаечние зависимого параметра");
        }

        [Test(Description = "Тест на сеттер длины шлица")]
        public void TestSlitLength_Set()
        {
            var testMalletParameters = DefaultParameters;

            testMalletParameters.HeadDiameter = ScrewParameters.MAX_HEAD_DIAMETER;
            testMalletParameters.SliteLength = ScrewParameters.MAX_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE;
            testMalletParameters.HeadDiameter = ScrewParameters.MIN_HEAD_DIAMETER;

            Assert.That(testMalletParameters.SliteLength, Is.EqualTo(ScrewParameters.MIN_HEAD_DIAMETER
                - ScrewParameters.SLITE_HEAD_DIFFERENCE),
                "Сеттер не поменял знаечние зависимого параметра");
        }
    }
}