using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace api.Services;

public class MongoImageService : IMongoImageService{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;
    private readonly string _apiBaseUrl;

    public MongoImageService(IConfiguration config){
        MongoClient client = new MongoClient(config["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(config["MongoDb:Database"]);
        _gridFS = new GridFSBucket(_database);
        
        _apiBaseUrl = config["ApiBaseUrl"] ?? "https://localhost:5001";
    }

    public async Task<ObjectId> UploadImageAsync(byte[] fileBytes, string fileName, string assignmentId, string contentType = "image/jpeg"){
        GridFSUploadOptions options = new GridFSUploadOptions{
            Metadata = new BsonDocument{
                { "assignmentIds", assignmentId },
                { "fileName", fileName },
                { "contentType", contentType }
            }
        };

        ObjectId id = await _gridFS.UploadFromBytesAsync(fileName, fileBytes, options);
        return id;
    }

    public async Task<byte[]> DownloadImageAsync(ObjectId fileId){
        return await _gridFS.DownloadAsBytesAsync(fileId);
    }

    public async Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId){
        FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
        IAsyncCursor<GridFSFileInfo> cursor = await _gridFS.FindAsync(filter);
        return await cursor.FirstOrDefaultAsync();
    }

    private async Task<List<string>> GetImageUrlsAsync(FilterDefinition<GridFSFileInfo> filter){
        IAsyncCursor<GridFSFileInfo> cursor = await _gridFS.FindAsync(filter);
        List<string> imageUrlList = new List<string>();

        await cursor.ForEachAsync(file =>{
            string url = $"{_apiBaseUrl}/api/images/{file.Id}";
            imageUrlList.Add(url);
        });

        return imageUrlList;
    }

    public Task<List<string>> GetImagesByAssignmentIdAsync(string assignmentId){
        FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.Eq("metadata.assignmentIds", assignmentId);
        return GetImageUrlsAsync(filter);
    }

    public Task<List<string>> GetAllImagesAsync(){
        return GetImageUrlsAsync(Builders<GridFSFileInfo>.Filter.Empty);
    }
    public async Task DeleteImagesByAssignmentIdAsync(string assignmentId){
        FilterDefinition<GridFSFileInfo> filter =
            Builders<GridFSFileInfo>.Filter.Eq("metadata.assignmentIds", assignmentId);

        IAsyncCursor<GridFSFileInfo> cursor = await _gridFS.FindAsync(filter);
        List<GridFSFileInfo> files = await cursor.ToListAsync();

        foreach (GridFSFileInfo file in files){
            await _gridFS.DeleteAsync(file.Id);
        }
    }
}
