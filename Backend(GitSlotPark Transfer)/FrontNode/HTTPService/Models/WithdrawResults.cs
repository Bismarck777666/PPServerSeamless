namespace FrontNode.HTTPService.Models
{
    public enum WithdrawResults
    {
        OK              = 0,
        INVALIDACCOUNT  = 1,
        NOTENOUGHMONEY  = 2,
        PARAMERROR      = 3,
        OTHERERROR      = 4,
    }
}
