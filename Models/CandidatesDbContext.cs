using Microsoft.EntityFrameworkCore;

public class CandidatesDbContext(DbContextOptions<CandidatesDbContext> options) : DbContext(options)
{
    public required DbSet<Candidate> Candidates { get; set; }
}
