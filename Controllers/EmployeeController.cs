using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpManagementMVC.Models;
using PagedList;
using PagedList.Mvc;
using ImageResizer;
using ImageResizer.ExtensionMethods;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace EmpManagementMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private Neosoft_Mayur_ChaudhariEntities db = new Neosoft_Mayur_ChaudhariEntities();

        // GET: Employee
        public ActionResult Index(int? page, string searching)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            List<EmployeeMaster> employeeData;

            if (!string.IsNullOrEmpty(searching))
            {
                // If a search term is provided, call the stored procedure with the parameter
                var sqlQuery = "[stp_Emp_GetEmployeeData_MVC] @searching";
                SqlParameter parameter = new SqlParameter("searching", "%" + searching + "%");

                employeeData = db.Database.SqlQuery<EmployeeMaster>(sqlQuery, parameter).ToList();
            }
            else
            {
                // If no search term is provided, call the stored procedure without the parameter
                var sqlQuery = "[stp_Emp_GetEmployeeData_MVC] @searching = null";

                employeeData = db.Database.SqlQuery<EmployeeMaster>(sqlQuery).ToList();
            }

            var pagedData = employeeData.ToPagedList(pageNumber, pageSize);
            return View(pagedData);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_EmployeeMaster tbl_EmployeeMaster = db.tbl_EmployeeMaster.Find(id);
            if (tbl_EmployeeMaster == null)
            {
                return HttpNotFound();
            }
            return View(tbl_EmployeeMaster);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_Id,EmployeeCode,FirstName,LastName,CountryId,StateId,CityId,EmailAddress,MobileNumber,PanNumber,PassportNumber,ProfileImage,Gender,IsActive,DateOfBirth,DateOfJoinee,CreatedDate,UpdatedDate,IsDeleted,DeletedDate")] tbl_EmployeeMaster tbl_EmployeeMaster, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                //db.tbl_EmployeeMaster.Add(tbl_EmployeeMaster);
                //db.SaveChanges();
                //return RedirectToAction("Index");
                try
                {
                    // Check if an image was uploaded
                    if (ProfileImage != null && ProfileImage.ContentLength > 0)
                    {
                        // Get the file name
                        string fileName = Path.GetFileName(ProfileImage.FileName);

                        // Define the path to save the image in the "uploads" folder in your project root
                        string path = Path.Combine(Server.MapPath("~/uploads"), fileName);

                        // Save the file to the server
                        ProfileImage.SaveAs(path);

                        // Compress the image to be under 200KB
                        ImageResizer.ImageJob imageJob = new ImageJob(path, path, new Instructions("maxwidth=800&maxheight=800&format=jpg&quality=70"));
                        ImageBuilder.Current.Build(imageJob);

                        // Set the ProfileImage property to the saved file path
                        tbl_EmployeeMaster.ProfileImage = "~/uploads/" + fileName; // Assuming "uploads" is in the root of your project
                    }

                    db.tbl_EmployeeMaster.Add(tbl_EmployeeMaster);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during image processing or saving
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the profile image: " + ex.Message);
                }
            }

            return View(tbl_EmployeeMaster);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_EmployeeMaster tbl_EmployeeMaster = db.tbl_EmployeeMaster.Find(id);
            if (tbl_EmployeeMaster == null)
            {
                return HttpNotFound();
            }
            return View(tbl_EmployeeMaster);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_Id,EmployeeCode,FirstName,LastName,CountryId,StateId,CityId,EmailAddress,MobileNumber,PanNumber,PassportNumber,ProfileImage,Gender,IsActive,DateOfBirth,DateOfJoinee,CreatedDate,UpdatedDate,IsDeleted,DeletedDate")] tbl_EmployeeMaster tbl_EmployeeMaster, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                // Check if an image was uploaded
                if (ProfileImage != null && ProfileImage.ContentLength > 0)
                {
                    // Get the file name
                    string fileName = Path.GetFileName(ProfileImage.FileName);

                    // Define the path to save the image in the "uploads" folder in your project root
                    string path = Path.Combine(Server.MapPath("~/uploads"), fileName);

                    // Save the file to the server
                    ProfileImage.SaveAs(path);

                    // Compress the image to be under 200KB
                    ImageResizer.ImageJob imageJob = new ImageJob(path, path, new Instructions("maxwidth=800&maxheight=800&format=jpg&quality=70"));
                    ImageBuilder.Current.Build(imageJob);

                    // Set the ProfileImage property to the saved file path
                    tbl_EmployeeMaster.ProfileImage = "~/uploads/" + fileName; // Assuming "uploads" is in the root of your project

                    // Add the modified model to the context
                    db.Entry(tbl_EmployeeMaster).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where no image was uploaded
                }
            }
            return View(tbl_EmployeeMaster);
        }

        public ActionResult GetStates(int countryId)
        {
            // Replace "db.States" and "db.Cities" with the appropriate DbSet names in your context.
            var states = db.tbl_State.Where(s => s.CountryId == countryId)
                                 .Select(s => new { s.StateId, s.StateName })
                                 .ToList();

            return Json(new SelectList(states, "StateId", "StateName"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCities(int stateId)
        {
            // Replace "db.Cities" with the appropriate DbSet name in your context.
            var cities = db.tbl_City.Where(c => c.StateId == stateId)
                                 .Select(c => new { c.CityId, c.CityName })
                                 .ToList();

            return Json(new SelectList(cities, "CityId", "CityName"), JsonRequestBehavior.AllowGet);
        }


        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_EmployeeMaster tbl_EmployeeMaster = db.tbl_EmployeeMaster.Find(id);
            if (tbl_EmployeeMaster == null)
            {
                return HttpNotFound();
            }
            return View(tbl_EmployeeMaster);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_EmployeeMaster tbl_EmployeeMaster = db.tbl_EmployeeMaster.Find(id);
            db.tbl_EmployeeMaster.Remove(tbl_EmployeeMaster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
