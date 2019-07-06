﻿namespace TrademarkHistoryAnalysis.Models
{
    public class Classification
    {
        public int InternationalCode { get; private set; }
        public string GoodsAndServices { get; private set; }

        public Classification(int internationalCode, string goodsAndServices)
        {
            InternationalCode = internationalCode;
            GoodsAndServices = goodsAndServices;
        }
    }
}
