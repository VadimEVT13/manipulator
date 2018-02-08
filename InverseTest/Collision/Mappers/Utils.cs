﻿using InverseTest.Collision.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Mappers
{
    public class Utils 
    {
        public static PartShape ExtractShapeFromModel(Model3D model)
        {
            var points = new Point3DCollection().ToList();


            if (model is Model3DGroup)
            {
                var modelGroup = (model as Model3DGroup).Children;
                foreach (Model3D m in modelGroup)
                {
                    points.AddRange(GetPointsFromModel(m));
                }
            }
            else {
                points.AddRange(GetPointsFromModel(model));
            }
            return new PartShape(model.Bounds, points, model.Transform.Value);
        }

        private static List<Point3D> GetPointsFromModel(Model3D model)
        {
            GeometryModel3D geometryModel = model as GeometryModel3D;
            MeshGeometry3D geom = geometryModel.Geometry as MeshGeometry3D;
            return geom.Positions.ToList();
        }
    }
}
