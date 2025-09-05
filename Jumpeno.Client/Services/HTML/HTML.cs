namespace Jumpeno.Client.Utils;

public static class HTML {
    public static class Label {
        // Rendering --------------------------------------------------------------------------------------------------------------------------
        public static RenderFragment Render(OneOf<string, List<string>>? label) => builder => {
            if (label == null) return;
            var l = (OneOf<string, List<string>>)label;
            if (l.IsT0) {
                builder.AddContent(0, l.AsT0);
            } else {
                int seq = 0;
                foreach (var label in l.AsT1) {
                    builder.OpenElement(seq++, "span");
                    builder.AddContent(seq++, label);
                    builder.CloseElement();
                }
            }
        };

        // First label ------------------------------------------------------------------------------------------------------------------------
        public static string First(OneOf<string, List<string>>? label) {
            if (label == null) return "";
            var l = (OneOf<string, List<string>>)label;
            if (l.IsT0) return l.AsT0;
            else return l.AsT1[0];
        }
        public static string AddFirst(string label, OneOf<string, List<string>>? description) {
            if (description == null) return label;
            return $"{label} - {First(description)}";
        }

        // Aria -------------------------------------------------------------------------------------------------------------------------------
        public static string Aria(OneOf<string, List<string>>? label, bool active = false) {
            if (label == null) return "";
            var l = (OneOf<string, List<string>>)label;
            string result = l.IsT0 ? l.AsT0 : l.AsT1[0];
            return active ? $"{result} " : result;
        }
        public static string AriaActive(bool active = false) => $"{active}".ToLower();

        // Option aria ------------------------------------------------------------------------------------------------------------------------
        public static string OptionAria(OneOf<string, List<string>>? label, bool active = false) {
            if (label == null) return "";
            var l = (OneOf<string, List<string>>)label;
            string result = l.IsT0 ? l.AsT0 : l.AsT1[0];
            return active ? $"{result}:  {I18N.T("Selected")}" : result;
        }
        public static string OptionAriaActive(bool active = false) => "false";
    }
}
