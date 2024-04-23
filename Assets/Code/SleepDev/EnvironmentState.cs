using UnityEngine;

namespace SleepDev
{
    public static class EnvironmentState
    {
        public static string[] VehicleTrailParticles =
        {
            "vehicle_trail_city", // city 0
            "vehicle_trail_city", // city chase 1
            "vehicle_trail_city", // city night 2
            "vehicle_trail_desert", // desert 3
            "vehicle_trail_desert", // desert night 4 
            "vehicle_trail_winter", // winter 5
            "vehicle_trail_winter", // winter night 6
            "vehicle_trail_city", // forest 7
            "vehicle_trail_city", // forest night 8
        };
        
        public static byte CurrentIndex { get; set; }
        public static bool IsNight { get; set; }

        public static ParticleSystem GetCurrentVehicleTrailPrefab()
        {
            return Resources.Load<ParticleSystem>($"Prefabs/FX/{VehicleTrailParticles[CurrentIndex]}");
        }
    }
}