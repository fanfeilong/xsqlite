using System;
using System.Text;

namespace xsqlite {
    public static class BuilderCPPExtension {
        public static StringBuilder Include(this StringBuilder code,string header){
            // TODO: Check File Exsist
            if (header.Trim().StartsWith("<")) {
                code.Line(string.Format("#include {0}", header), true);
            }else{
                code.Line(string.Format("#include \"{0}\"", header), true);
            }
            
            return code;
        }
        public static StringBuilder Include(this StringBuilder code, string format,string arg) {
            // TODO: Check File Exsist
            if (format.Trim().StartsWith("<")) {
                code.FormatLine(string.Format("#include {0}", format),arg,true);
            } else {
                code.FormatLine(string.Format("#include \"{0}\"", format), arg,true);
            }

            return code;
        }
        public static EachBuilder<T> Include<T>(this EachBuilder<T> code, string header) {
            // TODO: Check File Exsist
            if (header.Trim().StartsWith("<")) {
                code.Line(string.Format("#include {0}", header), true);
            } else {
                code.Line(string.Format("#include \"{0}\"", header), true);
            }
            return code;
        }
        public static EachBuilder<T> Include<T>(this EachBuilder<T> code, string format,Func<T,string> args) {
            // TODO: Check File Exsist
            if (format.Trim().StartsWith("<")){
                var finalFormat=string.Format("#include {0}", format);
                code.FormatLine(finalFormat, args);
            } else {
                var finalFormat=string.Format("#include \"{0}\"", format);
                code.FormatLine(finalFormat, args);
            }
            return code;
        }
        public static EachBuilder<T> Include<T>(this EachBuilder<T> code, string format, Func<T, string> args1, Func<T, string> args2) {
            // TODO: Check File Exsist
            if (format.Trim().StartsWith("<")) {
                var finalFormat=string.Format("#include {0}", format);
                code.FormatLine(finalFormat, args1,args2);
            } else {
                var finalFormat=string.Format("#include \"{0}\"", format);
                code.FormatLine(finalFormat, args1, args2);
            }
            return code;
        }
        public static EachBuilder<T> Include<T>(this EachBuilder<T> code, string format, Func<T, string> args1, Func<T, string> args2, Func<T, string> args3) {
            // TODO: Check File Exsist
            if (format.Trim().StartsWith("<")) {
                var finalFormat=string.Format("#include {0}", format);
                code.FormatLine(finalFormat, args1, args2, args3);
            } else {
                var finalFormat=string.Format("#include \"{0}\"", format);
                code.FormatLine(finalFormat, args1, args2, args3);
            }
            return code;
        }
        public static StringBuilder BeginProgramOnce(this StringBuilder code,string name){
            var hTag = string.Format("__{0}_{1}_H__", Config.ShortNamespace, name.ToUpper());
            code.Line(string.Format("#ifndef {0}", hTag), true)
                .Line(string.Format("#define {0}", hTag), true)
                .Line();
            return code;
        }
        public static StringBuilder EndProgramOnce(this StringBuilder code, string name) {
            var hTag=string.Format("__{0}_{1}_H__", Config.ShortNamespace, name.ToUpper());
            code.Line(string.Format("#endif // {0}", hTag), true)
                .Line();
            return code;
        }
        public static StringBuilder IfElse(this StringBuilder code,string c,Action<StringBuilder> ifa,Action<StringBuilder> elsea){
            code.FormatLine("if({0})",c)
                .PushLine("{");

            ifa(code);
    
            code.PopLine("}")
                .Line("else")
                .PushLine("{");

            elsea(code);

            code.PopLine("}");
            return code;
        }
    }
}
