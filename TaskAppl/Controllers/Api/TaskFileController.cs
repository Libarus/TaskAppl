using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppl.DataAccess.Interfaces;
using TaskAppl.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskAppl.Controllers.Api
{
    /// <summary>
    /// API-контроллер для управления файлами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(500)]
    public class TaskFileController : ControllerBase
    {
        private readonly ITaskFileService _service;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="service"></param>
        public TaskFileController(ITaskFileService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        /// <summary>
        /// Возвращает все записи
        /// </summary>
        /// <returns></returns>
        // GET: api/<TaskController>
        [ProducesResponseType(typeof(List<TaskFileModel>), 200)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskFileModel>>> Get()
        {
            List<TaskFileModel> taskFiles = (await _service.GetAll()).ToList();
            return Ok(taskFiles);
        }

        /// <summary>
        /// Возвращает все записи по ID задачи
        /// </summary>
        /// <returns></returns>
        // GET: api/<TaskController>
        [ProducesResponseType(typeof(List<TaskFileModel>), 200)]
        [HttpGet("bytask/{id}")]
        public async Task<ActionResult<IEnumerable<TaskFileModel>>> GetByTask(int id)
        {
            List<TaskFileModel> taskFiles = await _service.GetContext().Where(x => x.TaskId == id).ToListAsync();
            return Ok(taskFiles);
        }

        /// <summary>
        /// Возвращает по ID одну запись
        /// </summary>
        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskFileModel>> GetById(int id)
        {
            var taskFile = await _service.GetById(id);
            if (taskFile == null) return NotFound();
            return taskFile;
        }

        /// <summary>
        /// Добавляет файл в задачу по ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(TaskFileModel), 200)]
        [HttpPost("{id}")]
        public async Task<ActionResult<TaskFileModel>> UploadFile(int id, IFormFile uploadedFile)
        {
            // TODO: Добавление проверки на существование задачи, чтобы не было возможности привязать к несуществующей задаче

            if (uploadedFile != null)
            {
                string dir = _getPath();
                DateTime currentTime = DateTime.UtcNow;
                long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                string fname = string.Format("{0}_{1}", unixTime, uploadedFile.FileName);

                string path = Path.Combine(dir, fname);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                // TODO: при сохранении определять mime-тип файла и сохранять его

                TaskFileModel tfm = new TaskFileModel { TaskId = id, FileName = fname };
                await _service.Create(tfm);
                return Ok(tfm);

            }
            else return NotFound("File data not found");
        }

        /// <summary>
        /// Позволяет скачать файл по ID записи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IActionResult), 200)]
        [HttpPost("downloadfile/{id}")]
        public IActionResult DownloadFile(int id)
        {
            byte[] mas = new byte[1];
            string file_type = "application/pdf"; // TODO: при сохранении определять mime-тип файла

            TaskFileModel tfm = _service.GetById(id).Result;
            if (tfm != null)
            {
                if (!string.IsNullOrEmpty(tfm.FileName))
                {
                    string fname = tfm.FileName;
                    string dir = _getPath();
                    string fullName = Path.Combine(dir, fname);

                    mas = System.IO.File.ReadAllBytesAsync(fullName).Result;

                    return File(mas, file_type, fname);
                }
                else return NotFound();
            }
            else return NotFound();

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
            if (!TaskFileExists(id)) return NotFound();

            TaskFileModel tfm = _service.GetById(id).Result;
            if (tfm != null)
            {
                if (!string.IsNullOrEmpty(tfm.FileName))
                {
                    string fname = tfm.FileName;
                    string dir = _getPath();
                    string fullName = Path.Combine(dir, fname);

                    if (System.IO.File.Exists(fullName))
                    {
                        System.IO.File.Delete(fullName);
                    }
                    else return NotFound("Файл не найден");
                }
                else return NotFound("Имя файла в БД пустое");
            }
            else return NotFound(string.Format("Запись по ID: {0} не найдена", id));

            await _service.Delete(id);

            return NoContent();
        }


        private bool TaskFileExists(int id) => _service.IsExists(id).Result;

        private string _getPath()
        {
            string? mediaDir = _configuration.GetSection("AppSettings").GetValue<string>("MediaDir");

            // TODO: проверка на существование директории
            if (!System.IO.File.Exists(mediaDir)) throw new Exception("Директория не найдена");

            return mediaDir;
        }
    }
}
