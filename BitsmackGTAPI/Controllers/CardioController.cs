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
    public class CardioController : ApiController
    {
        private GTDbContext db = new GTDbContext();

        // GET api/Cardio
        public IEnumerable<Cardio> GetCardios()
        {
            return db.Cardios.AsEnumerable();
        }

        // GET api/Cardio/5
        public Cardio GetCardio(int id)
        {
            Cardio cardio = db.Cardios.Find(id);
            if (cardio == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return cardio;
        }

        // PUT api/Cardio/5
        public HttpResponseMessage PutCardio(int id, Cardio cardio)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != cardio.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(cardio).State = EntityState.Modified;

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

        // POST api/Cardio
        public HttpResponseMessage PostCardio(Cardio cardio)
        {
            if (ModelState.IsValid)
            {
                db.Cardios.Add(cardio);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, cardio);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = cardio.id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Cardio/5
        public HttpResponseMessage DeleteCardio(int id)
        {
            Cardio cardio = db.Cardios.Find(id);
            if (cardio == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Cardios.Remove(cardio);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, cardio);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}