using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
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
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        public StudentController(ILogger<StudentController> logger, IMapper mapper, IStudentRepository studentRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        [Route("All")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]        
        public async Task <ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            _logger.LogInformation("Student Methods Started");
            var students = await _studentRepository.GetAllAsync();

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
            var student = await _studentRepository.GetByIdAsync(id);
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
           var id =   await _studentRepository.CreateAsync(student);

            dto.Id = id;
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


            var existingStudent =await _studentRepository.GetByIdAsync(dto.Id,true);
            if (existingStudent == null)
                return NotFound();
            var newRecord= _mapper.Map<Student>(dto);
            
           await _studentRepository.UpdateAsync(newRecord);

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

            var existingStudent = await _studentRepository.GetByIdAsync(id, true);
            if (existingStudent == null)
                return NotFound();

            var studentDTO = _mapper.Map<StudentDTO>(existingStudent);

            patchDocument.ApplyTo(studentDTO, ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent = _mapper.Map<Student>(studentDTO);
            await _studentRepository.UpdateAsync(existingStudent);
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
            var student =await _studentRepository.GetByNameAsync(name);
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
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
            await _studentRepository.DeleteAsync(student);
            //ok- 200 success
            return Ok(true);
        }
    }
}
