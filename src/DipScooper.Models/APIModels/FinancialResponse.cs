namespace DipScooper.Models.APIModels
{
    public class FinancialsResponse
    {
        public BalanceSheet BalanceSheet { get; set; }
        public IncomeStatement IncomeStatement { get; set; }
        public CashFlowStatement CashFlowStatement { get; set; }

        public FinancialsResponse(BalanceSheet balanceSheet, IncomeStatement incomeStatement, CashFlowStatement cashFlowStatement)
        {
            BalanceSheet = balanceSheet;
            IncomeStatement = incomeStatement;
            CashFlowStatement = cashFlowStatement;
        }
    }
}
