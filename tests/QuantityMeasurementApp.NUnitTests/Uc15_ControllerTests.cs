using System;
using ControllerLayer.Controllers;
using ModelLayer.Dtos;
using ModelLayer.Enums;
using NUnit.Framework;
using ServiceLayer.Interfaces;

namespace QuantityMeasurementApp.NUnitTests
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

    [TestFixture]
    public sealed class Uc15_ControllerTests
    {
        [Test]
        public void testController_NullService_Prevention()
        {
            Assert.Throws<ArgumentNullException>(() => new QuantityMeasurementController(null!));
        }

        [Test]
        public void testController_DemonstrateEquality_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = controller.PerformEquality(first, second);

            Assert.That(result.EqualityResult, Is.True);
        }

        [Test]
        public void testController_DemonstrateConversion_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto input = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };

            QuantityDto result = controller.PerformConversion(input, "inch");

            Assert.That(result.ResultUnitText, Is.EqualTo("inch"));
        }

        [Test]
        public void testController_DemonstrateAddition_Success()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 1.0, FirstUnitText = "feet" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Length, FirstValue = 12.0, FirstUnitText = "inch" };

            QuantityDto result = controller.PerformAddition(first, second);

            Assert.That(result.OperationType, Is.EqualTo(OperationType.Add));
        }

        [Test]
        public void testController_DemonstrateAddition_Error()
        {
            IQuantityMeasurementService errorService = new FakeErrorService();
            var controller = new QuantityMeasurementController(errorService);

            QuantityDto first = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 100.0, FirstUnitText = "celsius" };
            QuantityDto second = new QuantityDto { MeasurementType = MeasurementType.Temperature, FirstValue = 50.0, FirstUnitText = "celsius" };

            QuantityDto result = controller.PerformAddition(first, second);

            Assert.That(result.HasError, Is.True);
        }

        [Test]
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

            Assert.That(output, Does.Contain("Converted Result"));
        }

        [Test]
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

            Assert.That(output, Does.Contain("ERROR"));
        }

        [Test]
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

            Assert.That(output, Does.Contain("Division Result"));
        }

        [Test]
        public void testLayerSeparation_ControllerIndependence()
        {
            var controller = new QuantityMeasurementController(new FakeQuantityMeasurementService());
            Assert.That(controller, Is.Not.Null);
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