using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace api.Services;

public interface IMongoImageService
{
    Task<ObjectId> UploadImageAsync(byte[] fileBytes, string fileName, string assignmentId, string contentType = "image/jpeg");
    Task<byte[]> DownloadImageAsync(ObjectId fileId);
    Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId);
    Task<List<string>> GetImagesByAssignmentIdAsync(string assignmentId);
    Task<List<string>> GetAllImagesAsync();
    Task DeleteImagesByAssignmentIdAsync(string assignmentId);
}
