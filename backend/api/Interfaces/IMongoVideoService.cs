using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace api.Interfaces;

public interface IMongoVideoService
{
    Task<ObjectId> UploadVideoAsync(byte[] fileBytes, string fileName, List<string> assignmentIds, string contentType = "video/mp4");
    Task<byte[]> DownloadVideoAsync(ObjectId fileId);
    Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId);
    Task<List<string>> GetVideosByAssignmentIdAsync(string assignmentId);
    Task<List<string>> GetAllVideosAsync();
    Task DeleteVideosByAssignmentIdAsync(string assignmentId);
}
