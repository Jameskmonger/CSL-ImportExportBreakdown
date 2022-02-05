using ColossalFramework;
using ColossalFramework.Math;
using ImportExportBreakdown.Redirection;
using UnityEngine;

namespace ImportExportBreakdown
{
    public static class CustomBuildingAIUtils
    {
        public static bool InConnectionImportView(InfoManager.SubInfoMode subInfoMode)
        {
            return subInfoMode == InfoManager.SubInfoMode.Default;
        }

        public static bool InConnectionExportView(InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
        {
            return subInfoMode == InfoManager.SubInfoMode.WaterPower;
        }

        private static bool ShouldLightenColor(TransferManager.TransferReason reason) {
            return (
                reason == TransferManager.TransferReason.Grain
                || reason == TransferManager.TransferReason.Logs
                || reason == TransferManager.TransferReason.Oil
                || reason == TransferManager.TransferReason.Ore
            );
        }

        private static bool ShouldDarkenColor(TransferManager.TransferReason reason) {
            return (
                reason == TransferManager.TransferReason.Food
                || reason == TransferManager.TransferReason.Lumber
                || reason == TransferManager.TransferReason.Petrol
                || reason == TransferManager.TransferReason.Coal
            );
        }

        public static Color GetAdjustedResourceColor(TransferManager.TransferReason reason) {
            var resourceColor = Singleton<TransferManager>.instance.m_properties.m_resourceColors[(int)reason];

            if (ShouldDarkenColor(reason)) {
                return Color.Lerp(resourceColor, Color.black, 0.2f);
            }

            if (ShouldLightenColor(reason)) {                
                return Color.Lerp(resourceColor, Color.white, 0.2f);
            }

            return resourceColor;
        }

        // copied from base class IndustrialBuildingAI/IndustrialExtractorAI
        public static TransferManager.TransferReason GetIncomingTransferReason(ushort buildingID, ItemClass.SubService subService)
        {
            switch (subService)
            {
                case ItemClass.SubService.IndustrialForestry:
                    return TransferManager.TransferReason.Logs;
                case ItemClass.SubService.IndustrialFarming:
                    return TransferManager.TransferReason.Grain;
                case ItemClass.SubService.IndustrialOil:
                    return TransferManager.TransferReason.Oil;
                case ItemClass.SubService.IndustrialOre:
                    return TransferManager.TransferReason.Ore;
                default:
                    switch (new Randomizer((int)buildingID).Int32(4U))
                    {
                        case 0:
                            return TransferManager.TransferReason.Lumber;
                        case 1:
                            return TransferManager.TransferReason.Food;
                        case 2:
                            return TransferManager.TransferReason.Petrol;
                        case 3:
                            return TransferManager.TransferReason.Coal;
                        default:
                            return TransferManager.TransferReason.None;
                    }
            }
        }

        // copied from base class IndustrialBuildingAI/IndustrialExtractorAI
        public static TransferManager.TransferReason GetOutgoingTransferReason(ItemClass.SubService subService)
        {
            switch (subService)
            {
                case ItemClass.SubService.IndustrialForestry:
                    return TransferManager.TransferReason.Lumber;
                case ItemClass.SubService.IndustrialFarming:
                    return TransferManager.TransferReason.Food;
                case ItemClass.SubService.IndustrialOil:
                    return TransferManager.TransferReason.Petrol;
                case ItemClass.SubService.IndustrialOre:
                    return TransferManager.TransferReason.Coal;
                default:
                    return TransferManager.TransferReason.Goods;
            }
        }
    }

    public class CustomIndustrialBuildingAI : PrivateBuildingAI
    {
        [RedirectFrom(typeof(IndustrialBuildingAI))]
        public override Color GetColor(
            ushort buildingID,
            ref Building data,
            InfoManager.InfoMode infoMode)
        {
            var currentSubMode = Singleton<InfoManager>.instance.CurrentSubMode;

            if (infoMode == InfoManager.InfoMode.Connections)
            {
                if (!this.ShowConsumption(buildingID, ref data))
                {
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                }

                if (CustomBuildingAIUtils.InConnectionImportView(currentSubMode))
                {
                    TransferManager.TransferReason incomingTransferReason = CustomBuildingAIUtils.GetIncomingTransferReason(buildingID, this.m_info.m_class.m_subService);

                    if (
                        incomingTransferReason == TransferManager.TransferReason.None
                        || (data.m_tempImport == (byte)0 && data.m_finalImport == (byte)0)
                    ) {
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    
                    return CustomBuildingAIUtils.GetAdjustedResourceColor(incomingTransferReason);
                }

                if (CustomBuildingAIUtils.InConnectionExportView(infoMode, currentSubMode))
                {
                    TransferManager.TransferReason outgoingTransferReason = CustomBuildingAIUtils.GetOutgoingTransferReason(this.m_info.m_class.m_subService);

                    if (
                        outgoingTransferReason == TransferManager.TransferReason.None
                        || (data.m_tempExport == (byte)0 && data.m_finalExport == (byte)0)
                    ) {
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    
                    return CustomBuildingAIUtils.GetAdjustedResourceColor(outgoingTransferReason);
                }
            }

            return base.GetColor(buildingID, ref data, infoMode);
        }
    }

    public class CustomIndustrialExtractorAI : PrivateBuildingAI
    {
        [RedirectFrom(typeof(IndustrialExtractorAI))]
        public override Color GetColor(
            ushort buildingID,
            ref Building data,
            InfoManager.InfoMode infoMode)
        {
            var currentSubMode = Singleton<InfoManager>.instance.CurrentSubMode;

            if (infoMode == InfoManager.InfoMode.Connections)
            {
                if (!this.ShowConsumption(buildingID, ref data))
                {
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                }

                if (CustomBuildingAIUtils.InConnectionImportView(currentSubMode))
                {
                    // no imports for extractor buildings
                    return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                }

                if (CustomBuildingAIUtils.InConnectionExportView(infoMode, currentSubMode))
                {
                    TransferManager.TransferReason outgoingTransferReason = CustomBuildingAIUtils.GetOutgoingTransferReason(this.m_info.m_class.m_subService);
                    
                    if (
                        outgoingTransferReason == TransferManager.TransferReason.None
                        || (data.m_tempExport == (byte)0 && data.m_finalExport == (byte)0)
                    ) {
                        return Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    
                    return CustomBuildingAIUtils.GetAdjustedResourceColor(outgoingTransferReason);
                }
            }

            return base.GetColor(buildingID, ref data, infoMode);
        }
    }
}