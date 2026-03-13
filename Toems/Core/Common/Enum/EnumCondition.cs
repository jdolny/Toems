namespace Toems_Common.Enum
{
    public class EnumCondition
    {
        public enum FailedAction
        {
            MarkNotApplicable = 0,
            MarkSkipped = 1,
            MarkSuccess = 2,
            MarkFailed = 3,
            GotoModule = 4
           
        }
    }
}