using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FileWatcher.Models;
using LogLib;

namespace FileWatcher.Services
{
    public interface IDbService
    {
         Task<List<FileOperationLog>> GetFileOperationItemsAsync(int pageIndex, int pageSize);
         Task<int> GetFileOperationItemCountsAsync();
         Task<int> GetFileOperationTimeCountsAsync();
         Task<List<FileOperationTimes>> GetFileOperationTimesAsync(int pageIndex, int pageSize);
         Task<int> InsertFileOperationLogAsync(FileOperationLog log);
    }
}