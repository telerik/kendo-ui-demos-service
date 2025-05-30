﻿using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace kendo_northwind_pg.Controllers
{
    public class CategoriesController : ODataController
    {
        private DemoDbContext _dbContext;
        private ProductRepository _productRepository;

        public CategoriesController(DemoDbContext dbContext, ProductRepository productRepository)
        {
            _dbContext = dbContext;
            _productRepository = productRepository;
        }

        // GET: odata/Categories
        [HttpGet]
        [EnableQuery]
        [Route("Categories")]
        public IQueryable<Category> GetCategories()
        {
            return _dbContext.Categories;
        }

        // GET: odata/Categories(5)
        [HttpGet]
        [EnableQuery]
        [Route("Categories({key})")]
        public IQueryable<Category> GetCategory([FromODataUri] int key)
        {
            return _dbContext.Categories.Where(category => category.CategoryID == key);
        }

        // PUT: odata/Categories(5)
        [HttpPut]
        [Route("Categories({key})")]
        public IActionResult Put([FromODataUri] int key, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != category.CategoryID)
            {
                return BadRequest();
            }

            _dbContext.Entry(category).State = EntityState.Modified;

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(category);
        }

        // POST: odata/Categories
        [HttpPost]
        [Route("Categories")]
        public IActionResult Post(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return Created(category);

        }

        // PATCH: odata/Categories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Categories")]
        public IActionResult Patch([FromODataUri] int key, Delta<Category> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category category = _dbContext.Categories.Find(key);
            if (category == null)
            {
                return NotFound();
            }

            patch.Patch(category);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(category);
        }

        // DELETE: odata/Categories(5)
        [HttpDelete]
        [Route("Categories({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Category category = _dbContext.Categories.Find(key);
            if (category == null)
            {
                return NotFound();
            }

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Categories(5)/Products
        [HttpGet]
        [EnableQuery]
        [Route("Categories({key})/Products")]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return _productRepository.All().Where(m => m.CategoryID == key).AsQueryable();
        }

        private bool CategoryExists(int key)
        {
            return _dbContext.Categories.Count(e => e.CategoryID == key) > 0;
        }
    }
}
