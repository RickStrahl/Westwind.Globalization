namespace Westwind.Globalization.AspnetCore.Models
{
    public class CreateClassModel
    {
        public string Filename { get; set; }
        public string Namespace { get; set; }
        public string ClassType { get; set; }
        public string[] ResourceSets { get; set; }
    }
}
