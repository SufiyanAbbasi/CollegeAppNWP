﻿using CollegeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("All")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]        
        public ActionResult<IEnumerable<StudentDTO>> GetStudents()
        {
            _logger.LogInformation("Student Methods Started");
            var students = CollegeRepositiory.Students.Select(s => new StudentDTO()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Address = s.Address,
                Email = s.Email,
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
            var student = CollegeRepositiory.Students.Where(n  => n.Id == id).FirstOrDefault();
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

            int newId = CollegeRepositiory.Students.LastOrDefault().Id + 1;
            Student student = new Student
            {
                Id = newId,
                StudentName = model.StudentName,
                Address = model.Address,
                Email = model.Email,
            };
            CollegeRepositiory.Students.Add(student);
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

            var existingStudent = CollegeRepositiory.Students.Where(s=> s.Id  == model.Id).FirstOrDefault();
            if (existingStudent == null)
                return NotFound();

            existingStudent.StudentName = model.StudentName;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;

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

            var existingStudent = CollegeRepositiory.Students.Where(s => s.Id == id).FirstOrDefault();
            if (existingStudent == null)
                return NotFound();

            var studentDTO = new StudentDTO
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Email = existingStudent.Email,
                Address = existingStudent.Address,
            };

            patchDocument.ApplyTo(studentDTO, ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent.StudentName = studentDTO.StudentName;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Address;

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
            var student = CollegeRepositiory.Students.Where(n => n.StudentName == name).FirstOrDefault();
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
            var student = CollegeRepositiory.Students.Where(n => n.Id == id).FirstOrDefault();
            if (student == null)
            {
                // not found - 404 - client error
                return NotFound($"The Student with id {id} not found");
            }
            CollegeRepositiory.Students.Remove(student);
            //ok- 200 success
            return Ok(true);
        }
    }
}