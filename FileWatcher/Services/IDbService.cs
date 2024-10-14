using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FileWatcher.Models;
using LogLib;

namespace FileWatcher.Services
{
    public interface IDbService
    {
        public Task<List<FileOperationLog>> GetFileOperationItemsAsync(int pageIndex, int pageSize);
        public Task<int> GetFileOperationItemCountsAsync();
        public Task<int> GetFileOperationTimeCountsAsync();
        public Task<List<FileOperationTimes>> GetFileOperationTimesAsync(int pageIndex, int pageSize);
        public Task<int> InsertFileOperationLogAsync(FileOperationLog log);
    }
}