﻿using HelixToolkit.Wpf;
using ISC_Rentgen.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.ModelView
{
    public static class Emitter_and_scan_point_controller
    {
        private static Key_Point emitter_and_scan_point = new Key_Point(new Point3D(0, 0, 80), new Point3D(-10, 0, 80)); // начальное положение
        public  static Key_Point Emitter_and_scan_point { get { return emitter_and_scan_point; } set { emitter_and_scan_point = value; } }

        public static Model3DGroup Group { get; set; }
        private static string emitter_point_name { get { return "emitter_point"; } }
        private static string scan_point_name { get { return "scan_point"; } }

        public static void AddEitter(Point3D Emitter_point )
        {
            Emitter_and_scan_point.Emitter_point = Emitter_point;

            if (Group != null)
            {
                // Если уже есть такая точка, то удалить её
                while(Group.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
                {
                    Group.Children.Remove(Group.Children.Where(x => x.GetName() == emitter_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Emitter_point, 1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Red };
                gm.SetName(emitter_point_name);
                Group.Children.Add(gm);
            }
        }

        public static void AddScan(Point3D Scan_point)
        {
            if (Group != null)
            {
                Emitter_and_scan_point.Scan_point = Scan_point;

                // Если уже есть такая точка, то удалить её
                while(Group.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
                {
                    Group.Children.Remove(Group.Children.Where(x => x.GetName() == scan_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Scan_point, 1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Yellow };
                gm.SetName(scan_point_name);
                Group.Children.Add(gm);
            }
        }

        public static void Clear()
        {
            while (Group.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
            {
                Group.Children.Remove(Group.Children.Where(x => x.GetName() == emitter_point_name).First());
            }
            while (Group.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
            {
                Group.Children.Remove(Group.Children.Where(x => x.GetName() == scan_point_name).First());
            }
        }
    }
}