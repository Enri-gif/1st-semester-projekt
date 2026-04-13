using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
namespace api.Services;

public class AssignmentService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly MongoImageService _mongoImageService;
    private readonly MongoVideoService _mongoVideoService;

    public AssignmentService(ApplicationDbContext dbContext, MongoVideoService mongoVideoService, MongoImageService mongoImageService){
        _dbcontext = dbContext;
        _mongoImageService = mongoImageService;
        _mongoVideoService = mongoVideoService;
    }

    public async Task<Assignment> Create(Assignment assignment){

        _dbcontext.Assignments.Add(assignment);
        await _dbcontext.SaveChangesAsync(new CancellationToken());

        return assignment;
    }

    public async Task<Assignment?> GetById(Guid id){
        return await _dbcontext.Assignments.FindAsync(id);
    }

    public async Task<IEnumerable<Assignment>> GetAll(){
        return await _dbcontext.Assignments.AsNoTracking().ToListAsync();
    }

    public async Task<bool> DeleteAssignment(Guid id){
        Assignment assignment = await _dbcontext.Assignments.FindAsync(id);

        if (assignment == null)
            return false;

        string assignmentId = id.ToString();

        await _mongoImageService.DeleteImagesByAssignmentIdAsync(assignmentId);
        await _mongoVideoService.DeleteVideosByAssignmentIdAsync(assignmentId);

        _dbcontext.Assignments.Remove(assignment);
        await _dbcontext.SaveChangesAsync();

        return true;
    }
}
