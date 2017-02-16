using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Square9.SecurityDisplay.Controllers
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [ActionName("start")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Self Host Started" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}