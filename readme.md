xsqlite
=========
sqlite c++ orm

Usage:
==========
```
xsqlite.exe prefix xsqlitefile
xsqlite.exe test ./Test/db.xsqlite
```

DSL:
=========
```
table tablename
{
   type:field1+label1+label2;
   type:field2+label1+label2;
}

sql queryname
{
   in:table1.field1,table2,field2;
   out:table1.field3,table2.field4,;
   exp:"querystatement";
}
```

label primary key:
------------------

add `+p` in the field's label region 

default generate API：
---------------------
- insert record: `InsertXXX`
- find all records in table: `BeginFindXXX` `MoveNextXXX` `EndFindXXX`
- find records in table by **primary key**: `BeginFindXXX` `MoveNextXXX` `EndFindXXX`
- remove records in table by **primary key**: `RemoveXXX`


label simple search query in table： 
------------------

assume we have the following simple query in table.

```
"select field3,field4 from table where field1=1? and field2=2?" 
```

take the search quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+ski` 
- leble field3 and field3 with `+svi` 

which will then generate correspod c++ query functions: `BeginFindXXX`,`MoveNextXXX`,`EndFindXXX`, where XXX is the table name.

for example:
```
// "select file_d from DataBlock where hash=1?"
table DataBlock
{
   bytes:hash+p+sk1;
   i32:file_id+sv1;
   i64:create_time;
   i64:time_to_live;
   i32:length;
   i32:range_count;
}
```

label simple delete query in table
------------------

assume we have the following simple query in table.
```
"delete from table where field1=1? and field2=2?"
```

take the delete quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+ri`

which will then generate correspod c++ query functions: `RemoveXXX`, where XXX is the table name.

for example:
```
// "delete from table where hash=1? and pos=2?"
table DataRange
{
   bytes:hash+p+r1;
   i32:seq+p;
   i32:file_id;
   i64:posr1;
   i32:length;
}
```

label simple update query in table:
------------------

assume we have the following simple query in table.
```
"update table set field1=3? and field4=4? where field1=1? and field2=2? " 
```
take the update quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+uki`
- label field3 and field4 with `+uvi`
which will then generate correspod c++ query functions: `UpdateXXX`, where XXX is the table name.

for example:
```
// "update table set last_scan_time=2? where file_id=1?"
table File
{
   i32:file_id+p+uk1;
   bytes:path;
   bytes:hash;
   i64:create_time;
   i64:last_scan_time+uv1;
   i32:flag+uv1;
}
```

other query statements：
------------------

otherwise, you can declare sql by the following statements, which is more clear and easy.

```
sql FindEarlestDataBlock
{
  in:DataBlock.length;
  out:DataBlock.hash,DataBlock.file_id,DataBlock.create_time;
  exp:"SELECT hash, file_id, min(create_time) FROM DataBlock WHERE length<=1?";
}
```

####

```
sql FetchDataBlockStoreInfo
{
  out:DataBlock.hash,DataBlock.file_id,File.path;
  exp:"SELECT DataBlock.hash, DataBlock.file_id, File.path FROM DataBlock, File WHERE Datablock.hash=File.hash";
}
```
