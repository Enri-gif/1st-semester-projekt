using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace api.Services;

public class AssignmentService
{
    private readonly ApplicationDbContext _dbcontext;

    public AssignmentService(ApplicationDbContext dbContext){
        _dbcontext = dbContext;
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

}
