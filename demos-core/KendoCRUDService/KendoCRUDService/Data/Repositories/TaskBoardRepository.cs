﻿using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class TaskBoardRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        ConcurrentDictionary<string, IList<CardModel>> _carModels;
        ConcurrentDictionary<string, IList<ColumnModel>> _columnsList;
        public IList<CardModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _carModels.GetOrAdd(userKey, key =>
            {
                return new List<CardModel>
                    {
                            new CardModel { ID = 1, Title = "Campaigns", Order = 1, Description = "Create a new landing page for campaign", Status = "todo", Category = "urgent" },
                            new CardModel { ID = 2, Title = "Newsletters", Order = 2, Description = "Send newsletter", Status = "todo", Category = "highpriority" },
                            new CardModel { ID = 3, Title = "Ads Analytics", Order = 3, Description = "Review ads performance", Status = "todo", Category = "lowpriority" },
                            new CardModel { ID = 4, Title = "SEO Analytics", Order = 4, Description = "Review SEO results", Status = "todo", Category = "lowpriority" },
                            new CardModel { ID = 5, Title = "Customer Research", Order = 5, Description = "Interview focus groups", Status = "todo", Category = "urgent" },
                            new CardModel { ID = 6, Title = "Testimonials & Case Studies", Order = 6, Description = "Publish new case study", Status = "todo", Category = "urgent" },
                            new CardModel { ID = 7, Title = "Content", Order = 7, Description = "Plan content for podcasts", Status = "todo", Category = "highpriority" },
                            new CardModel { ID = 8, Title = "Customer Journey", Order = 8, Description = "Update virtual classrooms' experience", Status = "todo", Category = "urgent" },
                            new CardModel { ID = 9, Title = "Funnel Analytics", Order = 9, Description = "Funnel analysis", Status = "inProgress", Category = "highpriority" },
                            new CardModel { ID = 10, Title = "Customer Research", Order = 10, Description = "Refine feedback from user interviews", Status = "inProgress", Category = "highpriority" },
                            new CardModel { ID = 11, Title = "Campaigns", Order = 11, Description = "Collaborate with designers on new banners", Status = "inProgress", Category = "urgent" },
                            new CardModel { ID = 12, Title = "Campaigns", Order = 12, Description = "Schedule social media for release", Status = "inProgress", Category = "highpriority" },
                            new CardModel { ID = 13, Title = "Customer Journey", Order = 13, Description = "Review shopping cart experience", Status = "done", Category = "lowpriority" },
                            new CardModel { ID = 14, Title = "Content", Order = 14, Description = "Publish new blogpost", Status = "done", Category = "urgent" },
                            new CardModel { ID = 15, Title = "Post-Release Party", Order = 15, Description = "Plan new release celebration", Status = "done", Category = "lowpriority" }
                    };
            });
        }

        public IList<ColumnModel> ColumnsList()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _columnsList.GetOrAdd(userKey, key =>
            {
                return new List<ColumnModel>
                {
                        new ColumnModel { ID = 1, Text = "Pending", Order = 1, Status = "todo" },
                        new ColumnModel { ID = 2, Text = "Under Review", Order = 2, Status = "inProgress" },
                        new ColumnModel { ID = 2, Text = "Scheduled", Order = 3, Status = "done" }
                };

            });
        }

        public TaskBoardRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _columnsList = new ConcurrentDictionary<string, IList<ColumnModel>>();
            _carModels = new ConcurrentDictionary<string, IList<CardModel>>();
        }

        public void Create(CardModel model)
        {
            int lastID = All().Select(m => m.ID).Max();
            model.ID = lastID + 1;
            All().Add(model);
        }

        public void Update(CardModel model)
        {
            var target = One(m => m.ID == model.ID);

            target.Title = model.Title;
            target.Description = model.Description;
            target.Category = model.Category;
            target.Order = model.Order;
            target.Status = model.Status;
        }

        public void Destroy(CardModel model)
        {
            var target = One(m => m.ID == model.ID);

            All().Remove(target);
        }

        public void Columns_Create(ColumnModel model)
        {
            int lastID = ColumnsList().Select(m => m.ID).Max();
            int order = ColumnsList().Select(m => m.Order).Max();
            model.ID = lastID + 1;
            model.Order = order + 1;
            model.Status = model.Text.ToLowerInvariant();
            ColumnsList().Add(model);
        }

        public void Columns_Update(ColumnModel model)
        {
            var target = ColumnOne(m => m.ID == model.ID);

            target.Text = model.Text;
            target.Order = model.Order;
            target.Status = model.Status;
        }

        public void Columns_Destroy(ColumnModel model)
        {
            var target = ColumnOne(m => m.ID == model.ID);

            ColumnsList().Remove(target);
        }

        public CardModel One(Func<CardModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public ColumnModel ColumnOne(Func<ColumnModel, bool> predicate)
        {
            return ColumnsList().FirstOrDefault(predicate);
        }
    }
}
