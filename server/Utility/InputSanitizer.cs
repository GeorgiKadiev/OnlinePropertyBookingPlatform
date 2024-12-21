using Ganss.Xss;

namespace OnlinePropertyBookingPlatform.Utility
{
    public class InputSanitizer
    {
        private readonly HtmlSanitizer _sanitizer;

        public InputSanitizer()
        {
            _sanitizer = new HtmlSanitizer();

            // Опционално: Позволяване на специфични HTML тагове и атрибути
            //_sanitizer.AllowedTags.Add("b");
            //_sanitizer.AllowedTags.Add("i");
            //_sanitizer.AllowedTags.Add("strong");
            //_sanitizer.AllowedTags.Add("a");
            //_sanitizer.AllowedAttributes.Add("href");

            //следните 2 реда са за никакви тагове!
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedAttributes.Clear();
        }

        public string Sanitize(string input)
        {
            Console.WriteLine($"Before Sanitization: {input}");
            string sanitized = string.IsNullOrEmpty(input) ? input : _sanitizer.Sanitize(input);
            Console.WriteLine($"After Sanitization: {sanitized}");
            return sanitized;
        }
    }
}
