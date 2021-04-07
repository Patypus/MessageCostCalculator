using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CostCalculator
{
    [TestFixture]
    public class TotalCostCalculatorTests
    {
        private IAccountDetails mockAccountDetails;
        private ITotalCostCalculator totalCostCalculator;

        [SetUp]
        public void SetUp()
        {
            mockAccountDetails = Substitute.For<IAccountDetails>();

            totalCostCalculator = new TotalCostCalculator(mockAccountDetails);
        }

        [Test]
        public void CalculateCost_SatisfiesExample()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, QtyTo = 200, PricePerTextMessage = 0.1M },
                new PriceBand {  QtyFrom = 201, QtyTo = 500, PricePerTextMessage = 0.08M },
                new PriceBand {  QtyFrom = 501, QtyTo = 1000, PricePerTextMessage = 0.06M },
                new PriceBand {  QtyFrom = 1001, PricePerTextMessage = 0.03M }
            };
            
            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(700);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            // Act
            var result = totalCostCalculator.CalculateCost(Guid.NewGuid(), 3, 2021);

            //Assert
            Assert.AreEqual(56.00M, result);
        }

        [Test]
        public void CalculateCost_ZeroMessgaesAccruesZeroCost()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, QtyTo = 200, PricePerTextMessage = 0.1M },
                new PriceBand {  QtyFrom = 201, QtyTo = 500, PricePerTextMessage = 0.08M },
                new PriceBand {  QtyFrom = 501, QtyTo = 1000, PricePerTextMessage = 0.06M },
                new PriceBand {  QtyFrom = 1001, PricePerTextMessage = 0.03M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(0);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            // Act
            var result = totalCostCalculator.CalculateCost(Guid.NewGuid(), 3, 2021);

            //Assert
            Assert.AreEqual(0.0M, result);
        }

        [Test]
        public void CalculateCost_CostFromBandWithNoUpperLimitCalculatesForMessagesSent()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, PricePerTextMessage = 0.03M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(90);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            // Act
            var result = totalCostCalculator.CalculateCost(Guid.NewGuid(), 3, 2021);

            //Assert
            Assert.AreEqual(2.7M, result);
        }

        [Test]
        public void CalculateCost_CalculatesCorrectlyWhenMessagesSentIsOnQtyFromBoundry()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, QtyTo = 100, PricePerTextMessage = 0.01M },
                new PriceBand {  QtyFrom = 101, PricePerTextMessage = 0.77M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(101);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            // Act
            var result = totalCostCalculator.CalculateCost(Guid.NewGuid(), 3, 2021);

            //Assert
            Assert.AreEqual(1.77M, result);
        }

        [Test]
        public void CalculateCost_CalculatesCorrectlyWhenMessagesSentIsOnQtyToBoundry()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, QtyTo = 100, PricePerTextMessage = 0.01M },
                new PriceBand {  QtyFrom = 101, PricePerTextMessage = 0.77M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(100);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            // Act
            var result = totalCostCalculator.CalculateCost(Guid.NewGuid(), 3, 2021);

            //Assert
            Assert.AreEqual(1.0M, result);
        }

        [Test]
        public void CalculateCost_CallsNumberOfTextMessagesSentInMonth_WithCorrectParameters()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, PricePerTextMessage = 0.01M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(100);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            var accountId = Guid.NewGuid();
            var monthNumber = 3;
            var yearNumber = 2021;

            // Act
            totalCostCalculator.CalculateCost(accountId, monthNumber, yearNumber);

            //Assert
            mockAccountDetails.Received(1).NumberOfTextMessagesSentInMonth(accountId, monthNumber, yearNumber);
        }

        [Test]
        public void CalculateCost_CallsGetAccountPriceBands_WithCorrectParameters()
        {
            // Arrange
            var priceBands = new List<PriceBand>
            {
                new PriceBand {  QtyFrom = 1, PricePerTextMessage = 0.01M }
            };

            mockAccountDetails.NumberOfTextMessagesSentInMonth(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(100);
            mockAccountDetails.GetAccountPriceBands(Arg.Any<Guid>())
                .Returns(priceBands);

            var accountId = Guid.NewGuid();

            // Act
            totalCostCalculator.CalculateCost(accountId, 2, 2021);

            //Assert
            mockAccountDetails.Received(1).GetAccountPriceBands(accountId);
        }
    }
}
