using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class TasksController : Controller
    {
        private readonly TasksRepository _tasksRepository;

        public TasksController(TasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }

        public ActionResult Index()
        {
            return Json(_tasksRepository.All());
        }

        public JsonResult Update(IEnumerable<TaskViewModel> models)
        {
            if (models != null)
            {
                foreach (var task in models)
                {
                    _tasksRepository.Update(task);
                }
            }

            return Json(models);
        }

        public JsonResult Destroy(IEnumerable<TaskViewModel> models)
        {
            if (models != null)
            {
                foreach (var task in models)
                {
                    _tasksRepository.Delete(task);
                }
            }

            return Json(models);
        }

        public JsonResult Create(IEnumerable<TaskViewModel> models)
        {
            if (models != null)
            {
                foreach (var task in models)
                {
                    _tasksRepository.Insert(task);
                }
            }

            return Json(models);
        }
    }
}
