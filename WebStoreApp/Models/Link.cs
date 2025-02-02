namespace WebStoreApp.Models
{
    public class Link
    {
        public string Href { get; set; }  // The URL for the link
        public string Rel { get; set; }   // The relation type (e.g., "self", "update", "delete")
        public string Method { get; set; } // The HTTP method (e.g., GET, POST)

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
