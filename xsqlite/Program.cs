/**
 * Description： sqlite c++ orm
 * Author：范飞龙
 * 2016-01-30
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace xsqlite {
    class Program {
        public static void Main(string[] args) {
            if (args.Length!=2){
                PrintUseage();
                return;
            }
            try{
                // Parse Config
                Config.Namespace=args[0];
                Config.TableFile = Path.Combine(Environment.CurrentDirectory,args[1]);
                if(Path.GetExtension(Config.TableFile)!=".xsqlite"){
                    PrintUseage();
                    return;
                }

                // Run
                Run();
                
                Console.WriteLine("Done, Press Any Key to Quit.");
                Console.Read();
            }catch(Exception e){
                Console.WriteLine(e.Message);
            }
        }

        private static void Run(){
            var outputDir=Path.GetDirectoryName(Config.TableFile);
            var name = Path.GetFileNameWithoutExtension(Config.TableFile);
            var defFile=Path.Combine(outputDir, string.Format("{0}.h",name));
            var implFile=Path.Combine(outputDir, string.Format("{0}.cpp",name));
            var tables=Config.TableFile.Parse();
           
            tables.Emit("xsqlite by fanfeilong",Config.Namespace,name,defFile,implFile);
        }

        private static void Test(){
            // ----------------------------------------------------------------------------
            // Sqlite DSL (1:1) 
            // Table    : {"Table","i32:field1,bytes:field2,i64:field3"}
            // primary  : {"Table","i32:field1+p,bytes:field2,i64:field3"}
            // search   : {"Table","i32:field1+p+sk1,bytes:field2,i64:field3+sv1"}
            // update   : {"Table","i32:field1+p+uk1,bytes:field2+uv1,i64:field3"}
            // delete   : {"Table","i32:field1+p+r1,bytes:field2,i64:field3"}
            // ----------------------------------------------------------------------------
            var tables=new Dictionary<string, string>{
                {"File","i32:file_id+p+uk1, bytes:path, bytes:hash, i64:create_time, i64:last_scan_time+uv1, i32:flag+uv1"},
                {"Chunk","bytes:chunk_id+p+sk1,i32:group_id+p+sk1,i64:offset,i32:size,i32:file_id+sv1+r1"},
                {"Piece","bytes:chunk_id+p+sk1+r1+uk1,i32:group_id+p+sk1+r1+uk1,bytes:box_peerid+p+uk1+sv1,i32:seq+sv1,i64:offset+sv1,i32:size+sv1,i32:file_id+sv1+r2,i32:status+uv1+sv1"},
                {"TaskSession","i32:task_id+p,i32:task_type,bytes:trace_id,i32:group_id,i32:set_id,i32:k,i32:quorum,bytes:chunk_id,i64:chunk_size,i64:create_time,i64:time_to_live,i64:cache_time,bytes:info"},
                {"FountainFile","bytes:chunk_id+p,i32:group_id+p,i32:piece_size,i64:create_time,i64:time_to_live,i32:file_id+r1"}
            };
        }
        
        private static void PrintUseage() {
            Console.WriteLine("Useage: xsqlite.exe  Test ./Test/db.xsqlite");
        }
    }
}
