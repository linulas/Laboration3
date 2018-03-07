//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Laboration3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Abilities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Abilities()
        {
            this.HasAbility = new HashSet<HasAbility>();
        }
    
        public int Ab_Id { get; set; }
        [Required(ErrorMessage = "You must enter ability name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Must be at least 2 and max 50 charachters")]
        public string Ab_Name { get; set; }
        public int Ab_Type { get; set; }
    
        public virtual ABILITY_TYPES ABILITY_TYPES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HasAbility> HasAbility { get; set; }
    }
}
