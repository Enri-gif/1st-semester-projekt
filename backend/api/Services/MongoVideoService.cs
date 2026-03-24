using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace api.Services;

public class MongoVideoService{

    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;

    public MongoVideoService(IConfiguration config){

        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(config["MongoDb:VideoDatabase"]);

        _gridFS = new GridFSBucket(_database, new GridFSBucketOptions{
            BucketName = "videos"
        });
    }

    public async Task<ObjectId> UploadVideoAsync(byte[] fileBytes, string fileName, List<string> assignmentIds){

        var options = new GridFSUploadOptions{

            Metadata = new BsonDocument{

                { "fileName", fileName },
                { "assignmentIds", new BsonArray(assignmentIds) }
            }
        };

        return await _gridFS.UploadFromBytesAsync(fileName, fileBytes, options);
    }

    public async Task<byte[]> DownloadVideoAsync(ObjectId fileId){

        return await _gridFS.DownloadAsBytesAsync(fileId);
    }

    public async Task<List<GridFSFileInfo>> GetVideosByAssignmentAsync(string assignmentId){
        
        var filter = Builders<GridFSFileInfo>.Filter.AnyEq("metadata.assignmentIds", assignmentId);
        var files = await _gridFS.FindAsync(filter);
        return await files.ToListAsync();
    }
}
