namespace PointOfSale.Utilities.Regex
{
    public static class RegexPattern
    {
        public const string INTEGER_ONLY = @"^\d+$";
        public const string DECIMAL_ONLY = @"^\d+(\.\d{1,2})?$";
        public const string ALPHANUMERIC = @"^[a-zA-Z0-9]+$";
        public const string LETTERS_SPACES = @"^[a-zA-Z\s]+$";
        public const string EMAIL = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    }
}
