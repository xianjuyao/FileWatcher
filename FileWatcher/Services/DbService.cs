using System.Collections.Generic;
using System.Threading.Tasks;
using FileWatcher.Models;
using SqlSugar;

namespace FileWatcher.Services
{
    public class DbService : IDbService
    {
        private readonly ISqlSugarClient _db;

        public DbService(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<FileOperationLog>> GetFileOperationItemsAsync(int pageIndex, int pageSize)
        {
            return await _db.Queryable<FileOperationLog>().OrderBy(it => it.OperationTime, OrderByType.Desc)
                .ToOffsetPageAsync(pageIndex, pageSize);
        }

        public async Task<int> GetFileOperationItemCountsAsync()
        {
            return await _db.Queryable<FileOperationLog>().CountAsync();
        }

        public async Task<int> GetFileOperationTimeCountsAsync()
        {
            return await _db.Queryable<FileOperationLog>().GroupBy(it =>
                it.OperationTime.Date).Select(it => new
            {
                Time = it.OperationTime.Date
            }).CountAsync();
        }

        public async Task<List<FileOperationTimes>> GetFileOperationTimesAsync(int pageIndex, int pageSize)
        {
            return await _db.Queryable<FileOperationLog>().GroupBy(it =>
                it.OperationTime.Date).Select(it => new FileOperationTimes
            {
                OperationDate = it.OperationTime.Date,
                OperationCount = SqlFunc.AggregateCount(it.OperationTime.Date)
            }).ToOffsetPageAsync(pageIndex, pageSize);
        }

        public async Task<int> InsertFileOperationLogAsync(FileOperationLog log)
        {
            return await _db.Insertable(log).ExecuteCommandAsync();
        }
    }
}