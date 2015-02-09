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
    public class PedometerController : ApiController
    {
        private GTDbContext db = new GTDbContext();

        // GET api/Pedometer
        [AuthorizeFilter]
        public IEnumerable<Pedometer> GetPedometers()
        {
            try
            {
                return db.Pedometers.AsEnumerable();
            }
            catch(Exception ex)
            {
                return new List<Pedometer>();

            }

        }

        // GET api/Pedometer/5
        [AuthorizeFilter]
        public Pedometer GetPedometer(int id)
        {
            Pedometer pedometer = db.Pedometers.Find(id);
            if (pedometer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return pedometer;
        }

        // PUT api/Pedometer/5
        [AuthorizeFilter]
        public HttpResponseMessage PutPedometer(int id, Pedometer pedometer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != pedometer.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(pedometer).State = EntityState.Modified;

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

        // POST api/Pedometer
        [AuthorizeFilter]
        public HttpResponseMessage PostPedometer(Pedometer pedometer)
        {
            if (ModelState.IsValid)
            {
                db.Pedometers.Add(pedometer);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pedometer);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pedometer.id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Pedometer/5
        [AuthorizeFilter]
        public HttpResponseMessage DeletePedometer(int id)
        {
            Pedometer pedometer = db.Pedometers.Find(id);
            if (pedometer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Pedometers.Remove(pedometer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, pedometer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}