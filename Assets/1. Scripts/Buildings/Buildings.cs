using CraftingResources;

namespace Buildings
{
    public enum BuildingType
    {
        Harvest,
        Refine,
        Utility,
        Terraform
    }

    public enum PlopLocation
    {
        Land,
        Ocean
    }

    public struct BuildingTextIDs
    {
        private const string HARVEST = "BUI-001";
        private const string REFINE = "BUI-002";
        private const string UTILITY = "BUI-003";
        private const string TERRAFORM = "BUI-004";

        public static string GetBuildingTextID(BuildingType type)
        {
            switch (type)
            {
                default:
                case BuildingType.Harvest:
                    return HARVEST;
                case BuildingType.Refine:
                    return REFINE;
                case BuildingType.Utility:
                    return UTILITY;
                case BuildingType.Terraform:
                    return TERRAFORM;
            }
        }
    }

    [System.Serializable]
    public struct BuildingMaterial
    {
        public MaterialData material;
        public int count;
    }
    [System.Serializable]
    public struct BuildingComponent
    {
        public ComponentData component;
        public int count;
    }
}