using Microsoft.AspNetCore.Mvc;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    public class KanbanController : Controller
    {
        private readonly AppDbContext _context;

        public KanbanController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tasks = _context.Tasks.OrderBy(t => t.CreatedAt).ToList();
            return View(tasks);
        }

        [HttpPost]
        public IActionResult Create([FromBody] WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                if (!Enum.IsDefined(typeof(WorkTaskStatus), workTask.Status))
                    return BadRequest(new { success = false, message = "Nieprawidłowy status zadania" });

                if (!Enum.IsDefined(typeof(WorkTaskPriority), workTask.Priority))
                    return BadRequest(new { success = false, message = "Nieprawidłowy priorytet zadania" });

                if (workTask.Assignee?.Length > 100)
                    return BadRequest(new { success = false, message = "Osoba może mieć maksymalnie 100 znaków" });

                if (workTask.TimeSpentMinutes < 0)
                    return BadRequest(new { success = false, message = "Czas pracy nie może być ujemny" });

                workTask.CreatedAt = DateTime.Now;
                workTask.Assignee = string.IsNullOrWhiteSpace(workTask.Assignee) ? null : workTask.Assignee.Trim();
                workTask.DueDate = workTask.DueDate?.Date;
                _context.Tasks.Add(workTask);
                _context.SaveChanges();

                return Json(new { success = true, id = workTask.Id, message = "Zadanie dodane" });
            }

            return Json(new { success = false, message = "Błąd podczas dodawania zadania" });
        }

        [HttpPost]
        public IActionResult UpdateStatus([FromBody] UpdateTaskStatusRequest request)
        {
            if (!Enum.IsDefined(typeof(WorkTaskStatus), request.Status))
                return BadRequest(new { success = false, message = "Nieprawidłowy status zadania" });

            var task = _context.Tasks.Find(request.Id);
            if (task == null)
                return Json(new { success = false, message = "Zadanie nie znalezione" });

            task.Status = request.Status;
            task.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = "Status zaktualizowany" });
        }

        [HttpPost]
        public IActionResult Update([FromBody] UpdateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > 200)
                return BadRequest(new { success = false, message = "Tytuł jest wymagany i może mieć maksymalnie 200 znaków" });

            if (request.Description?.Length > 1000)
                return BadRequest(new { success = false, message = "Opis może mieć maksymalnie 1000 znaków" });

            if (!Enum.IsDefined(typeof(WorkTaskPriority), request.Priority))
                return BadRequest(new { success = false, message = "Nieprawidłowy priorytet zadania" });

            if (request.Assignee?.Length > 100)
                return BadRequest(new { success = false, message = "Osoba może mieć maksymalnie 100 znaków" });

            if (request.TimeSpentMinutes < 0)
                return BadRequest(new { success = false, message = "Czas pracy nie może być ujemny" });

            var task = _context.Tasks.Find(request.Id);
            if (task == null)
                return NotFound(new { success = false, message = "Zadanie nie znalezione" });

            task.Title = request.Title.Trim();
            task.Description = request.Description?.Trim();
            task.Priority = request.Priority;
            task.Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim();
            task.TimeSpentMinutes = request.TimeSpentMinutes;
            task.DueDate = request.DueDate?.Date;
            task.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = "Zadanie zaktualizowane" });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] DeleteTaskRequest request)
        {
            var task = _context.Tasks.Find(request.Id);
            if (task == null)
                return Json(new { success = false, message = "Zadanie nie znalezione" });

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return Json(new { success = true, message = "Zadanie usunięte" });
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _context.Tasks.OrderBy(t => t.CreatedAt).ToList();
            return Json(tasks);
        }

        public sealed class UpdateTaskStatusRequest
        {
            public int Id { get; init; }
            public WorkTaskStatus Status { get; init; }
        }

        public sealed class DeleteTaskRequest
        {
            public int Id { get; init; }
        }

        public sealed class UpdateTaskRequest
        {
            public int Id { get; init; }
            public string Title { get; init; } = string.Empty;
            public string? Description { get; init; }
            public WorkTaskPriority Priority { get; init; } = WorkTaskPriority.Medium;
            public string? Assignee { get; init; }
            public int TimeSpentMinutes { get; init; }
            public DateTime? DueDate { get; init; }
        }
    }
}
