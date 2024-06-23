using Sirenix.OdinInspector;

public class SC_ArchiveInteractable : SC_InteractableBase
{
    [PropertySpace(SpaceBefore = 15f)]
    public SO_Archive archiveAttached;
    
    public void TakeArchive()
    {
        if(archiveAttached == null) return;
        
        SC_GameManager.instance.OpenArchiveUI(archiveAttached);
    }
    
    
}
