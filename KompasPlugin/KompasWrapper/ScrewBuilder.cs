using Core;
using Kompas;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using KompasAPI7;
using System.Drawing;
using System.Numerics;

namespace KompasWrapper
{
    /// <summary>
    /// Класс осуществляющий построение детали
    /// </summary>
    public class ScrewBuilder
    {
        /// <summary>
        /// Объект класса конектора для связи с КОММПАС-3D
        /// </summary>
        private KompasConnector _connector;

        /// <summary>
        /// Объект класса параметра для построение детали
        /// </summary>
        private ScrewParameters _parameters;

        /// <summary>
        /// Стиль линии: основная
        /// </summary>
        private const int MainLineStyle = 1;

        /// <summary>
        /// Глубина шлица
        /// </summary>
        private const double SLITE_DEPTH = 1.2;

        /// <summary>
        /// Угол выреза шлица
        /// </summary>
        private const double SLITE_ANGLE = -45;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="parameters">Параметры киянки</param>
        /// <param name="connector">Объект для связи с КОМПАС-3D</param>
        public ScrewBuilder(ScrewParameters parameters,
            KompasConnector connector)
        {
            _parameters = parameters;
            _connector = connector;
        }

        /// <summary>
        /// Метод объединяющий головку и стержень
        /// </summary>
        public void BuildScrew()
        {
            _connector.Start();
            _connector.CreateDocument3D();

            //Построение основных частей винта
            BuildRod(_parameters.ScrewLength, _parameters.BaseDiameter,
                _parameters.IndentLength, _parameters.HeadDiameter);
            BuildHead(_parameters.HeadDiameter, _parameters.SliteLength,
                _parameters.BaseDiameter, _parameters.ScrewdriverType);

            //Создание скругления между головкой и стержнем
            
            var xCoordOfEdge = Convert.ToDouble(_parameters.BaseDiameter) / 2;
            var yCoordOfEdge = Convert.ToDouble(_parameters.HeadDiameter) / 2 -
                Convert.ToDouble(_parameters.BaseDiameter) / 2;
            var zCoordOfEdge = 0;
            CreateChamfer(_parameters.FilletRadius,
                xCoordOfEdge, yCoordOfEdge, zCoordOfEdge, true);
        }

        /// <summary>
        /// Метод осуществляющий построение головки
        /// </summary>
        /// <param name="headDiameter">Диаметр головки</param>
        /// <param name="slitLength">Длина шлица</param>
        private void BuildHead(double headDiameter, double slitLength,
            double baseDiameter, ScrewdriverTypes screwdriverType)
        {
            //Создание головки
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ);
            var doc2d = (ksDocument2D)sketch.BeginEdit();
            doc2d.ksCircle(0, 0, headDiameter / 2, MainLineStyle);

            sketch.EndEdit();
            СreateExtrusion(sketch, headDiameter /2 - baseDiameter / 2);

            //Вырезание шлица

            switch (screwdriverType)
            {
                case ScrewdriverTypes.Hexagonal:
                    {
                        var slitWidth = slitLength / 4;
                        var slitSketch = CreateSketch(Obj3dType.o3d_planeXOZ);
                        var slitDoc2d = (ksDocument2D)slitSketch.BeginEdit();
                        var radius = headDiameter / 2 - 1;
                        var x = radius;
                        var y = 0.0;
                        var angle = 60 * Math.PI / 180;
                        var points = new List<PointDoubleType>();
                        points.Add(new PointDoubleType(x, y));
                        for (var i = 1; i <= 5; i++)
                        {
                            points.Add(new PointDoubleType(radius * Math.Cos(i * angle),
                                radius * Math.Sin(i * angle)));
                        }

                        for (var i = 0; i < points.Count; i++)
                        {
                            var nextIndex = i + 1;
                            if (i == points.Count - 1)
                            {
                                nextIndex = 0;
                            }
                            slitDoc2d.ksLineSeg(points[i].X, points[i].Y,
                                points[nextIndex].X, points[nextIndex].Y, 1);
                        }
                        slitSketch.EndEdit();
                        СreateCutExtrusion(slitSketch, SLITE_DEPTH, SLITE_ANGLE, false);
                        break;
                    }
                case ScrewdriverTypes.Cross:
                    {
                        var slitWidth = slitLength / 4;
                        var slitSketch = CreateSketch(Obj3dType.o3d_planeXOZ);
                        var slitDoc2d = (ksDocument2D)slitSketch.BeginEdit();
                        slitDoc2d.ksRectangle(DrawRectangle(-slitWidth / 2, -slitLength / 2,
                            slitLength, slitWidth));
                        slitDoc2d.ksRectangle(DrawRectangle(-slitLength / 2, -slitWidth / 2,
                            slitWidth, slitLength));

                        slitSketch.EndEdit();
                        СreateCutExtrusion(slitSketch, SLITE_DEPTH, SLITE_ANGLE, false);
                        break;
                    }
                case ScrewdriverTypes.Slotted:
                    {

                        var slitWidth = slitLength / 4;
                        var slitSketch = CreateSketch(Obj3dType.o3d_planeXOZ);
                        var slitDoc2d = (ksDocument2D)slitSketch.BeginEdit();
                        slitDoc2d.ksRectangle(DrawRectangle(-slitWidth / 2, -slitLength / 2,
                            slitLength, slitWidth));

                        slitSketch.EndEdit();
                        СreateCutExtrusion(slitSketch, SLITE_DEPTH, SLITE_ANGLE, false);
                        break;
                    }
            }

            //Создание фаски на головке
            var xCoordOfEdge = headDiameter / 2;
            var yCoordOfEdge = headDiameter / 2 - baseDiameter / 2;
            var zCoordOfEdge = 0;
            CreateChamfer(headDiameter / 2 - baseDiameter / 2,
                xCoordOfEdge, yCoordOfEdge, zCoordOfEdge, false);
        }

        /// <summary>
        /// Метод осуществляющий построение стержня
        /// </summary>
        /// <param name="screwLength">Общая длина винта</param>
        /// <param name="baseDiameter">Диаметр основанмя стержня</param>
        /// <param name="indentLength">Длина отсупа</param>
        private void BuildRod(double screwLength, double baseDiameter, double indentLength, double headDiameter)
        {
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ);
            var doc2d = (ksDocument2D)sketch.BeginEdit();
            doc2d.ksCircle(0, 0, baseDiameter / 2, MainLineStyle);

            sketch.EndEdit();
            СreateExtrusion(sketch, screwLength);

            //Создание фаски на конце винта
            var xCoordOfEdge = -baseDiameter / 2;
            var yCoordOfEdge = screwLength;
            var zCoordOfEdge = 0;
            CreateChamfer(_parameters.FilletRadius,
                xCoordOfEdge, yCoordOfEdge, zCoordOfEdge, false);

            //Создание резьбы
            //Создание вырезающего элемента
            var threadBeginning = indentLength + headDiameter / 2 
                - baseDiameter / 2;
            var threadSketch = CreateSketch(Obj3dType.o3d_planeXOY, null);
            var threadDoc2d = (ksDocument2D)threadSketch.BeginEdit();
            threadDoc2d.ksCircle(baseDiameter / 2, threadBeginning, 0.25, MainLineStyle);
            threadSketch.EndEdit();

            //Создание спирали
            ksEntity conicSpiral =
                (ksEntity)_connector.Part.NewEntity((short)Obj3dType.o3d_cylindricSpiral);
            ksCylindricSpiralDefinition cylindricalSpiralDefinition =
                (ksCylindricSpiralDefinition)conicSpiral.GetDefinition();
            cylindricalSpiralDefinition.diamType = 0;
            cylindricalSpiralDefinition.buildDir = true;
            cylindricalSpiralDefinition.diam = baseDiameter;
            cylindricalSpiralDefinition.buildMode = 2;
            cylindricalSpiralDefinition.turn = 40;
            cylindricalSpiralDefinition.height = screwLength;
            var spiritalOffsetPlane = CreateOffsetPlane(Obj3dType.o3d_planeXOZ,
                -threadBeginning);
            cylindricalSpiralDefinition.SetPlane(spiritalOffsetPlane);
            conicSpiral.SetAdvancedColor(0);
            conicSpiral.hidden = true;
            conicSpiral.Create();

            CutTrajectoryEvolution(threadSketch, conicSpiral);
        }

        /// <summary>
        /// Метод рисования прямоугольника
        /// </summary>
        /// <param name="x">X базовой точки</param>
        /// <param name="y">Y базовой точки</param>
        /// <param name="height">Высота</param>
        /// <param name="width">Ширина</param>
        /// <returns>Переменная с параметрами прямоугольника</returns>
        private ksRectangleParam DrawRectangle(double x, double y,
            double height, double width)
        {
            var rectangleParam =
                (ksRectangleParam)_connector.Object.GetParamStruct
                    ((short)StructType2DEnum.ko_RectangleParam);
            rectangleParam.x = x;
            rectangleParam.y = y;
            rectangleParam.height = height;
            rectangleParam.width = width;
            rectangleParam.style = MainLineStyle;
            return rectangleParam;
        }

        /// <summary>
        /// Метод создающий эскиз
        /// </summary>
        /// <param name="planeType">Плоскость</param>
        /// <param name="offsetPlane">Объект смещения</param>
        /// <returns>Эскиз</returns>
        private ksSketchDefinition CreateSketch(Obj3dType planeType,
            ksEntity offsetPlane = null)
        {
            var plane = (ksEntity)_connector.Part
                .GetDefaultEntity((short)planeType);

            var sketch = (ksEntity)_connector.Part.
                NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketch = (ksSketchDefinition)sketch.GetDefinition();

            if (offsetPlane != null)
            {
                ksSketch.SetPlane(offsetPlane);
                sketch.Create();
                return ksSketch;
            }

            ksSketch.SetPlane(plane);
            sketch.Create();
            return ksSketch;
        }

        /// <summary>
        /// Метод смещающий плоскость
        /// </summary>
        /// <param name="plane">Плоскость</param>
        /// <param name="offset">Расстояние смещения</param>
        /// <returns>Объект смещения</returns>
        private ksEntity CreateOffsetPlane(Obj3dType plane, double offset)
        {
            var offsetEntity = (ksEntity)_connector
                .Part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var offsetDef = (ksPlaneOffsetDefinition)offsetEntity
                .GetDefinition();
            offsetDef.SetPlane((ksEntity)_connector
                .Part.NewEntity((short)plane));
            offsetDef.offset = offset;
            offsetDef.direction = false;
            offsetEntity.Create();
            return offsetEntity;
        }

        /// <summary>
        /// Создания фаски на выбранном ребре
        /// </summary>
        /// <param name="filletRadius">Радиус</param>
        /// <param name="x">X-координата точки на ребре</param>
        /// <param name="y">Y-координата точки на ребре</param>
        /// <param name="z">Z-координата точки на ребре</param>
        /// <param name="isFillet">закругленная ли фаска</param>
        private void CreateChamfer(double filletRadius, double x,
            double y, double z, bool isFillet)
        {
            ksEntityCollection iArray = null;
            ksEntity chamfertEntity = null;

            if (isFillet)
            {
                chamfertEntity = (ksEntity)_connector
                    .Part.NewEntity((short)Obj3dType.o3d_fillet);
                var filletDef =
                    (ksFilletDefinition)chamfertEntity.GetDefinition();
                filletDef.radius = filletRadius;
                filletDef.tangent = true;
                iArray = (ksEntityCollection)filletDef.array();
            }
            else
            {
                chamfertEntity = (ksEntity)_connector
                    .Part.NewEntity((short)Obj3dType.o3d_chamfer);
                var chamferDef =
                    (ksChamferDefinition)chamfertEntity.GetDefinition();
                chamferDef.SetChamferParam(true, filletRadius, filletRadius);
                iArray = (ksEntityCollection)chamferDef.array();
            }
            ksEntityCollection iCollection = (ksEntityCollection)_connector
                .Part.EntityCollection((short)Obj3dType.o3d_edge);

            iCollection.SelectByPoint(x, y, z);
            var iEdge = iCollection.Last();
            iArray.Add(iEdge);
            chamfertEntity.Create();
        }

        /// <summary>
        /// Метод осущетсвляющий выдавливание
        /// </summary>
        /// <param name="sketch">Эскиз</param>
        /// <param name="depth">Расстояние выдавливания</param>
        private void СreateExtrusion(ksSketchDefinition sketch,
            double depth, bool side = true)
        {
            var extrusionEntity = (ksEntity)_connector.Part.NewEntity(
                (short)ksObj3dTypeEnum.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusionEntity
                .GetDefinition();

            extrusionDef.SetSideParam(side,
                (short)End_Type.etBlind, depth);
            extrusionDef.directionType = side ?
                (short)Direction_Type.dtNormal :
                (short)Direction_Type.dtReverse;
            extrusionDef.SetSketch(sketch);
            extrusionEntity.Create();
        }

        /// <summary>
        /// Метод осуществляющий вырезание
        /// </summary>
        /// <param name="sketch">Эскиз</param>
        /// <param name="depth">Расстояние выреза</param>
        private void СreateCutExtrusion(ksSketchDefinition sketch,
            double depth, double angle, bool side = true)
        {
            var cutExtrusionEntity = (ksEntity)_connector.Part.NewEntity(
                (short)ksObj3dTypeEnum.o3d_cutExtrusion);
            var cutExtrusionDef =
                (ksCutExtrusionDefinition)cutExtrusionEntity
                    .GetDefinition();

            cutExtrusionDef.SetSideParam(side,
                (short)End_Type.etBlind, depth, angle);
            cutExtrusionDef.directionType = side ?
                (short)Direction_Type.dtNormal :
                (short)Direction_Type.dtReverse;
            cutExtrusionDef.cut = true;
            cutExtrusionDef.SetSketch(sketch);
            cutExtrusionEntity.Create();
        }

        /// <summary>
		/// Выдавливание вырезанием по траектории
		/// </summary>
		/// <param name="sketch">Эскиз</param>
		/// <param name="trajectory">Траектория</param>
		private void CutTrajectoryEvolution(ksSketchDefinition sketch, ksEntity trajectory)
        {
            ksEntity cinematicEvolition =
                (ksEntity)_connector.Part.NewEntity((short)Obj3dType.o3d_cutEvolution);
            ksCutEvolutionDefinition cutEvolutionDefinition =
                (ksCutEvolutionDefinition)cinematicEvolition.GetDefinition();
            cutEvolutionDefinition.SetSketch(sketch);
            ksEntityCollection collection =
                (ksEntityCollection)cutEvolutionDefinition.PathPartArray();
            collection.Add(trajectory);
            cinematicEvolition.Create();
        }
    }
}
