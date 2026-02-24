using System.Linq;

/// <summary>
/// Summary description for AddParameter
/// </summary>
public class AddParameter
{
    public AddParameter()
    {

    }
    public void GetProjects()
    {
        NextOnServicesEntities3 db = new NextOnServicesEntities3();
        var abc = db.Projects.Where(x => x.ID == 1).ToList();
        int y = 1;
    }
}