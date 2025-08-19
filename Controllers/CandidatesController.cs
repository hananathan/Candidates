using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly CandidatesDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CandidatesController(CandidatesDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

   // POST: api/candidates/upload
   [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] CandidateUploadModel model)
    {
        if (model.DetailsFile == null || model.ImageFile == null)
            return BadRequest("Both files are required.");

        // Read details file (assume JSON for this example)
        Candidate? candidate;
        using (var stream = model.DetailsFile.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            var json = await reader.ReadToEndAsync();
            candidate = System.Text.Json.JsonSerializer.Deserialize<Candidate>(json);
        }
        if (candidate == null)
            return BadRequest("Invalid details file.");

        // Save image
        var uploads = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploads);
        var imageFileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
        var imagePath = Path.Combine(uploads, imageFileName);
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await model.ImageFile.CopyToAsync(stream);
        }
        candidate.ImageFileName = imageFileName;

        // Save to DB
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        return Ok(new { candidate.Id });
    }

    //GET: api/candidates/search? age = 30 & nationality = USA
   [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] int age, [FromQuery] string nationality)
    {
        var matches = await _context.Candidates
            .Where(c => c.Age == age && c.Nationality == nationality)
            .ToListAsync();

        return Ok(matches);
    }
}
