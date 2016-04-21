using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace xprotocol {
    public static class TemplateExtension {
        public static string AsLinqCode(this string source) {
            var lines=source.Split(new char[] { '\r', '\n' });
            var sb=new StringBuilder();
            var depth=0;
            foreach (var l in lines) {
                var s=l.Trim();

                int count=0;
                s=Regex.Replace(s, @"\$\([^$()]*\)", m => string.Format("{{{0}}}", count++));

                if (s=="{") {
                    sb.AppendLine(".Line(\"{\")");
                    depth++;
                } else if (s=="}") {
                    sb.AppendLine(".Line(\"}\")");
                    depth--;
                } else if (!string.IsNullOrEmpty(s)) {
                    if (depth>0) {
                        sb.Append(new string('\t', depth));
                    }
                    if (count>0) {
                        sb.Append(".FormatLine(\""+s+"\"");
                        while (count-->0) {
                            sb.Append(",\"\"");
                        }
                        sb.Append(")\n");
                    } else {
                        sb.AppendLine(".Line(\""+s+"\")");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
