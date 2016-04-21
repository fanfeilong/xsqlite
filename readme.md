xsqlite
=========
sqlite c++ orm

Usage:
==========
xsqlite.exe prefix xsqlitefile

Example:
==========
xsqlite.exe test ./Test/db.xsqlite

DSL Description:
=========
table tablename
{
   type:field1+label1+label2;
   type:field2+label1+label2;
}

sql queryname
{
   "querystatement";
}

label primary key:
------------------

add `+p` in the field's label region 

default generate api：
===========
- insert record: InsertXXX
- find all records in table: BeginFindXXX MoveNextXXX EndFindXXX
- find records in table by primary key: BeginFindXXX MoveNextXXX EndFindXXX
- remove records in table by primary key: RemoveXXX


label simple search query in table： 
------------------

assume we have the following simple query in table.

"select field3,field4 from table where field1=1? and field2=2?" 

take the search quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+ski` 
- leble field3 and field3 with `+svi` 

which will then generate correspoding c++ query functions: BeginFindXXX,MoveNextXXX,EndFindXXX, where XXX is the table name.

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

"delete from table where field1=1? and field2=2?"

take the delete quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+ri`

which will then generate correspoding c++ query functions: RemoveXXX, where XXX is the table name.

for example:
```
// "delete from table where hash=1? and pos=2?"
table DataRange
{
   bytes:hash+p+r1;
   i32:seq+p;
   i32:file_id;
   i64:pos+sv1+r1;
   i32:length+sv1;
}
```

label simple update query in table:
------------------

assume we have the following simple query in table.

"update table set field1=3? and field4=4? where field1=1? and field2=2? " 

take the update quey an id i(1,2,...), we can label the query by：
- label field1 and field2 with `+uki`
- label field3 and field4 with `+uvi`
which will then generate correspoding c++ query functions: UpdateXXX, where XXX is the table name.

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

xsqlite DO NOT Support complex query ORM, but you can declare it and then write it by yourself, for example
we can declare the following query statement by id SQL_FindEarlestDataBlock:

```
sql SQL_FindEarlestDataBlock
{
         "SELECT hash, file_id, min(create_time) FROM DataBlock";
}
```

suppose the generated class is db, 
and then we can write the implement by uplevel class DataBaseWrapper, which will use the genetated api.

```
std::string path=...
db* pDb = new db(path);
pDb->Open();

...

int FindEarlestDataBlock(std::string& hash, uint32_t& file_id,uint64_t& create_time)
{
    int ret = SQLITE_OK;
    sqlite3_stmt* pStmt = pDb->GetSqliteStatement(SQL_FindEarlestDataBlock);
    VERIFY_RET_BY("FindEarlestDataBlock", "get stmt", pStmt != NULL, XPF_RESULT_FAILED);

    ret = sqlite3_reset(pStmt);
    VERIFY_RET("FindEarlestDataBlock", "sqlite3_reset", ret);

    ret = sqlite3_step(pStmt);
    VERIFY_RET("FindEarlestDataBlock", "sqlite3_step", ret);

    const char* hash_value = (const char*)(sqlite3_column_text(pStmt, 0));
    if (hash_value != NULL) {
          hash.assign(hash_value, strlen(hash_value));
    }
    file_id = sqlite3_column_int(pStmt, 1);
    create_time = (int64_t)sqlite3_column_int64(pStmt,2);

    return XPF_RESULT_SUCCESS;
}

```
