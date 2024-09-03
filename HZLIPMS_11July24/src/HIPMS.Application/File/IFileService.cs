using HIPMS.File.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.File;

public interface IFileService
{
    Task<bool> CreateAsync(CreateFileRequestDto input, CancellationToken cancellationToken);
    Task<List<FileRequest>> GetReqItemDocsAsync(long RequestId, int type);
}
