using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Laboration3.Models;

namespace Laboration3.Controllers
{
    public class HeroesController : Controller
    {
        private HeroEntities db = new HeroEntities();

        // GET: Heroes
        /*public ActionResult Index()
        {
            var heroes = db.Heroes.Include(h => h.HERO_CLASSES);
            return View(heroes.ToList());
        }*/
        
        // GET: Heroes
        public ActionResult Index(string sortorder, string currentFilter, string searchString)
        {
            ViewBag.SortHeroName = String.IsNullOrEmpty(sortorder) ? "Hero_Name_Desc" : "";
            ViewBag.SortHeroClass = sortorder == "Hero_Class" ? "Hero_Class_Desc" : "Hero_Class";

            var dbHeroes = db.Heroes.AsEnumerable().OrderBy(h => h.Hero_Class);
            var heroes = from h in db.Heroes select h;

            if(searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                heroes = heroes.Where(h => h.HERO_CLASSES.HCLASS_TYPE.ToUpper().Contains(searchString.ToUpper()) || 
                h.Hero_Name.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortorder)
            {
                case "Hero_Name_Desc":
                    heroes = heroes.OrderByDescending(h => h.Hero_Name);
                    break;
                case "Hero_Class":
                    heroes = heroes.OrderBy(h => h.Hero_Class);
                    break;
                case "Hero_Class_Desc":
                    heroes = heroes.OrderByDescending(h => h.Hero_Class);
                    break;
                default:
                    heroes = heroes.OrderBy(h => h.Hero_Name);
                    break;
            }
            return View(heroes.ToList());
        }

        // GET: /Person/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var heroDetails = db.Heroes.Where(h => h.Hero_Id == id).SelectMany(h => h.HasAbility);
            if (heroDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;

            return View(heroDetails);
        }

        // GET: Heroes/Create
        public ActionResult Create()
        {
            ViewBag.Hero_Class = new SelectList(db.HERO_CLASSES, "HCLASS_ID", "HCLASS_TYPE");
            ViewBag.Ab_Type = new SelectList(db.ABILITY_TYPES, "ABTY_ID", "ABTY_TYPE");
            return View();
        }

        // POST: Heroes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Hero_Name,Hero_Class,Hero_PortraitLink,Ab_Name,Ab_Type")] Heroes hero, Abilities abilities)
        {
            // Set the hero ID
            hero.Hero_Id = GetNewHeroId();
            abilities.Ab_Id = GetNewAbilityId();

            if (ModelState.IsValid)
            {
                SetHasAbility(hero, abilities);
                db.Heroes.Add(hero);
                db.Abilities.Add(abilities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hero_Class = new SelectList(db.HERO_CLASSES, "HCLASS_ID", "HCLASS_TYPE", hero.Hero_Class);
            ViewBag.Ab_Type = new SelectList(db.ABILITY_TYPES, "ABTY_ID", "ABTY_TYPE", abilities.Ab_Type);
            return View(hero);
        }

        // GET: Heroes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var heroDetails = db.Heroes.Where(h => h.Hero_Id == id).SelectMany(h => h.HasAbility);
            if (heroDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hero_Class = new SelectList(db.HERO_CLASSES, "HCLASS_ID", "HCLASS_TYPE", heroDetails.ToList().ElementAt(0).Heroes.Hero_Class);
            ViewBag.Ab_Type = new SelectList(db.ABILITY_TYPES, "ABTY_ID", "ABTY_TYPE", heroDetails.ToList().ElementAt(0).Abilities.Ab_Type);
            return View(heroDetails);
        }

        // POST: Heroes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Hero_Id,Hero_Name,Hero_Class,Hero_PortraitLink,Ab_Id,Ab_Name,Ab_Type")]
        Heroes heroes, Abilities abilities, HasAbility ability)
        {
            Debug.WriteLine("");
            Debug.WriteLine("");

            Debug.WriteLine("");
            Debug.WriteLine(heroes.Hero_Id);
            Debug.WriteLine(heroes.Hero_Name);
            Debug.WriteLine(heroes.Hero_PortraitLink);
            Debug.WriteLine(heroes.Hero_Class);
            Debug.WriteLine("");

            Debug.WriteLine("");
            Debug.WriteLine(abilities.Ab_Id);
            Debug.WriteLine(abilities.Ab_Name);
            Debug.WriteLine(abilities.Ab_Type);
            Debug.WriteLine("");

            // For debugging
            var errors = ModelState
    .Where(x => x.Value.Errors.Count > 0)
    .Select(x => new { x.Key, x.Value.Errors })
    .ToArray();

            foreach (var item in errors)
            {
                Debug.WriteLine(item);
            }

            if (ModelState.IsValid)
            {
                db.Entry(heroes).State = EntityState.Modified;
                db.Entry(abilities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hero_Class = new SelectList(db.HERO_CLASSES, "HCLASS_ID", "HCLASS_TYPE", heroes.Hero_Class);
            ViewBag.Ab_Type = new SelectList(db.ABILITY_TYPES, "ABTY_ID", "ABTY_TYPE", abilities.Ab_Type);
            return View(heroes);
        }

        // GET: Heroes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heroes heroes = db.Heroes.Find(id);
            if (heroes == null)
            {
                return HttpNotFound();
            }
            return View(heroes);
        }

        // POST: Heroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Heroes heroes = db.Heroes.Find(id);
            db.Heroes.Remove(heroes);
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

        private bool ValidateHeroId(int id)
        {
            if(db.Heroes == null)
            {
                return true;
            }

            foreach(var hero in db.Heroes)
            {
                if (id == hero.Hero_Id)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateAbilityId(int id)
        {
            if (db.Abilities == null)
            {
                return true;
            }

            foreach (var ability in db.Abilities)
            {
                if (id == ability.Ab_Id)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateHasAbilityId(int id)
        {
            if (db.HasAbility == null)
            {
                return true;
            }

            foreach (var ability in db.HasAbility)
            {
                if (id == ability.Ha_ID)
                {
                    return false;
                }
            }

            return true;
        }

        private int GetNewHasAbilityId()
        {
            int id = db.HasAbility.Count() + 1;
            while (!ValidateHasAbilityId(id))
            {
                id++;
            }
            return id;
        }

        private int GetNewHeroId()
        {
            int id = db.Heroes.Count() + 1;
            while (!ValidateHeroId(id))
            {
                id++;
            }
            return id;
        }

        private int GetNewAbilityId()
        {
            int id = db.Abilities.Count() + 1;
            while (!ValidateAbilityId(id))
            {
                id++;
            }
            return id;
        }

        private void SetHasAbility(Heroes hero, Abilities abilities)
        {
            HasAbility hasAbility = new HasAbility();
            hasAbility.Ha_ID = GetNewHasAbilityId();
            hasAbility.HasHero = hero.Hero_Id;
            hasAbility.HasAbility1 = abilities.Ab_Id;
            db.HasAbility.Add(hasAbility);
        }
    }
}
