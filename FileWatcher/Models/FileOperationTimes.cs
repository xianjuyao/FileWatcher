using SqlSugar;
using System;

namespace FileWatcher.Models
{
    /**
     *
     *记录操作的次数
     * day日期
     * count 次数
     *
     */
    public class FileOperationTimes
    {

        public DateTime OperationDate { get; set; }
        public int OperationCount { get; set; }

        public override string ToString()
        {
            return $"day={OperationDate}, count={OperationCount}";
        }
    }
}