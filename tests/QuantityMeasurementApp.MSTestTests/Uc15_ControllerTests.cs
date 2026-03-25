using System;
using ControllerLayer.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using ServiceLayer.Interfaces;

namespace QuantityMeasurementApp.MSTestTests
{
    internal sealed class FakeQuantityMeasurementService : IQuantityMeasurementService
    {
        public QuantityDto CompareEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.CompareEquality,
                MeasurementType = firstQuantityDto.MeasurementType,
                EqualityResult = true
            };

        public QuantityDto Convert(QuantityDto quantityDto, string targetUnitText, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Convert,
                MeasurementType = quantityDto.MeasurementType,
                ResultValue = 12.0,
                ResultUnitText = targetUnitText
            };

        public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Add,
                MeasurementType = firstQuantityDto.MeasurementType,
                ResultValue = 2.0,
                ResultUnitText = firstQuantityDto.FirstUnitText
            };

        public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Add,
                MeasurementType = firstQuantityDto.MeasurementType,
                ResultValue = 2.0,
                ResultUnitText = targetUnitText
            };

        public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Subtract,
                MeasurementType = firstQuantityDto.MeasurementType,
                ResultValue = 1.0,
                ResultUnitText = firstQuantityDto.FirstUnitText
            };

        public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Subtract,
                MeasurementType = firstQuantityDto.MeasurementType,
                ResultValue = 1.0,
                ResultUnitText = targetUnitText
            };

        public QuantityDto Divide(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
            new QuantityDto
            {
                OperationType = OperationType.Divide,
                MeasurementType = firstQuantityDto.MeasurementType,
                ScalarResult = 5.0
            };
    }

    [TestClass]
    public sealed class Uc15_ControllerTests
    {
        [TestMethod]
        public void testController_NullService_Prevention()
        {
            bool didThrow = false;
            try { _ = new QuantityMeasurementController(null!); } catch (ArgumentNullException) { didThrow = true; }
            Assert.IsTrue(didThrow);
        }

        [TestMethod]
        public void testController_DemonstrateEquality_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = controller.PerformEquality(first, second);

            Assert.AreEqual(true, result.EqualityResult);
        }

        [TestMethod]
        public void testController_DemonstrateConversion_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto input = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };

            QuantityDto result = controller.PerformConversion(input, "inch");

            Assert.AreEqual("inch", result.ResultUnitText);
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = controller.PerformAddition(first, second);

            Assert.AreEqual(OperationType.Add, result.OperationType);
        }

        [TestMethod]
        public void testController_DemonstrateAddition_Error()
        {
            var controller = new QuantityMeasurementController(new FakeErrorService());

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 100.0, FirstUnitText = "celsius" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 50.0, FirstUnitText = "celsius" };

            QuantityDto result = controller.PerformAddition(first, second);

            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void testController_DisplayResult_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto dto = new QuantityDto
            {
                OperationType = OperationType.Convert,
                MeasurementType = MeasurementType.Length,
                ResultValue = 12.0,
                ResultUnitText = "inch",
                HasError = false
            };

            string output = controller.DisplayResult(dto);

            Assert.IsTrue(output.Contains("Converted Result"));
        }

        [TestMethod]
        public void testController_DisplayResult_Error()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto dto = new QuantityDto
            {
                OperationType = OperationType.Add,
                MeasurementType = MeasurementType.Temperature,
                HasError = true,
                ErrorMessage = "Temperature does not support addition."
            };

            string output = controller.DisplayResult(dto);

            Assert.IsTrue(output.Contains("ERROR"));
        }

        [TestMethod]
        public void testController_ConsoleOutput_Format()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto dto = new QuantityDto
            {
                OperationType = OperationType.Divide,
                MeasurementType = MeasurementType.Length,
                ScalarResult = 5.0
            };

            string output = controller.DisplayResult(dto);

            Assert.IsTrue(output.Contains("Division Result"));
        }

        [TestMethod]
        public void testLayerSeparation_ControllerIndependence()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());
            Assert.IsNotNull(controller);
        }

        private sealed class FakeErrorService : IQuantityMeasurementService
        {
            public QuantityDto CompareEquality(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Convert(QuantityDto quantityDto, string targetUnitText, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Add(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Subtract(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, string targetUnitText, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };

            public QuantityDto Divide(QuantityDto firstQuantityDto, QuantityDto secondQuantityDto, Guid? userId = null) =>
                new QuantityDto { HasError = true, ErrorMessage = "error" };
        }
    }
}