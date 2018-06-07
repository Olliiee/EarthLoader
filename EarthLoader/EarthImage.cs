using System;
using System.IO;
using System.Text;

namespace EarthLoader
{
    public class CentroidCoordinates
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class DscovrJ2000Position
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class LunarJ2000Position
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class SunJ2000Position
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class AttitudeQuaternions
    {
        public double q0 { get; set; }
        public double q1 { get; set; }
        public double q2 { get; set; }
        public double q3 { get; set; }
    }

    public class CentroidCoordinates2
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class DscovrJ2000Position2
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class LunarJ2000Position2
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class SunJ2000Position2
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class AttitudeQuaternions2
    {
        public double q0 { get; set; }
        public double q1 { get; set; }
        public double q2 { get; set; }
        public double q3 { get; set; }
    }

    public class Coords
    {
        public CentroidCoordinates2 centroid_coordinates { get; set; }
        public DscovrJ2000Position2 dscovr_j2000_position { get; set; }
        public LunarJ2000Position2 lunar_j2000_position { get; set; }
        public SunJ2000Position2 sun_j2000_position { get; set; }
        public AttitudeQuaternions2 attitude_quaternions { get; set; }
    }

    public class EarthImage
    {
        public string identifier { get; set; }
        public string caption { get; set; }
        public string image { get; set; }
        public string version { get; set; }
        public CentroidCoordinates centroid_coordinates { get; set; }
        public DscovrJ2000Position dscovr_j2000_position { get; set; }
        public LunarJ2000Position lunar_j2000_position { get; set; }
        public SunJ2000Position sun_j2000_position { get; set; }
        public AttitudeQuaternions attitude_quaternions { get; set; }
        public DateTime date { get; set; }
        public Coords coords { get; set; }
    }
}