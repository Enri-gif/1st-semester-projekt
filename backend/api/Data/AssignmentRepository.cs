using api.Models;

namespace api.Data;

public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext db) : base(db)
    {
    }
}
