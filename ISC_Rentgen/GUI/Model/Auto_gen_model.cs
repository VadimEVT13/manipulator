using ISC_Rentgen.GUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ISC_Rentgen.GUI.Model
{
    public delegate void OnRadiusChanged(double R);

    public class Auto_gen_model : Property_change_base
    {
        private static Auto_gen_model instance;
        public static Auto_gen_model getInstance { get { if (instance == null) instance = new Auto_gen_model(); return instance; } }
        public OnRadiusChanged RadiusChanged;

        private string methodic_name = "Шпангоут";
        public string Methodic_name { get { return methodic_name; } set { methodic_name = value; NotifyPropertyChanged(nameof(methodic_name)); } }

        private double radius = 1;
        public double Radius { get { return radius; } set { if (value > 0) { radius = value; NotifyPropertyChanged(nameof(radius)); RadiusChanged?.Invoke(radius); } } }

        private int num = 1;
        public int Num { get { return num; } set { if (value > 0) { num = value; NotifyPropertyChanged(nameof(num)); } } }

        private CollectionView methodic_collection = new CollectionView(new List<string>() { "Шпангоут", "Лопатка", "Шпангоут (дугами)" });
        public CollectionView Methodic_collection { get { return methodic_collection; } }
    }
}
