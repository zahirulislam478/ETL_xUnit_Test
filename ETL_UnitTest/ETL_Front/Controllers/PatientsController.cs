using ETL_Front.Services;
using ETL_Shared.InputModels;
using ETL_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace ETL_Front.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IHttpClientWrapper httpClientWrapper;

        public PatientsController(IHttpClientWrapper httpClientWrapper)
        {
            this.httpClientWrapper = httpClientWrapper;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5; // Set your desired page size
            int pageNumber = page ?? 1;

            var patientsJson = await httpClientWrapper.GetStringAsync("api/Patients/All/Include");
            var patients = JsonConvert.DeserializeObject<List<Patient>>(patientsJson);

            // Create a paged list of patients
            var pagedList = patients.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        public async Task<IActionResult> Create(string msg = "")
        {
            ViewBag.Msg = msg;
            ViewBag.Allergies = await GetDeserializedList<Allergy>("api/Allergies");
            ViewBag.DiseaseInformations = await GetDeserializedList<DiseaseInformation>("api/DiseaseInformations");
            ViewBag.NCDs = await GetDeserializedList<NCD>("api/NCDs");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientInputModel model)
        {
            if (ModelState.IsValid)
            {
                await httpClientWrapper.PostJsonAsync("api/Patients", model);
                return RedirectToAction("Create", routeValues: new { msg = "done" });
            }

            ViewBag.Allergies = await GetDeserializedList<Allergy>("api/Allergies");
            ViewBag.DiseaseInformations = await GetDeserializedList<DiseaseInformation>("api/DiseaseInformations");
            ViewBag.NCDs = await GetDeserializedList<NCD>("api/NCDs");
            ViewBag.Msg = "";

            return View(model);
        }


        private async Task<List<T>> GetDeserializedList<T>(string apiUrl)
        {
            var json = await httpClientWrapper.GetStringAsync(apiUrl);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }
    }
}
