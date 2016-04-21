using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace xsqlite{
    public static class BuilderExtension {
        private static int indent=0;
        private static int lastIndent = 0;
        
        private static void ResetIndent(){
            if (lastIndent>0) {
                indent=lastIndent;
                lastIndent=0;
            }
        }
        public static StringBuilder Head(this StringBuilder code, string text) {
            lastIndent=indent;
            indent=0;
            var space=new string('\t', lastIndent);
            code.AppendFormat("{0}{1}", space, text);
            return code;
        }

        public static StringBuilder Push(this StringBuilder code) {
            if (lastIndent>0) {
                indent = ++lastIndent;
                lastIndent = 0;
            } else {
                indent++;
            }
            return code;
        }
        public static StringBuilder Pop(this StringBuilder code) {
            if (lastIndent>0){
                indent=--lastIndent;
                lastIndent=0;
                Debug.Assert(indent>=0);
            }else{
                indent--;
                Debug.Assert(indent>=0);
            }
            
            
            return code;
        }
        public static StringBuilder Line(this StringBuilder code) {
            code.AppendLine();
            ResetIndent();
            return code;
        }

        static readonly  List<string> keywords=new List<string>{
            "if", "else", "while", "for", "do", "template","class","struct","switch","case","default"
        };

        static readonly List<string> blocks=new List<string>{
            "{", "}"
        };
        static bool AnyIn(this string text,List<string> list){
            return list.Any(i => text.Contains(i));
        }
        static bool AnyEqual(this string text,List<string> list){
            return list.Any(i => i == text);
        }
        static bool IsFuncDeclare(this string text){
            if (text.Contains("=")){
                return false;
            }
            const string regex1="^.*(.*)$";
            if (Regex.Match(text, regex1).Captures.Count>0) {
                return true;
            }

            const string regex = "^\\w+\\s+\\w+::\\w+(.*)$";
            return Regex.Match(text,regex).Captures.Count > 0;
        }
        static bool IsLineBlock(this string text){
            return blocks.Any(b => text.Contains(b));
        }

        private static string functionDeclare;
        static void CheckLine(this string text){
            var realCode=text.Trim();

            if (realCode.IsFuncDeclare()) {
                functionDeclare=realCode;
                return;
            }

            if (!string.IsNullOrEmpty(functionDeclare)) {
                if (realCode=="{")
                    return;
            }

            if ((!realCode.AnyIn(keywords))&&
                (!realCode.AnyEqual(blocks))&&
                (!realCode.StartsWith("//"))&&
                (!realCode.StartsWith("#"))&&
                (!realCode.IsLineBlock())  &&
                (!realCode.EndsWith(";"))
            ) {
                Console.WriteLine("Warning, Statement should end with`;`: {0}", realCode);
            }

            
        }

        public static StringBuilder Line(this StringBuilder code, string text, bool ignoreIndent=false){
            CheckLine(text);

            if (ignoreIndent) {
                code.AppendLine(text);
            } else {
                var space=new string('\t', indent);
                code.AppendFormat("{0}{1}\n", space, text);
            }
            ResetIndent();
            return code;
        }
        public static StringBuilder FormatLine(this StringBuilder code, string format, params object[] args) {
            var space=new string('\t', indent);
            var text=string.Format(format, args);
            CheckLine(text);
            code.AppendFormat("{0}{1}\n", space, text);
            ResetIndent();
            return code;
        }
        public static StringBuilder PushLine(this StringBuilder code, string text) {
            return code.Line(text).Push();
        }
        public static StringBuilder PopLine(this StringBuilder code, string text) {
            return code.Pop().Line(text);
        }
        public static StringBuilder PopLine(this StringBuilder code) {
            return code.Pop().Line();
        }

        public static EachBuilder<T> BeginEach<T>(this StringBuilder code, IEnumerable<T> list) {
            return new EachBuilder<T>(){
                    List = list,
                    Builder = code
            };
        }
        public static EachBuilder<T> Push<T>(this EachBuilder<T> code) {
            var action=new Action<T>(t => code.Builder.Push());
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> PushLine<T>(this EachBuilder<T> code, string text){
            var action=new Action<T>(t => code.Builder.PushLine(text));
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> PopLine<T>(this EachBuilder<T> code, string text) {
            var action=new Action<T>(t => code.Builder.PopLine(text));
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> PopLine<T>(this EachBuilder<T> code) {
            var action=new Action<T>(t => code.Builder.PopLine());
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> Pop<T>(this EachBuilder<T> code) {
            var action=new Action<T>(t => code.Builder.Pop());
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> Line<T>(this EachBuilder<T> code) {
            var action=new Action<T>(t => code.Builder.Line());
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> Line<T>(this EachBuilder<T> code, string text, bool ignoreIndent=false) {
            var action=new Action<T>(t => code.Builder.Line(text, ignoreIndent));
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> code, string format, Func<T,string> args){
            var action=new Action<T>(t => code.Builder.FormatLine(format, args(t)));
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> code, string format, Func<T, string> args1, Func<T, string> args2) {
            var action=new Action<T>(t => code.Builder.FormatLine(format, args1(t),args2(t)));
            code.Actions.Add(action);
            return code;
        }
        public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> code, string format, Func<T, string> args1, Func<T, string> args2, Func<T, string> args3) {
            var action=new Action<T>(t => code.Builder.FormatLine(format, args1(t), args2(t), args3(t)));
            code.Actions.Add(action);
            return code;
        }
        public static StringBuilder EndEach<T>(this EachBuilder<T> code){
            foreach (var item in code.List){
                foreach (var action in code.Actions){
                    action(item);
                }
            }
            return code.Builder;
        }
    }    
}