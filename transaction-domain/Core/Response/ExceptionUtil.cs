namespace transaction_domain.Core.Response
{
    public static class ExceptionUtil
    {
        public static string GetMessage(Exception ExceptionOrigin)
        {
            return $"[{ExceptionOrigin.Source ?? "None"}] has generated the following exception: ${ExceptionOrigin.InnerException!.Message ?? ExceptionOrigin.Message}";
        }

    }
}
