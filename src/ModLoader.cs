using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ImportExportBreakdown
{
    public class ModLoader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
			Debug.Log("About to load");

            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
				Debug.Log("Bad load mode " + mode.ToString());
                return;
            }

			DistrictManager districtManager = Singleton<DistrictManager>.instance;

			uint[] averages = new uint[] {
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_averageAgricultural),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_averageForestry),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_averageOil),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_averageOre),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_averageGoods)
			};

			uint[] temps = new uint[] {
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_tempAgricultural),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_tempForestry),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_tempOil),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_tempOre),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_tempGoods)
			};

			uint[] finals = new uint[] {
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_finalAgricultural),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_finalForestry),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_finalOil),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_finalOre),
				NormalizeImportExportAmount(districtManager.m_districts.m_buffer[0].m_importData.m_finalGoods)
			};

			Debug.Log("averages: " + JoinUIntArray(averages));
			Debug.Log("temps: " + JoinUIntArray(temps));
			Debug.Log("finals: " + JoinUIntArray(finals));
        }

		private uint NormalizeImportExportAmount(uint amount) {
			return (amount + 99) / 100;
		}

		private string JoinUIntArray(uint[] array) {
			return string.Join(", ", new List<uint>(array).ConvertAll(i => i.ToString()).ToArray());
		}
    }
}