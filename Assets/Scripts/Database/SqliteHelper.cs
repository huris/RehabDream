using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Security.Cryptography;
using System.Text;

public class SQLiteHelper
{
    /// <summary>
    /// 数据库连接定义
    /// </summary>
    private SqliteConnection dbConnection;

    /// <summary>
    /// SQL命令定义
    /// </summary>
    private SqliteCommand dbCommand;

    /// <summary>
    /// 数据读取定义
    /// </summary>
    private SqliteDataReader dataReader;

    public SqliteCommand GetSqliteCommand()
    {
        return this.dbCommand;
    }

    public SqliteConnection GetSqliteConnection()
    {
        return this.dbConnection;
    }

    /// <summary>
    /// 构造函数    
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SQLiteHelper(string connectionString)
    {
        try
        {
            //构造数据库连接
            dbConnection = new SqliteConnection(connectionString);
            //打开数据库
            dbConnection.Open();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SqliteDataReader ExecuteQuery(string queryString)
    {
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = queryString;
        dataReader = dbCommand.ExecuteReader();

        //Debug.Log("@SqliteHelper: "+ queryString);
        return dataReader;
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void CloseConnection()
    {
        //销毁Command
        if (dbCommand != null)
        {
            dbCommand.Cancel();
        }
        dbCommand = null;

        //销毁Reader
        if (dataReader != null)
        {
            dataReader.Close();
        }
        dataReader = null;

        //销毁Connection
        if (dbConnection != null)
        {
            dbConnection.Close();
        }
        dbConnection = null;
    }

    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    public SqliteDataReader ReadFullTable(string tableName)
    {
        string queryString = "SELECT * FROM " + tableName;
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public SqliteDataReader InsertValues(string tableName, string[] values)
    {
        //获取数据表中字段数目
        int fieldCount = ReadFullTable(tableName).FieldCount;
        //当插入的数据长度不等于字段数目时引发异常
        if (values.Length != fieldCount)
        {
            throw new SqliteException("values.Length!=fieldCount");
        }
        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在values中的字符串类型需要添加单引号
        string queryString = "INSERT INTO " + tableName + " VALUES (" +  values[0] ;
        for (int i = 1; i < values.Length; i++)
        {
            queryString += ", " + values[i];
        }
        queryString += " )";
        return ExecuteQuery(queryString);
    }

    public bool IsTableExists(string tableName)
    {

        SqliteDataReader reader = ExecuteQuery("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'");
        reader.Read();

        return reader.GetInt32(0) != 0;
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字对应的值</param>
    public SqliteDataReader UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length");
        }

        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues value 中的字符串类型需要添加单引号
        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += ", " + colNames[i] + "=" + colValues[i];
        }
        queryString += " WHERE " + key + operation + value;
        //Debug.Log("@SqliteHelper: UpdateValues "+queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="keys">关键字</param>
    /// <param name="values">关键字对应的值</param>
    public SqliteDataReader UpdateValuesAND(string tableName, string[] colNames, string[] colValues, string[] keys, string[] operations, string[] values)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || keys.Length != values.Length || keys.Length != operations.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || keys.Length!=values.Length || keys.Length!=operations.Length");
        }

        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues values 中的字符串类型需要添加单引号
        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += ", " + colNames[i] + "=" + colValues[i];
        }

        queryString += " WHERE " + keys[0] + operations[0] + values[0];
        for (int i = 1; i < keys.Length; i++)
        {
            queryString += " AND " + keys[i] + "=" + values[i];

        }
        Debug.Log("@SqliteHelper: UpdateValuesAND " + queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="keys">关键字</param>
    /// <param name="values">关键字对应的值</param>
    public SqliteDataReader UpdateValuesOR(string tableName, string[] colNames, string[] colValues, string[] keys, string[] operations, string[] values)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || keys.Length != values.Length || keys.Length != operations.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || keys.Length!=values.Length || keys.Length!=operations.Length");
        }

        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues values 中的字符串类型需要添加单引号
        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += ", " + colNames[i] + "=" + colValues[i];
        }

        queryString += " WHERE " + keys[0] + operations[0] + values[0];
        for (int i = 1; i < keys.Length; i++)
        {
            queryString += "OR " + keys[i] + "=" + values[i];
        }
        Debug.Log("@SqliteReader: UpdateValuesOR " + queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues 中的字符串类型需要添加单引号
        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += "OR " + colNames[i] + operations[i] + colValues[i];
        }
        Debug.Log("@SqliteHelper: DeleteValuesOR " + queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SqliteDataReader DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues 中的字符串类型需要添加单引号
        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += " AND " + colNames[i] + operations[i] + colValues[i];
        }
        Debug.Log("@SqliteHelper: DeleteValuesAND " + queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public SqliteDataReader CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string queryString = "CREATE TABLE " + tableName + "( " + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += "  ) ";
        Debug.Log("@SqliteHelper: CreateTable " + queryString);
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    public SqliteDataReader ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items[0];
        for (int i = 1; i < items.Length; i++)
        {
            queryString += ", " + items[i];
        }
        // 表中已经设置成int类型的不需要再次添加‘单引号’，而字符串类型的数据需要进行添加‘单引号’
        // 即在colValues 中的字符串类型需要添加单引号

        if (colNames.Length == 0)
        {
            queryString += " FROM " + tableName;
            return ExecuteQuery(queryString);
        }

        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[i] + " ";
        }
        Debug.Log("@SqliteHelper: ReadTable " + queryString);
        return ExecuteQuery(queryString);
    }

    // 使用MD5加密字符串
    public static string MD5Encrypt(string str)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(str));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));

        }
        return tmp.ToString();
    }


}