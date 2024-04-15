using UnityEngine;

namespace SleepDev
{
    public static class EnvironmentState
    {
        public static string[] VehicleTrailParticles = {"vehicle_trail_city", 
            "vehicle_trail_winter", "vehicle_trail_desert"};
        
        public static byte CurrentIndex { get; set; }
        public static bool IsNight { get; set; }

        public static ParticleSystem GetCurrentVehicleTrailPrefab()
        {
            return Resources.Load<ParticleSystem>($"Prefabs/FX/{VehicleTrailParticles[CurrentIndex]}");
        }
    }
}