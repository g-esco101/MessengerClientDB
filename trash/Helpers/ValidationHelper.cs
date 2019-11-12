namespace MessengerClientDB.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsNullEmptyWhiteSpace(string a, string b = "second", string c = "third")
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrWhiteSpace(a) || string.IsNullOrEmpty(b) || string.IsNullOrWhiteSpace(b) || string.IsNullOrEmpty(c) || string.IsNullOrWhiteSpace(c))
            {
                return true;
            }
            return false;
        }

    }

}