using System.Text.RegularExpressions;

namespace xsqlite {
    public static class RegexExtension {
        public static string ValueAt(this Match match, int index) {
            if (index>=match.Groups.Count) {
                return "";
            }
            return match.Groups[index].Value;
        }
    }
}
