using System;

namespace JustTrack {
    public class Money {
        public double Value { get; set; }
        public string Currency { get; set; }

        public Money(double pValue, string pCurrency) {
            this.Value = pValue;
            this.Currency = pCurrency;
        }
    }
}
