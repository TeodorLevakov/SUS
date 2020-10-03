namespace SUS.HTTP
{
    public class Cookie
    {
        public Cookie(string cookieAsStr)
        {
            var cookieParts = cookieAsStr.Split(new char[] { '=' }, 2);

            this.Name = cookieParts[0];
            this.Value = cookieParts[1];
        }
        public string Name { get; set; }

        public string Value { get; set; }
    }
}