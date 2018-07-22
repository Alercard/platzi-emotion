using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmotionPlatzi.Web.Models
{
    public class EmoPicture
    {
        public int Id { get; set; }
        [Display(Name="Nombre")]
        public string Name { get; set; }
        [Required]
        [MaxLength(10,ErrorMessage ="La ruta supera el máximo establecido!")]
        public string Path { get; set; }

        // se indica la relacion con EmoFaces de 1 a muchos para que lo 
        // interprete EntityFramework
        public virtual ObservableCollection<EmoFace> Faces { get; set; }
    }
}