using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
namespace api.Services;

public class MongoImageService
{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;

    public MongoImageService(IConfiguration config)
    {
        MongoClient client = new MongoClient(config["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(config["MongoDb:Database"]);
        _gridFS = new GridFSBucket(_database);
    }

    // Upload an image
    public async Task<ObjectId> UploadImageAsync(byte[] fileBytes, string fileName, string assignmentId)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "assignmentId", assignmentId },
                { "fileName", fileName }
            }
        };

        var id = await _gridFS.UploadFromBytesAsync(fileName, fileBytes, options);
        return id;
    }

    // Download an image by file Id
    public async Task<byte[]> DownloadImageAsync(ObjectId fileId)
    {
        return await _gridFS.DownloadAsBytesAsync(fileId);
    }

    // List images for an assignment
    public async Task<List<GridFSFileInfo>> GetImagesByAssignmentAsync(string assignmentId)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq("metadata.assignmentId", assignmentId);
        var files = await _gridFS.FindAsync(filter);
        return await files.ToListAsync();
    }
}

