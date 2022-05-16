using System.Collections.Generic;

namespace ReMod.Core.VRChat
{
    public class VRCConfig
    {
        public List<string> betas { get; set; }
        public int ps_max_particles { get; set; }
        public int ps_max_systems { get; set; }
        public int ps_max_emission { get; set; }
        public int ps_max_total_emission { get; set; }
        public int ps_mesh_particle_divider { get; set; }
        public int ps_mesh_particle_poly_limit { get; set; }
        public int ps_collision_penalty_high { get; set; }
        public int ps_collision_penalty_med { get; set; }
        public int ps_collision_penalty_low { get; set; }
        public int ps_trails_penalty { get; set; }
        public int camera_res_height { get; set; }
        public int camera_res_width { get; set; }
        public int screenshot_res_height { get; set; }
        public int screenshot_res_width { get; set; }
        public int dynamic_bone_max_affected_transform_count { get; set; }
        public int dynamic_bone_max_collider_check_count { get; set; }
        public string cache_directory { get; set; }
        public int cache_size { get; set; }
        public int cache_expiry_delay { get; set; }
        public bool disableRichPresence { get; set; }
    }
}
