using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace xsqlite {
    public static class Parser {
        public static DataBase Parse(this string file){
            var lines = file.Format();
            var tableInfos=new Dictionary<string, List<Field>>();
            Table table = null;
            Query query = null;
            var db = new DataBase();
            db.Name = Path.GetFileNameWithoutExtension(file);

            var enterTable = false;
            var enterFreeQuery = false;
            foreach (var line in lines){

                if(line=="{"){
                    if(enterTable){
                        Debug.Assert(table!=null);    
                    }
                    if(enterFreeQuery){
                        Debug.Assert(query!=null);
                    }
                    continue;    
                }   
                     
                if(line=="}"){
                    if(enterTable){
                        db.Tables.Add(table);
                        table=null;    
                    }
                    if(enterFreeQuery){
                        query.Update();
                        db.FreeQuerys.Add(query);
                        query = null;
                    }
                    continue;
                }

                if (line.StartsWith("table ")) {
                    Debug.Assert(table==null);
                    Debug.Assert(query==null);
                    table=new Table() {
                        Name=line.Substring(6).Trim()
                    };
                    enterTable=true;
                    enterFreeQuery = false;
                    continue;
                }

                if(line.StartsWith("sql ")){
                    Debug.Assert(table==null);
                    Debug.Assert(query==null);
                    query = new Query(){
                        Name=line.Substring(4).Trim()
                    };
                    enterFreeQuery = true;
                    enterTable = false;
                    continue;
                }

                Debug.Assert(line.EndsWith(";"));

                if(enterFreeQuery){
                    var l = line.TrimEnd(new char[] { ';' });    
                    if(line.StartsWith("in:")){
                        query.Ins.AddRange(l.Substring(3).Split(new char[] { ',' }).ToFieldInfos());
                    } else if(line.StartsWith("out:")){
                        query.Outs.AddRange(l.Substring(4).Split(new char[] { ',' }).ToFieldInfos());
                    } else if(line.StartsWith("exp:")){
                        query.Statement=line.Substring(4);        
                    } else{
                        //Ignore
                    }
                }
                
                if(enterTable){
                    Debug.Assert(table!=null);

                    var f=line.TrimEnd(new char[] { ';' });
                    var pos=f.IndexOf(":");
                    var stype=f.Substring(0, pos).Trim();
                    var svalue=f.Substring(pos+1).Trim();
                    var labelpos=svalue.IndexOf("+");
                    var sname="";
                    var slable="";

                    var searchKeys=new List<string>();
                    var searchValues=new List<string>();

                    var removeLabels=new List<string>();

                    var updateKeys=new List<string>();
                    var updateValues=new List<string>();

                    if (labelpos>0) {
                        sname=svalue.Substring(0, labelpos).Trim();
                        slable=svalue.Substring(labelpos+1).Trim();
                        var labels=slable.Split(new char[] { '+' }).Select(l => l.Trim());
                        searchKeys=labels.Where(l => l.Contains("sk")).ToList();
                        searchValues=labels.Where(l => l.Contains("sv")).ToList();
                        removeLabels=labels.Where(l => l.Contains("r")).ToList();
                        updateKeys=labels.Where(l => l.Contains("uk")).ToList();
                        updateValues=labels.Where(l => l.Contains("uv")).ToList();
                    } else {
                        sname=svalue;
                        slable="";
                    }
                    table.Fields.Add(new Field() {
                        type=stype,
                        name=sname,
                        primary=slable.Contains("p"),
                        search_keys=searchKeys,
                        search_values=searchValues,
                        remove_labels=removeLabels,
                        update_keys=updateKeys,
                        update_values=updateValues,
                        table = table
                    });
                }

            }

            return db;
        }

        private static IEnumerable<FieldInfo> ToFieldInfos(this IEnumerable<string> fieldInfos){
            foreach (var fieldInfo in fieldInfos){
                var vs = fieldInfo.Split(new char[]{
                        '.'
                });
                yield return new FieldInfo{
                        TableName = vs[0],
                        FieldName = vs[1]
                };
            }
        } 

        public static Dictionary<string, List<Field>> Parse(this Dictionary<string, string> tables) {
            var tableInfos=new Dictionary<string, List<Field>>();
            foreach (var table in tables) {
                var fieldInfos=table.Value.Split(new char[] { ',' });
                var fields=new List<Field>();
                foreach (var f in fieldInfos) {
                    var pos=f.IndexOf(":");
                    var stype=f.Substring(0, pos).Trim();
                    var svalue=f.Substring(pos+1).Trim();
                    var labelpos=svalue.IndexOf("+");
                    var sname="";
                    var slable="";

                    var searchKeys=new List<string>();
                    var searchValues=new List<string>();

                    var removeLabels=new List<string>();

                    var updateKeys=new List<string>();
                    var updateValues=new List<string>();

                    if (labelpos>0) {
                        sname=svalue.Substring(0, labelpos).Trim();
                        slable=svalue.Substring(labelpos+1).Trim();
                        var labels=slable.Split(new char[] { '+' }).Select(l => l.Trim());
                        searchKeys=labels.Where(l => l.Contains("sk")).ToList();
                        searchValues=labels.Where(l => l.Contains("sv")).ToList();
                        removeLabels=labels.Where(l => l.Contains("r")).ToList();
                        updateKeys=labels.Where(l => l.Contains("uk")).ToList();
                        updateValues=labels.Where(l => l.Contains("uv")).ToList();
                    } else {
                        sname=svalue;
                        slable="";
                    }
                    fields.Add(new Field() {
                        type=stype,
                        name=sname,
                        primary=slable.Contains("p"),
                        search_keys=searchKeys,
                        search_values=searchValues,
                        remove_labels=removeLabels,
                        update_keys=updateKeys,
                        update_values=updateValues
                    });
                }
                tableInfos.Add(table.Key, fields);
            }
            return tableInfos;
        }

        private static IEnumerable<string> Format(this string protocolFileName) {
            var allText=File.ReadAllText(protocolFileName);
            var lines=new List<string>();
            var level=0;
            int index=0;
            int count=allText.Length;
            var line=new StringBuilder();
            while (index<count) {
                char c=allText[index++];
                if (c=='\r'||
                    c=='\n') {
                    //line.Append(' ');
                    continue;
                }

                if (c=='{') {
                    var space=new string('\t', level);
                    lines.Add(string.Format("{0}{1}", space, line.ToString().Trim()));
                    lines.Add(string.Format("{0}{{", space));
                    line.Clear();
                    level++;
                    continue;
                }
                if (c=='}') {
                    var preLine=line.ToString().Trim();
                    if (!string.IsNullOrEmpty(preLine)) {
                        var space1=new string('\t', level);
                        lines.Add(string.Format("{0}{1}", space1, preLine));
                    }

                    level--;
                    var space=new string('\t', level);

                    int j=index;
                    bool f=false;
                    while (index<count) {
                        c=allText[j++];
                        if (c==' ') {
                            continue;
                        }
                        if (c==';') {
                            f=true;
                            index=j;
                            break;
                        }
                        break;
                    }
                    lines.Add(string.Format("{0}}}{1}\r\n", space, f?";":""));
                    line.Clear();
                    continue;
                }
                if (c==';') {
                    line.Append(c);
                    var space=new string('\t', level);
                    lines.Add(string.Format("{0}{1}", space, line.ToString().Trim()));
                    line.Clear();
                    continue;
                }

                // Ignore single line comment
                if (c=='/') {
                    int j=index;
                    line.Append(c);
                    while (index<count) {
                        c=allText[j++];
                        if (c=='\r'||
                            c=='\n') {
                            break;
                        }
                    }
                    line.Clear();
                    index=j;
                    continue;
                }
                line.Append(c);
            }
            return lines.Where(l=>!string.IsNullOrWhiteSpace(l)).Select(l=>l.Trim());
        }
    }
}
