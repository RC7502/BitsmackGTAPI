using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Controllers
{
    public class APIKeysController : ApiController
    {
        private GTDbContext db = new GTDbContext();

        // GET api/APIKeys
        [AuthorizeFilter]
        public IEnumerable<APIKeys> GetAPIKeys()
        {
            return db.APIKeys.AsEnumerable();
        }

        // GET api/APIKeys/5
        [AuthorizeFilter]
        public APIKeys GetAPIKeys(int id)
        {
            APIKeys apikeys = db.APIKeys.Find(id);
            if (apikeys == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return apikeys;
        }

        // PUT api/APIKeys/5
        [AuthorizeFilter]
        public HttpResponseMessage PutAPIKeys(int id, APIKeys apikeys)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != apikeys.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(apikeys).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/APIKeys
        [AuthorizeFilter]
        public HttpResponseMessage PostAPIKeys(APIKeys apikeys)
        {
            if (ModelState.IsValid)
            {
                db.APIKeys.Add(apikeys);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, apikeys);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = apikeys.id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/APIKeys/5
        [AuthorizeFilter]
        public HttpResponseMessage DeleteAPIKeys(int id)
        {
            APIKeys apikeys = db.APIKeys.Find(id);
            if (apikeys == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.APIKeys.Remove(apikeys);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, apikeys);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}