﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
  
    public class BuggyController : APIBaseController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("NotFound")]
        //baseUrl/api/buggy/notfound
        public ActionResult GetNotFound()
        {
            var Product = _dbContext.Products.Find(100);
            if ((Product is null)) return NotFound(new ApiResponse(404));
            return Ok(Product);
                
          
            
        }
        [HttpGet("ServerError")]
        //baseUrl/api/buggy/servererror
        public ActionResult GetServerError()
        {
            var Product = _dbContext.Products.Find(100);
           
            var ProductToReturn = Product.ToString();
            //Error
            //will Throw a NullReferenceException
            return Ok(ProductToReturn);
        }
        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }
        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }
    }
}
