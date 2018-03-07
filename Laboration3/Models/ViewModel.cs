using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboration3.Models
{
    public class ViewModel
    {
        public IEnumerable<Heroes> Heroes { get; set; }
        public IEnumerable<Abilities> Abilities { get; set; }
        public IEnumerable<HasAbility> HasAbilities { get; set; }
        public IEnumerable<HERO_CLASSES> HeroClasses { get; set; }
        public Heroes HeroClass { get; set; }
        public Heroes Hero { get; set; }
        public Abilities Ability { get; set; }
        public HasAbility HasAbility { get; set; }
    }
}