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
        public StudentController(ILogger<StudentController> logger, CollegeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("All")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]        
        public ActionResult<IEnumerable<StudentDTO>> GetStudents()
        {
            _logger.LogInformation("Student Methods Started");
            var students = _dbContext.Students.Select(s => new StudentDTO()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Address = s.Address,
                Email = s.Email,
                DOB = s.DOB
            });
            return Ok(students);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            // bad reqqueest - 400 -Client error
            if (id <= 0)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
            var student = _dbContext.Students.Where(n  => n.Id == id).FirstOrDefault();
            if (student == null)
            {
                _logger.LogError("Student not found with given ID");
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Address = student.Address,
                Email = student.Email,
                DOB = student.DOB

            };
            //ok- 200 success
            return Ok( studentDTO);
        }

        [HttpPost("Create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<StudentDTO>> CreateStudents([FromBody]StudentDTO model)
        {
            if(model == null)
                return BadRequest();

            Student student = new Student
            {
                StudentName = model.StudentName,
                Address = model.Address,
                Email = model.Email,
                DOB = model.DOB

            };
            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();
            model.Id = student.Id;
            //status 201 - new student details
            return CreatedAtRoute("GetStudentById", new {id = model.Id}, model);
           }


        [HttpPut("Update")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult UpdateStudent([FromBody] StudentDTO model)
        {
            if(model == null ||  model.Id <= 0)
                return BadRequest();

            var existingStudent = _dbContext.Students.Where(s=> s.Id  == model.Id).FirstOrDefault();
            if (existingStudent == null)
                return NotFound();

            existingStudent.StudentName = model.StudentName;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;
            existingStudent.DOB = model.DOB;

            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}UpdatePartial")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult UpdateStudentPartial(int id,[FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
                return BadRequest();

            var existingStudent = _dbContext.Students.Where(s => s.Id == id).FirstOrDefault();
            if (existingStudent == null)
                return NotFound();

            var studentDTO = new StudentDTO
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Email = existingStudent.Email,
                Address = existingStudent.Address,
                DOB = existingStudent.DOB,
            };

            patchDocument.ApplyTo(studentDTO, ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent.StudentName = studentDTO.StudentName;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Address;
            existingStudent.DOB = studentDTO.DOB;
            _dbContext.SaveChanges();
            return NoContent();
        }





        [HttpGet("{name:alpha}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult< StudentDTO> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            var student = _dbContext.Students.Where(n => n.StudentName == name).FirstOrDefault();
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {name} not found");
            }

            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Address = student.Address,
                Email = student.Email,
                DOB = student.DOB
            };

            //ok- 200 success
            return Ok(studentDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]  // success ok
        [ProducesResponseType(400)]  // bad req client error
        [ProducesResponseType(404)] // bad request client error not found
        [ProducesResponseType(500)] // server error
        public ActionResult<bool> DeleteStudents(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var student = _dbContext.Students.Where(n => n.Id == id).FirstOrDefault();
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
            _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
            //ok- 200 success
            return Ok(true);
        }
    }
}
