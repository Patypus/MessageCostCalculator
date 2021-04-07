using System;
using System.Linq;

namespace CostCalculator
{
    public class TotalCostCalculator : ITotalCostCalculator
    {
        private readonly IAccountDetails _accountDetails;

        public TotalCostCalculator(IAccountDetails accountDetails)
        {
            _accountDetails = accountDetails ?? throw new ArgumentNullException(nameof(accountDetails));
        }

        public decimal CalculateCost(Guid customerAccountId, int monthNumber, int yearNumber)
        {
            var totalMonthlyMessages = _accountDetails.NumberOfTextMessagesSentInMonth(customerAccountId, monthNumber, yearNumber);
            var priceBands = _accountDetails.GetAccountPriceBands(customerAccountId);

            var result = priceBands.Where(band => band.QtyFrom <= totalMonthlyMessages)
                .Sum(band => CalculateCostOfBand(band, totalMonthlyMessages));

            return result;
        }

        private decimal CalculateCostOfBand(PriceBand band, int totalMonthlyMessages)
        {
            return band.QtyTo.HasValue && totalMonthlyMessages > band.QtyTo
                    ? FullCostOfBand(band)
                    : PartialCostOfBand(band, totalMonthlyMessages);
        }

        private decimal FullCostOfBand(PriceBand band)
        {
            var totalMessagesInBand = band.QtyTo.Value - (band.QtyFrom - 1);

            return totalMessagesInBand * band.PricePerTextMessage;
        }

        private decimal PartialCostOfBand(PriceBand band, int totalMonthlyMessages)
        {
            var totalMessagesSentInBand = totalMonthlyMessages - (band.QtyFrom - 1);

            return totalMessagesSentInBand * band.PricePerTextMessage;
        }
    }
}
