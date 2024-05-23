using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly CollegeDBContext _dbContext;
        private readonly IMapper _mapper;
        public StudentController(ILogger<StudentController> logger, CollegeDBContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("All")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]        
        public async Task <ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            _logger.LogInformation("Student Methods Started");
            var students = await _dbContext.Students.ToListAsync();

            var studentDTOData = _mapper.Map<List<StudentDTO>>(students);
            return Ok(students);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task <ActionResult<StudentDTO>> GetStudentByIdAsync(int id)
        {
            // bad reqqueest - 400 -Client error
            if (id <= 0)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
            var student =await _dbContext.Students.Where(n  => n.Id == id).FirstOrDefaultAsync();
            if (student == null)
            {
                _logger.LogError("Student not found with given ID");
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
            var studentDTO = _mapper.Map<StudentDTO>(student);
            return Ok( studentDTO);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> CreateStudentsAsync([FromBody]StudentDTO dto)
        {
            if(dto == null)
                return BadRequest();

            Student student = _mapper.Map<Student>(dto);
             await _dbContext.Students.AddAsync(student);
             await _dbContext.SaveChangesAsync();
            dto.Id = student.Id;
            //status 201 - new student details
            return CreatedAtRoute("GetStudentById", new {id = dto.Id}, dto);
           }


        [HttpPut("Update")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task  <ActionResult> UpdateStudentAsync([FromBody] StudentDTO dto)
        {
            if(dto == null ||  dto.Id <= 0)
                return BadRequest();


            var existingStudent =await _dbContext.Students.Where(s=> s.Id  == dto.Id).FirstOrDefaultAsync();
            if (existingStudent == null)
                return NotFound();
            var newRecord= _mapper.Map<Student>(dto);

            //existingStudent.StudentName = model.StudentName;
            //existingStudent.Email = model.Email;
            //existingStudent.Address = model.Address;
            //existingStudent.DOB = model.DOB;

           await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}UpdatePartial")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateStudentPartialAsync(int id,[FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
                return BadRequest();

            var existingStudent = await _dbContext.Students.AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
            if (existingStudent == null)
                return NotFound();

            var studentDTO = _mapper.Map<StudentDTO>(existingStudent);

            patchDocument.ApplyTo(studentDTO, ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent = _mapper.Map<Student>(studentDTO);
            _dbContext.Students.Update(existingStudent);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }





        [HttpGet("{name:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<StudentDTO>> GetStudentByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            var student =await _dbContext.Students.Where(n => n.StudentName == name).FirstOrDefaultAsync();
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {name} not found");
            }

            var studentDTO = _mapper.Map<StudentDTO>(student);

            //ok- 200 success
            return Ok(studentDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]  // success ok
        [ProducesResponseType(400)]  // bad req client error
        [ProducesResponseType(404)] // bad request client error not found
        [ProducesResponseType(500)] // server error
        public async Task<ActionResult<bool>> DeleteStudentsAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var student = await _dbContext.Students.Where(n => n.Id == id).FirstOrDefaultAsync();
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
             _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();
            //ok- 200 success
            return Ok(true);
        }
    }
}
