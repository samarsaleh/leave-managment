using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_managment.Contracts;
using leave_managment.Data;
using leave_managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace leave_managment.Controllers
{
    [Authorize (Roles="Administrator")]//if smbody want to use this action need to be loged in

    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repo; //reference interface which knows the functions ..
        private readonly IMapper _mapper;


        public LeaveTypesController(ILeaveTypeRepository repo ,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
        // GET: LeaveTypes
        public ActionResult Index()
        {
            var leavetypes = _repo.FindAll().ToList(); // returning collection of leavetype OBJECTS
            //so its need to map to get data from data class using the view model 
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes);//note returning types 
            //or using ICollection instead of list 
            return View(model);
        }

        // GET: LeaveTypes/Details/5
        public ActionResult Details(int id)
        {
            if (!_repo.isExist(id))
            {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeVM model)// according to our data class 
        {
            try
            {
                // TODO: Add insert logic here
                // if the data is valid 
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model); //mapping the data to leave type 
                leaveType.DateCreated = DateTime.Now;
                var isSuccess= _repo.create(leaveType);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong samoora..");
                    return View(model); //returning the data 
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong samoora..");
                return View(model);
            }
        }

        // GET: LeaveTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if(!_repo.isExist(id))
            {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model); //mapping the data to leave type 
                var isSuccess = _repo.update(leaveType);


                // TODO: Add update logic here
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong samoora..");
                    return View(model); //returning the data 
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong samoora..");
                return View(model);
            }
        }

        // GET: LeaveTypes/Delete/5
        //basicly if for confimation
        public ActionResult Delete(int id)
        {
         /*   if (!_repo.isExist(id))
            {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);*/

            var leaveType = _repo.FindById(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            var isSuccess = _repo.Delete(leaveType);// shofe the return lal function 
            if (!isSuccess)
            {

                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
           



        //if yes you are sure 
        // POST: LeaveTypes/Delete/5
      /*  [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,LeaveTypeVM model)
        {//is only passing id in the chtml but i passed model cus of the similiraty of the functions
            try
            {
                // TODO: Add delete logic here
                var leaveType = _repo.FindById(id);
                if (leaveType == null)
                {
                    return NotFound();
                }
                var isSuccess = _repo.Delete(leaveType);// shofe the return lal function 
                if (!isSuccess)
                {
        
                    return View(model); //returning the data 
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }*/
    }
}