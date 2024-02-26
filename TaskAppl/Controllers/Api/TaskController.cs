using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppl.DataAccess.Interfaces;
using TaskAppl.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskAppl.Controllers.Api
{
    /// <summary>
    /// API-контроллер для управления задачами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(500)]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service"></param>
        public TaskController(ITaskService service)
        {
            _service = service;
        }

        /// <summary>
        /// Возвращает все записи
        /// </summary>
        /// <returns></returns>
        // GET: api/<TaskController>
        [ProducesResponseType(typeof(List<TaskModel>), 200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> Get()
        {
            List<TaskModel> tasks = (await _service.GetAll()).ToList();
            return Ok(tasks);
        }

        /// <summary>
        /// Возвращает по ID одну запись
        /// </summary>
        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> Get(int id)
        {
            var task = await _service.GetById(id);
            if (task == null) return NotFound();
            return task;
        }

        /// <summary>
        /// Создание записи
        /// </summary>
        /// <param name="model"></param>
        // POST api/<TaskController>
        [ProducesResponseType(typeof(TaskModel), 200)]
        [HttpPost]
        public async Task<ActionResult<TaskModel>> Post([FromBody] TaskModel model)
        {
            await _service.Create(model);
            return CreatedAtAction("GetTaskModel", new { id = model.Id }, model);
        }

        /// <summary>
        /// Обновление записи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT api/<TaskController>/5
        [ProducesResponseType(typeof(TaskModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaskModel model)
        {
            if (id != model.Id) return BadRequest();

            try
            {
                await _service.Update(id, model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Удаленеи записи по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<TaskController>/5
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TaskExists(id)) return NotFound();
            await _service.Delete(id);

            // TODO: Удаление файлов при удалении данной задачи

            return NoContent();
        }


        private bool TaskExists(int id) => _service.IsExists(id).Result;
    }
}
