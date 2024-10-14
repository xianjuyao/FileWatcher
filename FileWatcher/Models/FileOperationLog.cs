using System;
using SqlSugar;

namespace FileWatcher.Models
{
    [SugarTable("log")]
    public class FileOperationLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] //数据库是自增才配自增 
        public int Id { get; set; }

        [SugarColumn(InsertSql = "getdate()")] public DateTime OperationTime { get; set; }
        public string OperationType { get; set; }
        public string OperationMessage { get; set; }

        public FileOperationLog(string operationType, string operationMessage)
        {
            OperationType = operationType;
            OperationMessage = operationMessage;
        }

        public FileOperationLog()
        {
        }
    }

    public static class OperationType
    {
        public const string created = "created";
        public const string renamed = "renamed";
        public const string delete = "delete";
        public const string changed = "changed";
    }
}