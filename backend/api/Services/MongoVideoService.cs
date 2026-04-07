using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace api.Services;

public class MongoVideoService{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;
    private readonly string _apiBaseUrl;

    public MongoVideoService(IConfiguration config){
        MongoClient client = new MongoClient(config["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(config["MongoDb:VideoDatabase"]);

        _gridFS = new GridFSBucket(_database, new GridFSBucketOptions{
            BucketName = "videos"
        });

        _apiBaseUrl = config["ApiBaseUrl"] ?? "https://localhost:5000";
    }

    public async Task<ObjectId> UploadVideoAsync(byte[] fileBytes, string fileName, List<string> assignmentIds, string contentType = "video/mp4"){
        GridFSUploadOptions options = new GridFSUploadOptions{
            Metadata = new BsonDocument{
                { "fileName", fileName },
                { "assignmentIds", new BsonArray(assignmentIds) },
                { "contentType", contentType }
            }
        };
        ObjectId id = await _gridFS.UploadFromBytesAsync(fileName, fileBytes, options);
        return id;
    }

    public async Task<byte[]> DownloadVideoAsync(ObjectId fileId){
        byte[] videoBytes = await _gridFS.DownloadAsBytesAsync(fileId);
        return videoBytes;
    }

    public async Task<GridFSFileInfo> GetFileInfoAsync(ObjectId fileId){
        FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
        IAsyncCursor<GridFSFileInfo> cursor = await _gridFS.FindAsync(filter);
        GridFSFileInfo fileInfo = await cursor.FirstOrDefaultAsync();
        return fileInfo;
    }

    private async Task<List<string>> GetVideoUrlsAsync(FilterDefinition<GridFSFileInfo> filter){
        IAsyncCursor<GridFSFileInfo> cursor = await _gridFS.FindAsync(filter);
        List<string> videoUrlList = new List<string>();

        await cursor.ForEachAsync(file =>{
            string url = $"{_apiBaseUrl}/api/videos/{file.Id}";
            videoUrlList.Add(url);
        });
        return videoUrlList;
    }

    public Task<List<string>> GetVideosByAssignmentIdAsync(string assignmentId){
        FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.AnyEq("metadata.assignmentIds", assignmentId);
        return GetVideoUrlsAsync(filter);
    }

    public Task<List<string>> GetAllVideosAsync(){
        FilterDefinition<GridFSFileInfo> filter = Builders<GridFSFileInfo>.Filter.Empty;
        return GetVideoUrlsAsync(filter);
    }
}
