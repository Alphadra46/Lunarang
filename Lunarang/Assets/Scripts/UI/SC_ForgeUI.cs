using System;
using TMPro;
using UnityEngine;

public class SC_ForgeUI : MonoBehaviour
{

    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI statsTMP;

    public SC_Weapon weaponTest;

    private void Awake()
    {
        UpdateStatsInformations(weaponTest);
    }


    public void UpgradeWeapon(SC_Weapon upgradedWeapon)
    {

        upgradedWeapon.currentLevel++;
        UpdateStatsInformations(upgradedWeapon);
        
    }

    public void UnlockWeapon(SC_Weapon unlockedWeapon)
    {
        
        
        
    }

    public void UpdateStatsInformations(SC_Weapon weaponInfo)
    {
        
        
        levelTMP.text = string.Format("Level : {0}", weaponInfo.currentLevel);
        statsTMP.text = string.Format("Stats : \n{0}%\n{1}%\n{2}%",
            (weaponInfo.MovesValues[0] + (weaponInfo.levelUpStatsRate* (weaponInfo.currentLevel-1) )),
            (weaponInfo.MovesValues[1] + (weaponInfo.levelUpStatsRate* (weaponInfo.currentLevel-1) )),
            (weaponInfo.MovesValues[2] + (weaponInfo.levelUpStatsRate* (weaponInfo.currentLevel-1) ))
        );
        
    }
    
}
