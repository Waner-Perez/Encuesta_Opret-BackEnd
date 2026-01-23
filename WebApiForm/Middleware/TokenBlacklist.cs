namespace WebApiForm.Middleware
{
    public static class TokenBlacklist
    {
        private static HashSet<string> _blacklist = new HashSet<string>();

        public static void Add(string token)
        {
            _blacklist.Add(token);
        }

        public static bool IsBlacklisted(string token)
        {
            return _blacklist.Contains(token);
        }
    }
}
