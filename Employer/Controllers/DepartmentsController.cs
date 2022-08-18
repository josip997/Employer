using Employer.Data;
using Employer.Models;
using Employer.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employer.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly EmployeesDbContext employeesDbContext;

        public DepartmentsController(EmployeesDbContext employeesDbContext)
        {
            this.employeesDbContext = employeesDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await employeesDbContext.Departments.ToListAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddDepartmentViewModel departmentRequest)
        {
            var department = new Department()
            {
                Id = Guid.NewGuid(),
                Cipher = departmentRequest.Cipher,
                Name = departmentRequest.Name,
            };

            if (!ModelState.IsValid)
            {
                return View(departmentRequest);
            }

            await employeesDbContext.Departments.AddAsync(department);
            await employeesDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var department = await employeesDbContext.Departments.FirstOrDefaultAsync(x => x.Id == id);

            if (department != null)
            {
                var viewModel = new UpdateDepartmentViewModel()
                {
                    Id = department.Id,
                    Cipher = department.Cipher,
                    Name = department.Name,
                };

                return await Task.Run(() => View(nameof(View), viewModel));
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateDepartmentViewModel viewModel)
        {
            var department = await employeesDbContext.Departments.FindAsync(viewModel.Id);

            if (department != null)
            {
                department.Cipher = viewModel.Cipher;
                department.Name = viewModel.Name;

                if (ModelState.IsValid)
                {
                    await employeesDbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                return await Task.Run(() => View(nameof(View), viewModel));

            }

            return RedirectToAction(nameof(Index)); //TODO: Not Found
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateDepartmentViewModel viewModel)
        {
            var department = await employeesDbContext.Departments.FindAsync(viewModel.Id);

            if (department != null)
            {
                employeesDbContext.Departments.Remove(department);
                await employeesDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index)); //TODO: Not Found
        }
    }
}
