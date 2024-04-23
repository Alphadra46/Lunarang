using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_ArchiveButtonUI : SerializedMonoBehaviour
{
    [BoxGroup("Settings")]
    public Dictionary<ArchiveState, Sprite> statesSprites = new Dictionary<ArchiveState, Sprite>();

    [BoxGroup("Component"), PropertySpace(SpaceBefore = 15f)]
    public TextMeshProUGUI nameTMP;
    [BoxGroup("Component")]
    public Image stateImage;
    [BoxGroup("Component")]
    public GameObject separator;

    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 15f)]
    public SO_Archive archive;


    public void Init(SO_Archive newArchive)
    {

        archive = newArchive;
        
        nameTMP.text = archive.archiveState is ArchiveState.Discover or ArchiveState.New ? archive.archiveName : "???";
        stateImage.sprite = statesSprites[archive.archiveState];

        if (archive.archiveState == ArchiveState.Special)
        {
            
            separator.SetActive(true);
            
        }

    }

    public void OnClick()
    {
        
        if (archive.archiveState == ArchiveState.New) archive.archiveState = ArchiveState.Discover;
        SC_LibraryUI.showInformations?.Invoke(archive);
        
    }
    
}
