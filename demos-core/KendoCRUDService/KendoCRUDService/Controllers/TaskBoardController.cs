using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.SessionExtensions;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class TaskBoardController : Controller
    {
        private readonly TaskBoardRepository _taskBoard;

        public TaskBoardController(TaskBoardRepository tasksBoardRepository)
        {
            _taskBoard = tasksBoardRepository;
        }

        public ActionResult Index()
        {
            return Json(_taskBoard.All());
        }

        public JsonResult Create(CardModel model)
        {
            if (model != null)
            {
                _taskBoard.Create(model);
            }
            return Json(model);
        }

        public JsonResult Update(CardModel model)
        {
            if (model != null)
            {
                _taskBoard.Update(model);
            }
            return Json(model);
        }

        public JsonResult Destroy(CardModel model)
        {
            if (model != null)
            {
                _taskBoard.Destroy(model);
            }

            return Json(model);
        }

        public ActionResult Columns()
        {
            return Json(_taskBoard.ColumnsList());
        }

        public JsonResult Columns_Create(ColumnModel model)
        {
            if (model != null)
            {
                _taskBoard.Columns_Create(model);
            }

            return Json(model);
        }

        public JsonResult Columns_Update(ColumnModel model)
        {
            if (model != null)
            {
                _taskBoard.Columns_Update(model);
            }

            return Json(model);
        }

        public JsonResult Columns_Destroy(ColumnModel model)
        {
            if (model != null)
            {
                _taskBoard.Columns_Destroy(model);
            }

            return Json(model);
        }
    }
}
