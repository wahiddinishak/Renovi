using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly AppDbContext db;

        public MenuViewComponent(AppDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string roleID = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            ViewData["role"] = roleID;
            var menuItems = await GetAllMenuItems(roleID);
            return View("Default", GetMenu(menuItems, 0));
        }

        private IViewComponentResult RedirectToAction(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<userAccess>> GetAllMenuItems(string role)
        {
            var query = from a in db.Menus
                        join b in db.AccessRights.Where(x => x.role == role) on a.Id equals b.menu
                        select new userAccess
                        {
                            id = a.Id,
                            menu = a.name,
                            action = a.action,
                            controller = a.controller,
                            parent = a.parent,
                            property = a.property
                        };
            // var menu = await query.ToListAsync().ConfigureAwait(false);
            return await query.ToListAsync().ConfigureAwait(false);
        }


        public IList<ViewAccess> GetMenu(IList<userAccess> menuList, int? parentID)
        {
            var children = GetChildrenMenu(menuList, parentID);

            if (!children.Any())
            {
                return new List<ViewAccess>();
            }

            var vmList = new List<ViewAccess>();

            foreach (var item in children)
            {
                var menu = GetMenuItem(menuList, item.id);

                var vm = new ViewAccess();

                vm.id = menu.id;
                vm.menu = menu.menu;
                vm.action = menu.action;
                vm.controller = menu.controller;
                vm.property = menu.property;
                vm.Children = GetMenu(menuList, menu.id);

                vmList.Add(vm);
            }
            return vmList;
        }

        private IList<userAccess> GetChildrenMenu(IList<userAccess> menuList, int? parentId = null)
        {
            return menuList.Where(x => x.parent == parentId).OrderBy(x => x.id).ToList();
        }

        private userAccess GetMenuItem(IList<userAccess> menuList, int id)
        {
            return menuList.FirstOrDefault(x => x.id == id);
        }
    }
}
