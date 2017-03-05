using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var newEntry = new Entry()
            {
                Date = DateTime.Today
            };

            ViewBag.Items = Data.Data.Activities;

            PopulateActivitiesList();

            return View(newEntry);
        }

  
        [HttpPost, ActionName("Add")]
        public ActionResult AddPost(Entry entry)
        {
            if (ModelState.IsValid == true)
            {
                _entriesRepository.AddEntry(entry);
                TempData["Message"] = "Your Entry Successfully Added";
                return RedirectToAction("Index");

            }

            PopulateActivitiesList();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get entry from the repository 
            Entry entry = _entriesRepository.GetEntry((int)id); 

            //return a status of not found if not found
            if (entry == null)
            {
                return HttpNotFound(); 
            }
            //if found pass the entry to the view
            PopulateActivitiesList();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //validate the entry 
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["message"] = "Your Entry Was Successfully Updated.";
                return RedirectToAction("Index");
             }
            // use the repository to update the entry  
            // return the user to the entries list page

            PopulateActivitiesList();
            return View();
        }

     
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get entry from the repository 
            Entry entry = _entriesRepository.GetEntry((int)id);
            
            //return a status of not found if not found
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            _entriesRepository.DeleteEntry(id);
            TempData["Message"] = "Entry Successfully Deleted. ";
            return RedirectToAction("Index");
        }

        private void PopulateActivitiesList()
        {
            ViewBag.ListActivities = new SelectList(Data.Data.Activities, "Id", "Name");
        }



    }
}